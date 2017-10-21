using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Core.Interfaces;
using Fujitsu.AFC.Core.Metrics;
using Fujitsu.AFC.Data.Interfaces;
using Fujitsu.AFC.Extensions.CSOM;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Services.Common;
using Fujitsu.AFC.Services.Interfaces;
using Fujitsu.AFC.Tasks.Resources;
using Fujitsu.Exceptions.Framework;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using Site = Fujitsu.AFC.Model.Site;


namespace Fujitsu.AFC.Services
{
    public class UserService : SharePointOnline, IUserService
    {
        private readonly IParameterService _parameterService;
        private readonly IContextService _contextService;
        private readonly IRepository<Site> _siteRepository;
        private readonly IRepository<Library> _libraryRepository;
        private readonly IUserIdentity _userIdentity;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IParameterService parameterService,
            IContextService contextService,
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
            _siteRepository = siteRepository;
            _libraryRepository = libraryRepository;
            _userIdentity = userIdentity;
            _unitOfWork = unitOfWork;
        }

        [ExcludeFromCodeCoverage]
        public void RestictUser(Task task)
        {
            Debug.WriteLine("Fujitsu.AFC.Services.UserService.cs -> Processing RestictUser.");
            var prfMonMethod = new PrfMon();

            var site = _siteRepository.Single(x => x.Pin == task.Pin.Value);

            using (var context = new ClientContext(SiteCollectionUrl(site.ProvisionedSite.ProvisionedSiteCollection.Name)))
            {
                var web = _contextService.Load(context, Credentials);

                Debug.WriteLine("Fujitsu.AFC.Services.UserService.cs -> PinSitePermissionsUserChecking. Started.");
                var pinSitePermissionsPrfMonTriggers = new PrfMon();
                var spSite = web.Webs.Single(x => x.Url == site.Url);
                var colGroup = spSite.SiteGroups;
                context.Load(colGroup);
                context.ExecuteQuery();
                var spPinMembersGroup = colGroup.GetByName(string.Format(SharePointPinGroupNames.PinMembers, task.Pin.Value));
                var spPinVisitorsGroup = colGroup.GetByName(string.Format(SharePointPinGroupNames.PinVisitors, task.Pin.Value));
                context.Load(spPinMembersGroup.Users, x => x.Include(u => u.Id, u => u.PrincipalType, u => u.Title));
                context.Load(spPinVisitorsGroup.Users, x => x.Include(u => u.Id, u => u.PrincipalType, u => u.Title));
                context.ExecuteQuery();
                var contributorUsers = spPinMembersGroup.Users.Where(u => u.PrincipalType == PrincipalType.SecurityGroup && u.Title.EndsWith("_CONTRIBUTE") || u.Title.EndsWith("_CONTRIBUTE_CLOSED")).ToList();
                context.RestrictPinGroupPermissions(contributorUsers, spPinMembersGroup, task.Pin.Value.ToString(), false);
                var visitorUsers = spPinVisitorsGroup.Users.Where(u => u.PrincipalType == PrincipalType.SecurityGroup && u.Title.EndsWith("_READ")).ToList();
                context.RestrictPinGroupPermissions(visitorUsers, spPinVisitorsGroup, task.Pin.Value.ToString(), false);
                context.ExecuteQuery();
                Debug.WriteLine("Fujitsu.AFC.Services.UserService.cs -> PinSitePermissionsUserChecking. Completed. Average Execution: {0:0.000}s", pinSitePermissionsPrfMonTriggers.Stop());

                Debug.WriteLine("Fujitsu.AFC.Services.UserService.cs -> CaseDocumentLibraryPermissionsUserChecking. Started.");
                var caseDocumentLibraryPermissionsPrfMonTriggers = new PrfMon();
                var libraries = _libraryRepository.Query(x => x.SiteId == site.Id).ToList();
                var contributorClosedGroupName = _parameterService.GetParameterByNameAndCache<string>(ParameterNames.ActiveDirectoryContributeClosedGroup);
                var contributorGroupName = _parameterService.GetParameterByNameAndCache<string>(ParameterNames.ActiveDirectoryContributeGroup);
                context.RestrictDocumentLibraryContributePermissions(web, spSite, libraries, task.Pin.Value, contributorGroupName, contributorClosedGroupName, false);
                Debug.WriteLine("Fujitsu.AFC.Services.UserService.cs -> CaseDocumentLibraryPermissionsUserChecking. Completed. Average Execution: {0:0.000}s", caseDocumentLibraryPermissionsPrfMonTriggers.Stop());

                Debug.WriteLine("Fujitsu.AFC.Services.UserService.cs -> RestrictUser. Started.");
                var restrictUserPrfMonTriggers = new PrfMon();
                context.RestrictPinGroupPermissions(contributorUsers, spPinMembersGroup, task.Pin.Value.ToString(), true);
                context.RestrictPinGroupPermissions(visitorUsers, spPinVisitorsGroup, task.Pin.Value.ToString(), true);
                context.RestrictDocumentLibraryContributePermissions(web, spSite, libraries, task.Pin.Value, contributorGroupName, contributorClosedGroupName, true);
                Debug.WriteLine("Fujitsu.AFC.Services.UserService.cs -> RestrictUser. Completed. Average Execution: {0:0.000}s", restrictUserPrfMonTriggers.Stop());

                _contextService.Audit(context, task, string.Format(TaskResources.Audit_RestrictUser, task.Pin.Value));

                Debug.WriteLine("Fujitsu.AFC.Services.UserService.cs -> DatabaseSynchronisation. Started.");
                var databaseSynchronisationPrfMonTriggers = new PrfMon();
                site.RestrictedUser = true;
                site.UpdatedDate = DateTime.Now;
                site.UpdatedBy = _userIdentity.Name;
                RetryableOperation.Invoke(ExceptionPolicies.General,
                    () =>
                    {
                        _siteRepository.Update(site);
                        _unitOfWork.Save();
                    });
                Debug.WriteLine("Fujitsu.AFC.Services.UserService.cs -> DatabaseSynchronisation. Completed. Average Execution: {0:0.000}s", databaseSynchronisationPrfMonTriggers.Stop());
            }

            Debug.WriteLine("Fujitsu.AFC.Services.UserService.cs -> Completed Processing RestictUser. Duration: {0:0.000}s", prfMonMethod.Stop());
        }

        [ExcludeFromCodeCoverage]
        public void RemoveRestrictedUser(Task task)
        {
            Debug.WriteLine("Fujitsu.AFC.Services.UserService.cs -> Processing RemoveRestictedUser.");
            var prfMonMethod = new PrfMon();

            var site = _siteRepository.Single(x => x.Pin == task.Pin.Value);

            using (var context = new ClientContext(SiteCollectionUrl(site.ProvisionedSite.ProvisionedSiteCollection.Name)))
            {
                var web = _contextService.Load(context, Credentials);

                Debug.WriteLine("Fujitsu.AFC.Services.UserService.cs -> PinSitePermissionsUserChecking. Started.");
                var pinSitePermissionsPrfMonTriggers = new PrfMon();
                var spSite = web.Webs.Single(x => x.Url == site.Url);
                var colGroup = spSite.SiteGroups;
                context.Load(colGroup);
                context.ExecuteQuery();
                var spPinMembersGroup = colGroup.GetByName(string.Format(SharePointPinGroupNames.PinMembers, task.Pin.Value));
                var spPinVisitorsGroup = colGroup.GetByName(string.Format(SharePointPinGroupNames.PinVisitors, task.Pin.Value));
                context.Load(spPinMembersGroup.Users, x => x.Include(u => u.Id, u => u.PrincipalType, u => u.Title));
                context.Load(spPinVisitorsGroup.Users, x => x.Include(u => u.Id, u => u.PrincipalType, u => u.Title));
                context.ExecuteQuery();
                var contributorUsers = spPinMembersGroup.Users.Where(u => u.PrincipalType == PrincipalType.SecurityGroup && u.Title.EndsWith($"_CONTRIBUTE_{task.Pin.Value}") || u.Title.EndsWith($"_CONTRIBUTE_CLOSED_{task.Pin.Value}")).ToList();
                context.RemoveRestrictedPinGroupPermissions(contributorUsers, spPinMembersGroup, task.Pin.Value.ToString(), false);
                var visitorUsers = spPinVisitorsGroup.Users.Where(u => u.PrincipalType == PrincipalType.SecurityGroup && u.Title.EndsWith($"_READ_{task.Pin.Value}")).ToList();
                context.RemoveRestrictedPinGroupPermissions(visitorUsers, spPinVisitorsGroup, task.Pin.Value.ToString(), false);
                context.ExecuteQuery();
                Debug.WriteLine("Fujitsu.AFC.Services.UserService.cs -> PinSitePermissionsUserChecking. Completed. Average Execution: {0:0.000}s", pinSitePermissionsPrfMonTriggers.Stop());

                Debug.WriteLine("Fujitsu.AFC.Services.UserService.cs -> CaseDocumentLibraryPermissionsUserChecking. Started.");
                var caseDocumentLibraryPermissionsPrfMonTriggers = new PrfMon();
                var libraries = _libraryRepository.Query(x => x.SiteId == site.Id).ToList();
                var contributorClosedGroupName = _parameterService.GetParameterByNameAndCache<string>(ParameterNames.ActiveDirectoryContributeClosedGroup);
                var contributorGroupName = _parameterService.GetParameterByNameAndCache<string>(ParameterNames.ActiveDirectoryContributeGroup);
                context.RemoveRestrictedDocumentLibraryContributePermissions(web, spSite, libraries, task.Pin.Value, contributorGroupName, contributorClosedGroupName, false);
                Debug.WriteLine("Fujitsu.AFC.Services.UserService.cs -> CaseDocumentLibraryPermissionsUserChecking. Completed. Average Execution: {0:0.000}s", caseDocumentLibraryPermissionsPrfMonTriggers.Stop());

                Debug.WriteLine("Fujitsu.AFC.Services.UserService.cs -> RemoveRestrictedUser. Started.");
                var removeRestrictedUserPrfMonTriggers = new PrfMon();
                context.RemoveRestrictedPinGroupPermissions(contributorUsers, spPinMembersGroup, task.Pin.Value.ToString(), true);
                context.RemoveRestrictedPinGroupPermissions(visitorUsers, spPinVisitorsGroup, task.Pin.Value.ToString(), true);
                context.RemoveRestrictedDocumentLibraryContributePermissions(web, spSite, libraries, task.Pin.Value, contributorGroupName, contributorClosedGroupName, true);
                Debug.WriteLine("Fujitsu.AFC.Services.UserService.cs -> RemoveRestrictedUser. Completed. Average Execution: {0:0.000}s", removeRestrictedUserPrfMonTriggers.Stop());

                _contextService.Audit(context, task, string.Format(TaskResources.Audit_RemoveRestrictedUser, task.Pin.Value));

                Debug.WriteLine("Fujitsu.AFC.Services.UserService.cs -> DatabaseSynchronisation. Started.");
                var databaseSynchronisationPrfMonTriggers = new PrfMon();
                site.RestrictedUser = false;
                site.UpdatedDate = DateTime.Now;
                site.UpdatedBy = _userIdentity.Name;
                RetryableOperation.Invoke(ExceptionPolicies.General,
                    () =>
                    {
                        _siteRepository.Update(site);
                        _unitOfWork.Save();
                    });
                Debug.WriteLine("Fujitsu.AFC.Services.UserService.cs -> DatabaseSynchronisation. Completed. Average Execution: {0:0.000}s", databaseSynchronisationPrfMonTriggers.Stop());
            }

            Debug.WriteLine("Fujitsu.AFC.Services.UserService.cs -> Completed Processing RemoveRestictedUser. Duration: {0:0.000}s", prfMonMethod.Stop());

        }
    }
}
