using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Core.Interfaces;
using Fujitsu.AFC.Core.Metrics;
using Fujitsu.AFC.Data.Interfaces;
using Fujitsu.AFC.Enumerations;
using Fujitsu.AFC.Extensions.CSOM;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Services.Common;
using Fujitsu.AFC.Services.Interfaces;
using Fujitsu.AFC.Services.Model;
using Fujitsu.AFC.Tasks.Resources;
using Fujitsu.Exceptions.Framework;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;


namespace Fujitsu.AFC.Services
{
    public class ProvisioningService : SharePointOnline, IProvisioningService
    {
        private readonly IParameterService _parameterService;
        private readonly IRepository<ProvisionedSiteCollection> _provisionedSiteCollectionRepository;
        private readonly IRepository<ProvisionedSite> _provisionedSiteRepository;
        private readonly IUserIdentity _userIdentity;
        private readonly IUnitOfWork _unitOfWork;

        public ProvisioningService(IParameterService parameterService,
            IRepository<ProvisionedSiteCollection> provisionedSiteCollectionRepository,
            IRepository<ProvisionedSite> provisionedSiteRepository,
            IUserIdentity userIdentity,
            IUnitOfWork unitOfWork)
             : base(parameterService)
        {
            if (parameterService == null)
            {
                throw new ArgumentNullException(nameof(parameterService));
            }
            if (provisionedSiteCollectionRepository == null)
            {
                throw new ArgumentNullException(nameof(provisionedSiteCollectionRepository));
            }
            if (provisionedSiteRepository == null)
            {
                throw new ArgumentNullException(nameof(provisionedSiteRepository));
            }
            if (userIdentity == null)
            {
                throw new ArgumentNullException(nameof(userIdentity));
            }
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            _parameterService = parameterService;
            _provisionedSiteCollectionRepository = provisionedSiteCollectionRepository;
            _provisionedSiteRepository = provisionedSiteRepository;
            _userIdentity = userIdentity;
            _unitOfWork = unitOfWork;
        }

        public int GetNumberOfUnallocatedSites()
        {
            return _provisionedSiteRepository.Query(x => x.IsAllocated == false).AsNoTracking().Count();
        }

        public int GetSiteCollectionId()
        {
            var sitesPerSiteCollection = _parameterService.GetParameterByNameAndCache<int>(ParameterNames.SiteProvisionerSitesPerSiteCollection);
            var siteCollections = _provisionedSiteRepository.All().GroupBy(x => x.ProvisionedSiteCollectionId).Select(group => new { Id = group.Key, Count = group.Count() }).Where(x => x.Count < sitesPerSiteCollection).OrderBy(x => x.Id).ToList();

            if (siteCollections.Any())
            {
                return siteCollections.First().Id;
            }

            var siteCollectionId = _parameterService.GetParameterByName<int>(ParameterNames.SiteProvisionerCurrentSiteCollectionId);
            _parameterService.SaveParameter<int>(ParameterNames.SiteProvisionerCurrentSiteCollectionId, ++siteCollectionId);
            return siteCollectionId;
        }

        public int GetNumberOfProvisionedSitesWithinCollection(int id)
        {
            return _provisionedSiteRepository.Query(x => x.ProvisionedSiteCollectionId == id).AsNoTracking().Count();
        }

        [ExcludeFromCodeCoverage]
        public int ProvisionSiteCollection(int siteCollectionId)
        {
            Debug.WriteLine("Fujitsu.AFC.Services.ProvisioningService.cs -> Processing ProvisionSiteCollection.");
            var prfMonMethod = new PrfMon();

            var siteCollectionName = $"{EnvironmentSiteCollectionPrefix}{siteCollectionId}";

            // Open the Tenant Administration Context with the Tenant Admin Url
            using (var tenantContext = new ClientContext(TenantAdminWebSiteDomainPath))
            {
                // Assign Credentials
                tenantContext.Credentials = Credentials;

                var tenant = new Tenant(tenantContext);

                var url = SiteCollectionUrl(siteCollectionId);


                // Properties of the New SiteCollection
                var siteCreationProperties = new SiteCreationProperties
                {
                    Url = url, // New Site Collection Url
                    Title = $"{siteCollectionName} Root Site", // Title of the Root Site
                    Owner = Username, // Email of Owner
                    Template = SiteTemplate, // Template of the Root Site. Using Team Site for now.
                    StorageMaximumLevel = SiteCollectionStorageMaximumLevel, // Storage Limit in MB
                    UserCodeMaximumLevel = SiteCollectionUserCodeMaximumLevel, // User Code Resource Points Allowed
                    StorageWarningLevel = SiteCollectionStorageWarningLevel
                };

                // Create the SiteCollection
                var spo = tenant.CreateSite(siteCreationProperties);

                tenantContext.Load(tenant);

                // We will need the IsComplete property to check if the provisioning of the Site Collection is complete.
                tenantContext.Load(spo, i => i.IsComplete);

                try
                {
                    tenantContext.ExecuteQuery();

                    // Check if the provisioning of the Site Collection is complete.
                    while (!spo.IsComplete)
                    {
                        // Wait for 30 seconds and then try again

                        System.Threading.Thread.Sleep(30000);
                        spo.RefreshLoad();

                        tenantContext.ExecuteQuery();
                    }
                }
                catch (Exception)
                {
                    var spp = tenant.GetSiteProperties(0, true);
                    tenantContext.Load(spp);
                    tenantContext.ExecuteQuery();


                    // Check whether the Site Collection has already been created
                    if (!spp.Any(x => x.Url == url))
                    {
                        throw;
                    }
                }
            }

            var now = DateTime.Now;

            var siteCollection = new ProvisionedSiteCollection
            {
                Name = siteCollectionName,
                InsertedDate = now,
                InsertedBy = _userIdentity.Name,
                UpdatedDate = now,
                UpdatedBy = _userIdentity.Name
            };

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    _provisionedSiteCollectionRepository.Insert(siteCollection);
                    _unitOfWork.Save();
                });

            Debug.WriteLine("Fujitsu.AFC.Services.ProvisioningService.cs -> Completed Processing ProvisionSiteCollection. Duration: {0:0.000}s", prfMonMethod.Stop());
            return siteCollection.Id;
        }

        [ExcludeFromCodeCoverage]
        public void ConfigureSiteCollection(int siteCollectionId)
        {
            Debug.WriteLine("Fujitsu.AFC.Services.ProvisioningService.cs -> Processing ConfigureSiteCollection.");
            var prfMonMethod = new PrfMon();

            var siteCollectionName = _provisionedSiteCollectionRepository.Single(x => x.Id == siteCollectionId).Name;

            using (var context = new ClientContext(SiteCollectionUrl(siteCollectionName)))
            {
                context.Credentials = Credentials;
                var searchCentreUrl = _parameterService.GetParameterByNameAndCache<string>(ParameterNames.SearchCentreUrl);
                var searchCentreResultsPageUrl = _parameterService.GetParameterByNameAndCache<string>(ParameterNames.SearchCentreResultsPageUrl);
                var model = new SearchResultModel
                {
                    Inherit = false,
                    ResultsPageAddress = searchCentreResultsPageUrl,
                    ShowNavigation = false
                };
                var jsonSearchSettings = JsonConvert.SerializeObject(model);
                context.ActivateFeature(InPlaceRecordManagementTypes.InplaceRecordsManagementFeatureId);
                context.SetPropertyBagValue(InPlaceRecordManagementTypes.EcmSiteRecordDeclarationDefault, true.ToString());
                context.SetPropertyBagValue(InPlaceRecordManagementTypes.EcmSiteRecordRestrictions, $"{EcmSiteRecordRestrictions.BlockDelete}, {EcmSiteRecordRestrictions.BlockEdit}");
                context.SetPropertyBagValue(InPlaceRecordManagementTypes.EcmSiteRecordDeclarationBy, EcmRecordDeclarationBy.AllListContributors.ToString());
                context.SetPropertyBagValue(InPlaceRecordManagementTypes.EcmSiteRecordUndeclarationBy, EcmRecordDeclarationBy.OnlyAdmins.ToString());
                context.SetPropertyBagValue(SearchCentreKeyTypes.SearchCentreUrl, searchCentreUrl);
                context.SetPropertyBagValue(SearchCentreKeyTypes.SearchCentreSite, jsonSearchSettings);
                var site = context.Site;
                context.Load(site, s => s.Audit);
                context.ExecuteQuery();

                site.Audit.AuditFlags = AuditMaskType.All;
                site.Audit.Update();
                context.ExecuteQuery();
            }

            Debug.WriteLine("Fujitsu.AFC.Services.ProvisioningService.cs -> Completed Processing ConfigureSiteCollection. Duration: {0:0.000}s", prfMonMethod.Stop());
        }

        [ExcludeFromCodeCoverage]
        public void CreateSiteAudit(int siteCollectionId, Task task)
        {
            Debug.WriteLine("Fujitsu.AFC.Services.ProvisioningService.cs -> Processing CreateSiteAudit.");
            var prfMonMethod = new PrfMon();

            var siteCollectionName = _provisionedSiteCollectionRepository.Single(x => x.Id == siteCollectionId).Name;

            using (var context = new ClientContext(SiteCollectionUrl(siteCollectionName)))
            {
                context.Credentials = Credentials;

                var listInformation = new ListCreationInformation
                {
                    Title = SharePointListNames.Audit,
                    TemplateType = (int)ListTemplateType.GenericList
                };

                try
                {
                    // Create the Audit List
                    var newList = context.Web.Lists.Add(listInformation);
                    newList.CreateNumberField("TaskId");
                    newList.CreateTextField("Event");
                    var auditDictionary = new Dictionary<string, object>
                        {
                            {"TaskId", task.Id},
                            {"Title", task.Name},
                            {"Event", string.Format(TaskResources.Audit_CaseSiteProvisioning_RootSiteCreated, siteCollectionName)}
                        };

                    newList.AddListItem(auditDictionary);
                    context.ExecuteQuery();

                }
                catch (Exception)
                {
                    // Do not propogate the exception if we already have the list
                    if (!context.Web.Lists.Any(x => x.Title == SharePointListNames.Audit))
                    {
                        throw;
                    }

                }
            }

            Debug.WriteLine("Fujitsu.AFC.Services.ProvisioningService.cs -> Completed Processing CreateSiteAudit. Duration: {0:0.000}s", prfMonMethod.Stop());
        }

        [ExcludeFromCodeCoverage]
        public void ProvisionSite(int siteCollectionId, Task task)
        {
            Debug.WriteLine("Fujitsu.AFC.Services.ProvisioningService.cs -> Processing ProvisionSite.");
            var prfMonMethod = new PrfMon();

            var siteCollection = _provisionedSiteCollectionRepository.Single(x => x.Id == siteCollectionId);
            var siteCollectionName = siteCollection.Name;
            const string sitePagesTitle = "Site Pages";

            using (var context = new ClientContext(SiteCollectionUrl(siteCollection.Name)))
            {
                Debug.WriteLine("Fujitsu.AFC.Services.ProvisioningService.cs -> CreateWebLoadContextWebAndLists. Started.");

                var createWebLoadContextWebListsPrfMonTriggers = new PrfMon();
                var guid = Guid.NewGuid().ToString();
                var creation = new WebCreationInformation
                {
                    Url = guid,
                    Title = guid,
                    WebTemplate = SiteTemplate,
                    UseSamePermissionsAsParentSite = false
                };
                context.Credentials = Credentials;
                var newWeb = context.Web.Webs.Add(creation);
                context.Load(newWeb, w => w.Title, w => w.Url);
                context.ExecuteQuery();
                Debug.WriteLine("Fujitsu.AFC.Services.ProvisioningService.cs -> CreateWebLoadContextWebAndLists. Completed. Average Execution: {0:0.000}s", createWebLoadContextWebListsPrfMonTriggers.Stop());

                Debug.WriteLine("Fujitsu.AFC.Services.ProvisioningService.cs -> LoadWebRootFolderAndFiles. Started.");
                var loadWebRootFolderAndFilesPrfMonTriggers = new PrfMon();
                var sitePages = newWeb.Lists.GetByTitle(sitePagesTitle);
                var rootFolder = sitePages.RootFolder;
                var files = rootFolder.Files;
                var webRootFolder = newWeb.RootFolder;
                context.Load(webRootFolder);
                context.Load(rootFolder);
                context.Load(files);
                context.Load(newWeb.RegionalSettings);
                context.Load(newWeb.RegionalSettings.TimeZones);
                context.ExecuteQuery();
                Debug.WriteLine("Fujitsu.AFC.Services.ProvisioningService.cs -> LoadWebRootFolderAndFiles. Completed. Average Execution: {0:0.000}s", loadWebRootFolderAndFilesPrfMonTriggers.Stop());

                Debug.WriteLine("Fujitsu.AFC.Services.ProvisioningService.cs -> SetSiteLocaleAndTimeZone. Started.");
                var setSiteLocaleTimeZonePrfMonTriggers = new PrfMon();
                newWeb.RegionalSettings.LocaleId = 2057;
                const string utcTimeZone = "(UTC) Coordinated Universal Time";
                var timeZone = newWeb.RegionalSettings.TimeZones.FirstOrDefault(timezone => timezone.Description == utcTimeZone);
                if (timeZone != null)
                {
                    newWeb.RegionalSettings.TimeZone = timeZone;
                }
                newWeb.RegionalSettings.Update();
                context.ExecuteQuery();
                Debug.WriteLine("Fujitsu.AFC.Services.ProvisioningService.cs -> SetSiteLocaleAndTimeZone. Completed. Average Execution: {0:0.000}s", setSiteLocaleTimeZonePrfMonTriggers.Stop());


                Debug.WriteLine("Fujitsu.AFC.Services.ProvisioningService.cs -> AddHomePageAndAuditSiteCreation. Started.");
                var addHomePageAndAuditSiteCreationPrfMonTriggers = new PrfMon();
                var defaultPageName = rootFolder.GetPageName(files, "default");
                var defaultPageUrl = $"{rootFolder.ServerRelativeUrl}/{defaultPageName}";
                rootFolder.Files.AddTemplateFile(defaultPageUrl, TemplateFileType.WikiPage);
                webRootFolder.WelcomePage = $"{sitePagesTitle.Replace(" ", "")}/{defaultPageName}";
                webRootFolder.Update();
                context.Web.AddAuditListItem(SharePointListNames.Audit, task, string.Format(TaskResources.Audit_CaseSiteProvisioning_SubSiteCreated, SiteUrl(siteCollectionName, guid)));
                context.ExecuteQuery();
                Debug.WriteLine("Fujitsu.AFC.Services.ProvisioningService.cs -> AddHomePageAndAuditSiteCreation. Completed. Average Execution: {0:0.000}s", addHomePageAndAuditSiteCreationPrfMonTriggers.Stop());

                Debug.WriteLine("Fujitsu.AFC.Services.ProvisioningService.cs -> SiteAccessPermissionsLevels. Started.");
                var addSiteAccessPermissionsLevelsPrfMonTriggers = new PrfMon();
                context.Load(newWeb, w => w.MembersCanShare, w => w.RequestAccessEmail);
                context.ExecuteQuery();
                newWeb.MembersCanShare = false;
                newWeb.RequestAccessEmail = string.Empty;
                newWeb.Update();
                context.ExecuteQuery();
                Debug.WriteLine("Fujitsu.AFC.Services.ProvisioningService.cs -> SiteAccessPermissionsLevels. Completed. Average Execution: {0:0.000}s", addSiteAccessPermissionsLevelsPrfMonTriggers.Stop());

                Debug.WriteLine("Fujitsu.AFC.Services.ProvisioningService.cs -> DatabaseSynchronisation. Started.");
                var databaseSynchronisationPrfMonTriggers = new PrfMon();
                var now = DateTime.Now;
                var site = new ProvisionedSite
                {
                    ProvisionedSiteCollectionId = siteCollectionId,
                    Name = guid,
                    Url = newWeb.Url,
                    InsertedDate = now,
                    InsertedBy = _userIdentity.Name,
                    UpdatedDate = now,
                    UpdatedBy = _userIdentity.Name
                };

                RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    _provisionedSiteRepository.Insert(site);
                    _unitOfWork.Save();
                });
                Debug.WriteLine("Fujitsu.AFC.Services.ProvisioningService.cs -> DatabaseSynchronisation. Completed. Average Execution: {0:0.000}s", databaseSynchronisationPrfMonTriggers.Stop());

            }

            Debug.WriteLine("Fujitsu.AFC.Services.ProvisioningService.cs -> Completed Processing ProvisionSite. Duration: {0:0.000}s", prfMonMethod.Stop());
        }
    }
}
