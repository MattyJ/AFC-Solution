using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Core.Interfaces;
using Fujitsu.AFC.Core.Metrics;
using Fujitsu.AFC.Data.Interfaces;
using Fujitsu.AFC.Extensions;
using Fujitsu.AFC.Extensions.CSOM;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Services.Common;
using Fujitsu.AFC.Services.Interfaces;
using Fujitsu.AFC.Services.Model;
using Fujitsu.AFC.Tasks.Resources;
using Fujitsu.Exceptions.Framework;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Site = Fujitsu.AFC.Model.Site;


namespace Fujitsu.AFC.Services
{
    public class PinService : SharePointOnline, IPinService
    {
        private readonly IParameterService _parameterService;
        private readonly IContextService _contextService;
        private readonly IService<Site> _siteService;
        private readonly IService<Library> _libraryService;
        private readonly IRepository<ProvisionedSite> _provisionedSiteRepository;
        private readonly IRepository<ProvisionedSiteCollection> _provisionedSiteCollectionRepository;
        private readonly IRepository<Site> _siteRepository;
        private readonly IRepository<Library> _libraryRepository;
        private readonly IUserIdentity _userIdentity;
        private readonly IUnitOfWork _unitOfWork;

        public PinService(IParameterService parameterService,
            IContextService contextService,
            IService<Site> siteService,
            ILibraryService libraryService,
            IRepository<ProvisionedSiteCollection> provisionedSiteCollectionRepository,
            IRepository<ProvisionedSite> provisionedSiteRepository,
            IRepository<Site> siteRepository,
            IRepository<Library> libraryRepository,
            IUserIdentity userIdentity,
            IUnitOfWork unitOfWork)
            : base(parameterService)
        {
            if (parameterService == null)
            {
                throw new ArgumentNullException(nameof(parameterService));
            }
            if (contextService == null)
            {
                throw new ArgumentNullException(nameof(contextService));
            }
            if (siteService == null)
            {
                throw new ArgumentNullException(nameof(siteService));
            }
            if (libraryService == null)
            {
                throw new ArgumentNullException(nameof(libraryService));
            }
            if (provisionedSiteCollectionRepository == null)
            {
                throw new ArgumentNullException(nameof(provisionedSiteCollectionRepository));
            }
            if (provisionedSiteRepository == null)
            {
                throw new ArgumentNullException(nameof(provisionedSiteRepository));
            }
            if (siteRepository == null)
            {
                throw new ArgumentNullException(nameof(siteRepository));
            }
            if (libraryRepository == null)
            {
                throw new ArgumentNullException(nameof(libraryRepository));
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
            _contextService = contextService;
            _siteService = siteService;
            _libraryService = libraryService;
            _provisionedSiteRepository = provisionedSiteRepository;
            _provisionedSiteCollectionRepository = provisionedSiteCollectionRepository;
            _siteRepository = siteRepository;
            _libraryRepository = libraryRepository;
            _userIdentity = userIdentity;
            _unitOfWork = unitOfWork;
        }

        [ExcludeFromCodeCoverage]
        public int AllocatePin(Task task)
        {
            Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> Processing AllocatePin.");
            var prfMonMethod = new PrfMon();

            int result;

            var provisionedSite = _provisionedSiteRepository.Query(x => x.IsAllocated == false).OrderBy(x => x.InsertedDate).First();
            var now = DateTime.Now;

            using (var context = new ClientContext(SiteCollectionUrl(provisionedSite.ProvisionedSiteCollection.Name)))
            {
                var web = _contextService.Load(context, Credentials);

                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> AllocatePinSite. Started.");
                var allocatedPinPrfMonTriggers = new PrfMon();
                var globalSystemsAdminUser = web.EnsureUser(_parameterService.GetParameterByNameAndCache<string>(ParameterNames.ActiveDirectoryGlobalSystemsAdministratorGroup));
                context.ExecuteQuery();
                var spSiteUrl = SiteUrl(provisionedSite.ProvisionedSiteCollection.Name, task.Pin.Value.ToString());
                if (!web.Webs.Any(x => x.Url == spSiteUrl))
                {
                    var spProvisionedSite = web.Webs.Single(x => x.Url == provisionedSite.Url);
                    spProvisionedSite.InitialiseAllocatedSite(provisionedSite, task, ServerRelativeSiteUrl(provisionedSite.ProvisionedSiteCollection.Name, task.Pin.Value.ToString()));
                    context.Load(web, w => w.Webs, w => w.Lists);
                    context.ExecuteQuery();

                    var spAllocatedPinSite = web.Webs.Single(x => x.Url == spSiteUrl);
                    var topNavBar = spAllocatedPinSite.Navigation.TopNavigationBar;
                    context.Load(topNavBar, x => x.Include(n => n.Url, n => n.Title));
                    context.ExecuteQuery();

                    var navBar = topNavBar.Single(x => x.Url == ServerRelativeSiteUrl(provisionedSite.ProvisionedSiteCollection.Name, task.Pin.Value.ToString()));
                    navBar.Title = task.SiteTitle;
                    navBar.Update();
                    context.ExecuteQuery();
                }
                if (!_provisionedSiteRepository.Any(x => x.Url == spSiteUrl))
                {
                    provisionedSite.IsAllocated = true;
                    provisionedSite.Name = task.Pin.Value.ToString();
                    provisionedSite.Url = spSiteUrl;
                    provisionedSite.UpdatedDate = now;
                    provisionedSite.UpdatedBy = _userIdentity.Name;

                    RetryableOperation.Invoke(ExceptionPolicies.General,
                    () =>
                    {
                        _provisionedSiteRepository.Update(provisionedSite);
                        _unitOfWork.Save();
                    });
                }
                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> AllocatePinSite. Completed. Average Execution: {0:0.000}s", allocatedPinPrfMonTriggers.Stop());

                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> CreateSharePointGroups. Started.");
                var createSharePointGroupsPrfMonTriggers = new PrfMon();
                var spSite = web.Webs.Single(x => x.Url == spSiteUrl);
                context.Load(spSite, s => s.Lists);
                spSite.AddRoleWithPermissions(context, string.Format(SharePointPinGroupNames.PinOwners, task.Pin.Value), RoleType.Administrator);
                spSite.AddRoleWithPermissions(context, string.Format(SharePointPinGroupNames.PinMembers, task.Pin.Value), RoleType.Contributor);
                spSite.AddRoleWithPermissions(context, string.Format(SharePointPinGroupNames.PinVisitors, task.Pin.Value), RoleType.Reader);
                context.ExecuteQuery();
                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> CreateSharePointGroups. Completed. Average Execution: {0:0.000}s", createSharePointGroupsPrfMonTriggers.Stop());

                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> AssignPermissions. Started.");
                var assignPermissionsPrfMonTriggers = new PrfMon();
                var colGroup = spSite.SiteGroups;
                context.Load(colGroup);
                var spGroup = colGroup.GetByName(string.Format(SharePointPinGroupNames.PinMembers, task.Pin.Value));
                context.Load(spGroup);
                spGroup.Users.AddUser(globalSystemsAdminUser);
                context.ExecuteQuery();
                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> AssignPermissions. Completed. Average Execution: {0:0.000}s", assignPermissionsPrfMonTriggers.Stop());

                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> LoadWebRootFolderAndFiles. Started.");
                var loadWebRootFolderAndFilesPrfMonTriggers = new PrfMon();
                const string sitePagesTitle = "Site Pages";
                var sitePages = spSite.Lists.GetByTitle(sitePagesTitle);
                var rootFolder = sitePages.RootFolder;
                var files = rootFolder.Files;
                context.Load(rootFolder, r => r.ServerRelativeUrl, r => r.Files);
                context.ExecuteQuery();
                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> LoadWebRootFolderAndFiles. Completed. Average Execution: {0:0.000}s", loadWebRootFolderAndFilesPrfMonTriggers.Stop());

                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> AddSearchWebPartAndNavigation. Started.");
                var addSearchWebPartAndNavigationPrfMonTriggers = new PrfMon();
                var searchPageName = rootFolder.GetPageName(files, "Document Index");
                var searchPageUrl = $"{rootFolder.ServerRelativeUrl}/{searchPageName}";
                rootFolder.Files.AddTemplateFile(searchPageUrl, TemplateFileType.WikiPage);
                context.ExecuteQuery();
                var webPartXml = WebPartNames.SearchWebPartXml.Replace("##SiteUrl##", spSiteUrl);
                spSite.AddWebToPartToPage(context, webPartXml, searchPageUrl);
                spSite.AddPageToNavigation(context, searchPageUrl);
                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> AddSearchWebPartAndNavigation. Completed. Average Execution: {0:0.000}s", addSearchWebPartAndNavigationPrfMonTriggers.Stop());

                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> CreateDocumentLibrary. Started.");
                var createDocumentLibraryPrfMonTriggers = new PrfMon();
                var documentLibraryList = spSite.CreateDocumentLibrary(SharePointListNames.PinDocumentsAllOneWord);
                context.Load(web, w => w.Webs, w => w.Lists, w => w.ContentTypes);
                context.Load(documentLibraryList, d => d.ContentTypes);
                context.ExecuteQuery();
                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> CreateDocumentLibrary. Completed. Average Execution: {0:0.000}s", createDocumentLibraryPrfMonTriggers.Stop());

                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> UpdateDocumentLibraryAndMetaData. Started.");
                var updateDocumentLibraryPrfMonTriggers = new PrfMon();
                documentLibraryList.UpdateDocumentLibraryList(web.ContentTypes, ContentTypeGroupNames.PinDocumentContentTypes, SharePointListNames.PinDocuments);
                context.Load(documentLibraryList, d => d.Id, d => d.Fields);
                context.ExecuteQuery();
                var dictionary = context.CreateDocumentLibraryMetaData(documentLibraryList, spSite, task.Dictionary);
                context.ExecuteQuery();
                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> UpdateDocumentLibraryAndMetaData. Completed. Average Execution: {0:0.000}s", updateDocumentLibraryPrfMonTriggers.Stop());

                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> AddListViewWebPart. Started.");
                var addListViewWebPartPrfMonTriggers = new PrfMon();
                string pageUrl = $"{spSite.ServerRelativeUrl}/{spSite.GetWelcomePageUrl(context)}";
                spSite.AddWebToPartToPage(context, string.Format(WebPartNames.ListViewWebPartXml, SharePointListNames.PinDocumentsAllOneWord), pageUrl);
                context.ExecuteQuery();
                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> AddListViewWebPart. Completed. Average Execution: {0:0.000}s", addListViewWebPartPrfMonTriggers.Stop());

                _contextService.Audit(context, task, string.Format(TaskResources.Audit_AllocatePin, task.Pin.Value, spSiteUrl));

                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> SiteDatabaseSynchronisation. Started.");
                var databaseSynchronisationPrfMonTriggers = new PrfMon();
                var site = new Site
                {
                    Pin = task.Pin.Value,
                    Title = task.SiteTitle,
                    ProvisionedSiteId = provisionedSite.Id,
                    Url = spSiteUrl,
                    RestrictedUser = false,
                    Dictionary = dictionary,
                    InsertedDate = now,
                    InsertedBy = _userIdentity.Name,
                    UpdatedDate = now,
                    UpdatedBy = _userIdentity.Name
                };
                result = _siteService.Create(site);
                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> SiteDatabaseSynchronisation. Completed. Average Execution: {0:0.000}s", databaseSynchronisationPrfMonTriggers.Stop());
            }

            Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> Completed Processing AllocatePin. Duration: {0:0.000}s", prfMonMethod.Stop());
            return result;
        }

        [ExcludeFromCodeCoverage]
        public int UpdatePinTitle(Task task)
        {
            Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> Processing UpdatePinTitle.");
            var prfMonMethod = new PrfMon();

            var site = _siteRepository.Single(x => x.Pin == task.Pin.Value);

            using (var context = new ClientContext(SiteCollectionUrl(site.ProvisionedSite.ProvisionedSiteCollection.Name)))
            {
                var web = _contextService.Load(context, Credentials);
                var spSite = web.Webs.Single(x => x.Url == site.Url);

                if (!spSite.Title.SafeEquals(task.SiteTitle))
                {
                    Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> UpdateSiteTitleAndTopNavBar. Started.");
                    var updateSitePrfMonTriggers = new PrfMon();
                    spSite.Title = task.SiteTitle;
                    spSite.Update();
                    var topNavBar = spSite.Navigation.TopNavigationBar;
                    context.Load(topNavBar, x => x.Include(n => n.Url, n => n.Title));
                    context.ExecuteQuery();
                    var navBar = topNavBar.Single(x => x.Url == ServerRelativeSiteUrl(site.ProvisionedSite.ProvisionedSiteCollection.Name, task.Pin.Value.ToString()));
                    navBar.Title = task.SiteTitle;
                    navBar.Update();
                    context.ExecuteQuery();
                    Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> UpdateSiteAndTopNavBar. Completed. Average Execution: {0:0.000}s", updateSitePrfMonTriggers.Stop());

                    _contextService.Audit(context, task, string.Format(TaskResources.Audit_UpdateServiceUserTitle, site.Pin, site.Title, task.SiteTitle));

                    Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> DatabaseSynchronisation. Started.");
                    var databaseSynchronisationPrfMonTriggers = new PrfMon();
                    site.Title = task.SiteTitle;
                    site.UpdatedDate = DateTime.Now;
                    site.UpdatedBy = _userIdentity.Name;
                    _siteService.Update(site);
                    Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> DatabaseSynchronisation. Completed. Average Execution: {0:0.000}s", databaseSynchronisationPrfMonTriggers.Stop());
                }
            }

            Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> Completed Processing UpdatePinTitle. Duration: {0:0.000}s", prfMonMethod.Stop());
            return site.Id;
        }

        [ExcludeFromCodeCoverage]
        public int UpdatePinWithDictionaryValues(Task task)
        {
            Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> Processing UpdatePinWithDictionaryValues.");
            var prfMonMethod = new PrfMon();

            var site = _siteRepository.Single(x => x.Pin == task.Pin.Value);

            using (var context = new ClientContext(SiteCollectionUrl(site.ProvisionedSite.ProvisionedSiteCollection.Name)))
            {
                var web = _contextService.Load(context, Credentials);

                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs ->  UpdateDocumentLibraryMetaData. Started.");
                var loadPinDocumentListContentTypesAndFieldsPrfMonTriggers = new PrfMon();
                var spSite = web.Webs.Single(x => x.Url == site.Url);
                var documentLibraryList = spSite.Lists.GetByTitle(SharePointListNames.PinDocuments);
                context.Load(documentLibraryList, d => d.Id, d => d.Fields);
                context.ExecuteQuery();
                var dictionary = context.UpdateDocumentLibraryMetaData(documentLibraryList, spSite, site.Dictionary, task.Dictionary);
                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> UpdateDocumentLibraryMetaData. Completed. Average Execution: {0:0.000}s", loadPinDocumentListContentTypesAndFieldsPrfMonTriggers.Stop());

                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> DatabaseSynchronisation. Started.");
                var databaseSynchronisationPrfMonTriggers = new PrfMon();
                site.Dictionary = dictionary;
                site.UpdatedDate = DateTime.Now;
                site.UpdatedBy = _userIdentity.Name;
                _siteService.Update(site);
                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> DatabaseSynchronisation. Completed. Average Execution: {0:0.000}s", databaseSynchronisationPrfMonTriggers.Stop());


                _contextService.Audit(context, task, string.Format(TaskResources.Audit_UpdatePinWithDictionaryValues, site.Pin));
            }

            Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> Completed Processing UpdatePinWithDictionaryValues. Duration: {0:0.000}s", prfMonMethod.Stop());
            return site.Id;
        }

        [ExcludeFromCodeCoverage]
        public int ChangePrimaryProject(Task task)
        {
            Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> Processing ChangePrimaryProject.");
            var prfMonMethod = new PrfMon();

            var site = _siteRepository.Single(x => x.Pin == task.Pin.Value);

            using (var context = new ClientContext(SiteCollectionUrl(site.ProvisionedSite.ProvisionedSiteCollection.Name)))
            {
                var web = _contextService.Load(context, Credentials);

                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> CheckRequiredSecurityGroupsExist. Started.");
                var obtainRequiredUsersPrfMonTriggers = new PrfMon();
                var spSite = web.Webs.Single(x => x.Url == site.Url);
                var colGroup = spSite.SiteGroups;
                context.Load(colGroup);
                var activeDirectoryContributeGroup = _parameterService.GetParameterByNameAndCache<string>(ParameterNames.ActiveDirectoryContributeGroup);
                var activeDirectoryContributeClosedGroup = _parameterService.GetParameterByNameAndCache<string>(ParameterNames.ActiveDirectoryContributeClosedGroup);
                var currentProjectContributorsGroup = web.GetActiveDirectoryGroup(activeDirectoryContributeGroup, site.RestrictedUser, site.Pin, task.CurrentProjectId.Value);
                var currentProjectContributorsClosedGroup = web.GetActiveDirectoryGroup(activeDirectoryContributeClosedGroup, site.RestrictedUser, site.Pin, task.CurrentProjectId.Value);
                var newProjectContributorsGroup = web.GetActiveDirectoryGroup(activeDirectoryContributeGroup, site.RestrictedUser, site.Pin, task.NewProjectId.Value);
                var spPinMembersGroup = colGroup.GetByName(string.Format(SharePointPinGroupNames.PinMembers, site.Pin));
                context.Load(spPinMembersGroup.Users, x => x.Include(u => u.Id, u => u.PrincipalType, u => u.Title));
                context.Load(currentProjectContributorsGroup, u => u.Title);
                context.Load(currentProjectContributorsClosedGroup, u => u.Title);
                context.Load(newProjectContributorsGroup);
                context.ExecuteQuery();
                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> CheckRequiredSecurityGroupsExist. Completed. Average Execution: {0:0.000}s", obtainRequiredUsersPrfMonTriggers.Stop());

                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> AssignPermissions. Started.");
                var assignPermissionsPrfMonTriggers = new PrfMon();
                var projectContributors = spPinMembersGroup.Users.Where(u => u.Title == currentProjectContributorsGroup.Title || u.Title == currentProjectContributorsClosedGroup.Title).Select(s => s.Id).ToList();
                if (projectContributors.Any())
                {
                    foreach (var projectContributor in projectContributors)
                    {
                        spPinMembersGroup.Users.RemoveById(projectContributor);
                    }
                }
                spPinMembersGroup.Users.AddUser(newProjectContributorsGroup);
                context.ExecuteQuery();
                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> AssignPermissions. Completed. Average Execution: {0:0.000}s", assignPermissionsPrfMonTriggers.Stop());

                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs ->  UpdateDocumentLibraryMetaData. Started.");
                var loadPinDocumentListContentTypesAndFieldsPrfMonTriggers = new PrfMon();
                var documentLibraryList = spSite.Lists.GetByTitle(SharePointListNames.PinDocuments);
                context.Load(documentLibraryList, d => d.Id, d => d.Fields);
                context.ExecuteQuery();
                var dictionary = context.UpdateDocumentLibraryMetaData(documentLibraryList, spSite, site.Dictionary, task.Dictionary);
                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> UpdateDocumentLibraryMetaData. Completed. Average Execution: {0:0.000}s", loadPinDocumentListContentTypesAndFieldsPrfMonTriggers.Stop());

                _contextService.Audit(context, task, string.Format(TaskResources.Audit_ChangePrimaryProject, site.Pin, task.CurrentProjectId.Value, task.NewProjectId.Value));

                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> DatabaseSynchronisation. Started.");
                var databaseSynchronisationPrfMonTriggers = new PrfMon();
                RetryableOperation.Invoke(ExceptionPolicies.General,
                    () =>
                    {
                        site.Dictionary = dictionary;
                        site.PrimaryProjectId = task.NewProjectId;
                        site.UpdatedDate = DateTime.Now;
                        site.UpdatedBy = _userIdentity.Name;
                        _siteRepository.Update(site);
                        _unitOfWork.Save();
                    });
                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> DatabaseSynchronisation. Completed. Average Execution: {0:0.000}s", databaseSynchronisationPrfMonTriggers.Stop());

            }

            Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> Completed Processing ChangePrimaryProject. Duration: {0:0.000}s", prfMonMethod.Stop());
            return site.Id;
        }

        [ExcludeFromCodeCoverage]
        public int DeletePin(Task task)
        {
            Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> Processing DeletePin.");
            var prfMonMethod = new PrfMon();

            var site = _siteRepository.Single(x => x.Pin == task.Pin.Value);

            using (var siteContext = new ClientContext(site.Url))
            {
                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> SiteDeletion. Started.");
                var siteDeletionPrfMonTriggers = new PrfMon();
                siteContext.Credentials = Credentials;
                var siteWeb = siteContext.Web;
                siteContext.Load(siteWeb);
                siteContext.ExecuteQuery();
                siteWeb.DeleteObject();
                siteContext.ExecuteQuery();
                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> SiteDeletion. Completed. Average Execution: {0:0.000}s", siteDeletionPrfMonTriggers.Stop());
            }

            using (var siteCollectionContext = new ClientContext(SiteCollectionUrl(site.ProvisionedSite.ProvisionedSiteCollection.Name)))
            {
                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> AuditOperation. Started.");
                var auditOperationPrfMonTriggers = new PrfMon();
                siteCollectionContext.Credentials = Credentials;
                siteCollectionContext.ExecuteQuery();
                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> AuditOperation. Completed. Average Execution: {0:0.000}s", auditOperationPrfMonTriggers.Stop());

                _contextService.Audit(siteCollectionContext, task, string.Format(TaskResources.Audit_DeletePin, site.Url));
            }

            Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> DatabaseSynchronisation. Started.");
            var databaseSynchronisationPrfMonTriggers = new PrfMon();
            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    var provisionedSiteId = site.ProvisionedSiteId;
                    var provisionedSiteCollectionId = site.ProvisionedSite.ProvisionedSiteCollectionId;
                    _siteRepository.Delete(site);
                    _provisionedSiteRepository.Delete(provisionedSiteId);
                    if (_provisionedSiteRepository.Query(x => x.ProvisionedSiteCollectionId == provisionedSiteCollectionId).Count() == 1)
                    {
                        _provisionedSiteCollectionRepository.Delete(provisionedSiteCollectionId);
                    }

                    _unitOfWork.Save();
                });
            Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> DatabaseSynchronisation. Completed. Average Execution: {0:0.000}s", databaseSynchronisationPrfMonTriggers.Stop());


            Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> Completed Processing DeletePin. Duration: {0:0.000}s", prfMonMethod.Stop());
            return site.Id;
        }



        [ExcludeFromCodeCoverage]
        public void MergePin(Task task)
        {
            Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> Processing MergePin.");
            var prfMonMethod = new PrfMon();

            var fromSite = _siteRepository.Single(x => x.Pin == task.FromPin.Value);
            var toSite = _siteRepository.Single(x => x.Pin == task.ToPin.Value);
            var fromSiteDocumentLibraries = _libraryRepository.Query(x => x.SiteId == fromSite.Id).ToList();

            var activeDirectoryContributeGroup = _parameterService.GetParameterByNameAndCache<string>(ParameterNames.ActiveDirectoryContributeGroup);
            var activeDirectoryContributeClosedGroup = _parameterService.GetParameterByNameAndCache<string>(ParameterNames.ActiveDirectoryContributeClosedGroup);
            var activeDirectoryReadGroup = _parameterService.GetParameterByNameAndCache<string>(ParameterNames.ActiveDirectoryReadGroup);
            var pinSiteDocumentLibraries = new Dictionary<Guid, Guid>();

            using (var context = new ClientContext(SiteCollectionUrl(fromSite.ProvisionedSite.ProvisionedSiteCollection.Name)))
            {
                var web = _contextService.Load(context, Credentials);

                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> CheckRequiredSecurityGroupsExist. Started.");
                var checkRequiredSecurityGroupsExistPrfMonTriggers = new PrfMon();
                var spFromSite = web.Webs.Single(x => x.Url == fromSite.Url);
                var colGroup = spFromSite.SiteGroups;
                context.Load(colGroup);
                var globalSystemsAdminUser =
                    web.EnsureUser(
                        _parameterService.GetParameterByNameAndCache<string>(
                            ParameterNames.ActiveDirectoryGlobalSystemsAdministratorGroup));
                foreach (var library in
                    fromSiteDocumentLibraries.Select(s => new { s.ProjectId, s.IsClosed }).Distinct().ToList())
                {
                    web.GetActiveDirectoryContributorGroup(activeDirectoryContributeGroup,
                        activeDirectoryContributeClosedGroup, fromSite.RestrictedUser, library.IsClosed, fromSite.Pin,
                        library.ProjectId);
                    web.GetActiveDirectoryGroup(activeDirectoryReadGroup, fromSite.RestrictedUser, fromSite.Pin,
                        library.ProjectId);
                }
                context.Load(globalSystemsAdminUser, u => u.Id);
                var spFromPinMembersGroup = colGroup.GetByName(string.Format(SharePointPinGroupNames.PinMembers, fromSite.Pin));
                var spFromPinVisitorsGroup = colGroup.GetByName(string.Format(SharePointPinGroupNames.PinVisitors, fromSite.Pin));
                context.Load(spFromPinMembersGroup.Users, x => x.Include(u => u.Id, u => u.PrincipalType, u => u.Title));
                context.Load(spFromPinVisitorsGroup.Users, x => x.Include(u => u.Id, u => u.PrincipalType, u => u.Title));
                context.ExecuteQuery();
                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> CheckRequiredSecurityGroupsExist. Completed. Average Execution: {0:0.000}s", checkRequiredSecurityGroupsExistPrfMonTriggers.Stop());

                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> CheckFromPinSiteDocumentSizes. Started.");
                var checkFromPinSiteDocumentSizesPrfMonTriggers = new PrfMon();
                var maximumFileSizeBytes = _parameterService.GetParameterByNameAndCache<int>(ParameterNames.MaximumFileSizeMb) * 1024 * 1024;
                var allLists = spFromSite.Lists;
                context.Load(allLists, lc => lc.Include(l => l.Id, l => l.Title, l => l.IsSiteAssetsLibrary, l => l.Hidden,
                         l => l.BaseType, l => l.ContentTypesEnabled, l => l.IsApplicationList, l => l.IsCatalog,
                         l => l.IsPrivate, l => l.RootFolder.ServerRelativeUrl));
                context.ExecuteQuery();
                foreach (var list in allLists)
                {
                    if (!list.Hidden && (list.BaseType == BaseType.DocumentLibrary) && list.ContentTypesEnabled &&
                        !list.IsApplicationList && !list.IsCatalog && !list.IsSiteAssetsLibrary && !list.IsPrivate)
                    {
                        var folder = list.RootFolder;
                        folder.CheckFolderFileSizesWithinSizeLimit(context, fromSite.Pin, list.Title, maximumFileSizeBytes);
                    }
                }

                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> CheckFromPinSiteDocumentSizes. Completed. Average Execution: {0:0.000}s", checkFromPinSiteDocumentSizesPrfMonTriggers.Stop());

                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> PinFromSitePermissions. Started.");
                var pinSitePermissionsPrfMonTriggers = new PrfMon();
                var contributeUsers = spFromPinMembersGroup.Users.Where(u => u.PrincipalType == PrincipalType.SecurityGroup &&
                                                                                    u.Title.EndsWith("_CONTRIBUTE") ||
                                                                                    u.Title.EndsWith("_CONTRIBUTE_CLOSED") ||
                                                                                    u.Title.EndsWith($"_CONTRIBUTE_{fromSite.Pin}") ||
                                                                                    u.Title.EndsWith($"_CONTRIBUTE_CLOSED_{fromSite.Pin}")).ToList();
                foreach (var user in contributeUsers)
                {
                    spFromPinMembersGroup.Users.RemoveById(user.Id);
                }
                var readUsers = spFromPinVisitorsGroup.Users.Where(u => u.PrincipalType == PrincipalType.SecurityGroup && u.Title.EndsWith("_READ") || u.Title.EndsWith($"_READ_{fromSite.Pin}")).ToList();
                foreach (var user in readUsers)
                {
                    spFromPinVisitorsGroup.Users.RemoveById(user.Id);
                }
                var globalSystemAdminUser = spFromPinMembersGroup.Users.FirstOrDefault(u => u.Id == globalSystemsAdminUser.Id);
                if (globalSystemAdminUser != null)
                {
                    spFromPinMembersGroup.Users.RemoveById(globalSystemsAdminUser.Id);
                }
                context.ExecuteQuery();
                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> PinFromSitePermissions. Completed. Average Execution: {0:0.000}s", pinSitePermissionsPrfMonTriggers.Stop());

                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> RemoveDocumentLibraryPermissions. Started.");
                var libraryPermissionsPrfMonTriggers = new PrfMon();
                foreach (var documentLibrary in fromSiteDocumentLibraries)
                {
                    var documentLibraryList = spFromSite.Lists.GetById(documentLibrary.ListId);
                    documentLibraryList.OnQuickLaunch = false;
                    documentLibraryList.Update();
                    context.Load(documentLibraryList);
                    context.Load(documentLibraryList.RoleAssignments, r => r.Include(m => m.Member.Id));
                    context.ExecuteQuery();
                    var currentProjectContributorsGroup = web.GetActiveDirectoryContributorGroup(activeDirectoryContributeGroup, activeDirectoryContributeClosedGroup, fromSite.RestrictedUser, documentLibrary.IsClosed, fromSite.Pin, documentLibrary.ProjectId);
                    context.Load(currentProjectContributorsGroup, u => u.Id);
                    context.ExecuteQuery();
                    documentLibraryList.RoleAssignments.Where(r => r.Member.Id == currentProjectContributorsGroup.Id || r.Member.Id == globalSystemsAdminUser.Id).ToList().ForEach(ra => ra.DeleteObject());
                    context.ExecuteQuery();
                }
                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> RemoveDocumentLibraryPermissions. Completed. Average Execution: {0:0.000}s", libraryPermissionsPrfMonTriggers.Stop());

                _contextService.Audit(context, task, string.Format(TaskResources.Audit_MergePin, fromSite.Pin, toSite.Pin));
            }

            using (var context = new ClientContext(SiteCollectionUrl(toSite.ProvisionedSite.ProvisionedSiteCollection.Name)))
            {
                var web = _contextService.Load(context, Credentials);

                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> LoadRequiredSecurityGroups. Started.");
                var loadRequiredSecurityGroupsExistPrfMonTriggers = new PrfMon();
                var spToSite = web.Webs.Single(x => x.Url == toSite.Url);
                var colGroup = spToSite.SiteGroups;
                context.Load(colGroup);
                var globalSystemsAdminUser = web.EnsureUser(_parameterService.GetParameterByNameAndCache<string>(ParameterNames.ActiveDirectoryGlobalSystemsAdministratorGroup));
                context.Load(globalSystemsAdminUser, u => u.Id);
                var spToPinMembersGroup = colGroup.GetByName(string.Format(SharePointPinGroupNames.PinMembers, toSite.Pin));
                var spToPinVisitorsGroup = colGroup.GetByName(string.Format(SharePointPinGroupNames.PinVisitors, toSite.Pin));
                context.Load(spToPinMembersGroup.Users, x => x.Include(u => u.Id, u => u.PrincipalType, u => u.Title));
                context.Load(spToPinVisitorsGroup.Users, x => x.Include(u => u.Id, u => u.PrincipalType, u => u.Title));
                context.Load(spToSite, s => s.Lists.Include(l => l.Title));
                context.ExecuteQuery();
                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> LoadRequiredSecurityGroups. Completed. Average Execution: {0:0.000}s", loadRequiredSecurityGroupsExistPrfMonTriggers.Stop());

                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> CreateDocumentLibraries. Started.");
                var createDocumentLibrariesPrfMonTriggers = new PrfMon();
                var mergePinLibraries = fromSiteDocumentLibraries.ToList();
                foreach (var mergePinLibrary in mergePinLibraries)
                {
                    Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> CreateDocumentLibrary. Started.");
                    var createDocumentLibraryPrfMonTriggers = new PrfMon();
                    var documentLibraryList = spToSite.CreateDocumentLibrary(mergePinLibrary.CaseId.ToString());
                    context.Load(documentLibraryList, d => d.ContentTypes);
                    context.ExecuteQuery();
                    Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> CreateDocumentLibrary. Completed. Average Execution: {0:0.000}s", createDocumentLibraryPrfMonTriggers.Stop());

                    Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> UpdateDocumentLibraryList. Started.");
                    var updateDocumentLibraryPrfMonTriggers = new PrfMon();
                    documentLibraryList.UpdateDocumentLibraryList(web.ContentTypes, ContentTypeGroupNames.CaseDocumentContentTypes, mergePinLibrary.Title);
                    context.Load(documentLibraryList, d => d.Id, d => d.Fields);
                    context.ExecuteQuery();
                    Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> UpdateDocumentLibraryList. Completed. Average Execution: {0:0.000}s", updateDocumentLibraryPrfMonTriggers.Stop());

                    Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> UpdateDocumentLibraryMetaData. Started.");
                    var documentLibraryMetaDataPrfMonTriggers = new PrfMon();
                    var dictionary = context.CreateDocumentLibraryMetaData(documentLibraryList, spToSite, mergePinLibrary.Dictionary, toSite.Pin);
                    Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> UpdateDocumentLibraryMetaData. Completed. Average Execution: {0:0.000}s", documentLibraryMetaDataPrfMonTriggers.Stop());

                    Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> PinToSiteVisitorsPermissions. Started.");
                    var pinSitePermissionsPrfMonTriggers = new PrfMon();
                    var projectReadGroup = web.GetActiveDirectoryGroup(activeDirectoryReadGroup, toSite.RestrictedUser, toSite.Pin, mergePinLibrary.ProjectId);
                    spToPinVisitorsGroup.Users.AddUser(projectReadGroup);
                    context.Load(spToPinVisitorsGroup);
                    context.Load(documentLibraryList.RoleAssignments);
                    context.ExecuteQuery();
                    Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> PinToSiteVisitorsPermissions. Completed. Average Execution: {0:0.000}s", pinSitePermissionsPrfMonTriggers.Stop());

                    Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> SetupDocumentLibraryMembershipPermissions. Started.");
                    var setupDocumentLibraryMembershipPermissionsPrfMonTriggers = new PrfMon();
                    var projectContributorsGroup = web.GetActiveDirectoryContributorGroup(activeDirectoryContributeGroup, activeDirectoryContributeClosedGroup, toSite.RestrictedUser,
                        mergePinLibrary.IsClosed, toSite.Pin, mergePinLibrary.ProjectId);
                    documentLibraryList.BreakRoleInheritanceAndRemoveRoles(true, false, true);
                    documentLibraryList.AddRoleWithPermissions(context, web, globalSystemsAdminUser, RoleType.Contributor);
                    documentLibraryList.AddRoleWithPermissions(context, web, projectContributorsGroup, RoleType.Contributor);
                    documentLibraryList.AddRoleWithPermissions(context, web, spToPinVisitorsGroup, RoleType.Reader);
                    documentLibraryList.Update();
                    context.ExecuteQuery();
                    Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> SetupDocumentLibraryMembershipPermissions. Completed. Average Execution: {0:0.000}s", setupDocumentLibraryMembershipPermissionsPrfMonTriggers.Stop());

                    Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> DatabaseSynchronisation. Started.");
                    var databaseSynchronisationPrfMonTriggers = new PrfMon();
                    pinSiteDocumentLibraries.Add(mergePinLibrary.ListId, documentLibraryList.Id);
                    var now = DateTime.Now;
                    var library = new Library
                    {
                        CaseId = mergePinLibrary.CaseId,
                        Title = mergePinLibrary.Title,
                        ProjectId = mergePinLibrary.ProjectId,
                        SiteId = toSite.Id,
                        ListId = documentLibraryList.Id,
                        IsClosed = mergePinLibrary.IsClosed,
                        Url = $"{CaseDocumentLibraryUrl(toSite.ProvisionedSite.ProvisionedSiteCollection.Name, toSite.Pin.ToString(), mergePinLibrary.CaseId.ToString())}",
                        Dictionary = dictionary,
                        InsertedDate = now,
                        InsertedBy = _userIdentity.Name,
                        UpdatedDate = now,
                        UpdatedBy = _userIdentity.Name
                    };
                    _libraryService.Delete(mergePinLibrary.Id);
                    _libraryService.Create(library);
                    Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> DatabaseSynchronisation. Completed. Average Execution: {0:0.000}s", databaseSynchronisationPrfMonTriggers.Stop());

                }
                if (!fromSite.ProvisionedSite.ProvisionedSiteCollection.Name.SafeEquals(toSite.ProvisionedSite.ProvisionedSiteCollection.Name))
                {
                    _contextService.Audit(context, task, string.Format(TaskResources.Audit_MergePin, fromSite.Pin, toSite.Pin));
                }
                Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> CreateDocumentLibraries. Completed. Average Execution: {0:0.000}s", createDocumentLibrariesPrfMonTriggers.Stop());
            }

            Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> MergePinSiteContent. Started.");
            var mergePinSiteContentPrfMonTriggers = new PrfMon();
            var fromPinSite = new PinSite(fromSite.Url, Credentials);
            var toPinSite = new PinSite(toSite.Url, Credentials);

            foreach (var listSource in fromPinSite.Lists)
            {
                var listDestination = toPinSite.GetList(listSource, pinSiteDocumentLibraries);
                var rootFolderSource = listSource.GetRootFolder(fromPinSite.Context);
                var rootFolderDestination = listDestination.GetRootFolder(toPinSite.Context);
                var listName = GetListNameFromServerRelativeUrl(rootFolderDestination.ServerRelativeUrl);
                rootFolderSource.MoveFilesTo(rootFolderDestination, rootFolderDestination.ServerRelativeUrl, listName);
            }
            Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> MergePinSiteContent. Completed. Average Execution: {0:0.000}s", mergePinSiteContentPrfMonTriggers.Stop());

            DeletePin(new Task { Handler = TaskHandlerNames.OperationsHandler, Name = TaskNames.DeletePin, Pin = fromSite.Pin });

            Debug.WriteLine("Fujitsu.AFC.Services.PinService.cs -> Completed Processing MergePin. Duration: {0:0.000}s", prfMonMethod.Stop());
        }
    }
}
