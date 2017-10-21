using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Core.Interfaces;
using Fujitsu.AFC.Core.Metrics;
using Fujitsu.AFC.Data.Interfaces;
using Fujitsu.AFC.Extensions;
using Fujitsu.AFC.Extensions.CSOM;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Services.Common;
using Fujitsu.AFC.Services.Interfaces;
using Fujitsu.AFC.Tasks.Resources;
using Fujitsu.Exceptions.Framework;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Site = Fujitsu.AFC.Model.Site;
using Task = Fujitsu.AFC.Model.Task;

namespace Fujitsu.AFC.Services
{
    public class CaseService : SharePointOnline, ICaseService
    {
        private readonly IParameterService _parameterService;
        private readonly IContextService _contextService;
        private readonly ILibraryService _libraryService;
        private readonly IRepository<Site> _siteRepository;
        private readonly IRepository<Library> _libraryRepository;
        private readonly IUserIdentity _userIdentity;
        private readonly IUnitOfWork _unitOfWork;

        public CaseService(IParameterService parameterService,
            IContextService contextService,
            ILibraryService libraryService,
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
            if (libraryService == null)
            {
                throw new ArgumentNullException(nameof(libraryService));
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
            _libraryService = libraryService;
            _siteRepository = siteRepository;
            _libraryRepository = libraryRepository;
            _userIdentity = userIdentity;
            _unitOfWork = unitOfWork;
        }

        [ExcludeFromCodeCoverage]
        public int AllocateCase(Task task)
        {
            Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> Processing AllocateCase.");
            var prfMonMethod = new PrfMon();

            var site = _siteRepository.Single(x => x.Pin == task.Pin.Value);

            using (var context = new ClientContext(SiteCollectionUrl(site.ProvisionedSite.ProvisionedSiteCollection.Name)))
            {
                var web = _contextService.Load(context, Credentials);

                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> CheckRequiredSecurityGroupsExist. Started.");
                var obtainRequiredUsersPrfMonTriggers = new PrfMon();
                var spSite = web.Webs.Single(x => x.Url == site.Url);
                var colGroup = spSite.SiteGroups;
                context.Load(colGroup);
                var globalSystemsAdminUser = web.EnsureUser(_parameterService.GetParameterByNameAndCache<string>(ParameterNames.ActiveDirectoryGlobalSystemsAdministratorGroup));
                var activeDirectoryContributeGroup = _parameterService.GetParameterByNameAndCache<string>(ParameterNames.ActiveDirectoryContributeGroup);
                var activeDirectoryReadGroup = _parameterService.GetParameterByNameAndCache<string>(ParameterNames.ActiveDirectoryReadGroup);
                var projectContributorsGroup = web.GetActiveDirectoryGroup(activeDirectoryContributeGroup, site.RestrictedUser, site.Pin, task.ProjectId.Value);
                var projectReadGroup = web.GetActiveDirectoryGroup(activeDirectoryReadGroup, site.RestrictedUser, site.Pin, task.ProjectId.Value);
                var spPinVisitorsGroup = colGroup.GetByName(string.Format(SharePointPinGroupNames.PinVisitors, site.Pin));
                context.Load(spSite, s => s.Lists.Include(l => l.Title));
                context.ExecuteQuery();
                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> CheckRequiredSecurityGroupsExist. Completed. Average Execution: {0:0.000}s", obtainRequiredUsersPrfMonTriggers.Stop());

                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> CreateDocumentLibrary. Started.");
                var createDocumentLibraryPrfMonTriggers = new PrfMon();
                var documentLibraryList = spSite.CreateDocumentLibrary(task.CaseId.Value.ToString());
                context.Load(documentLibraryList, d => d.ContentTypes);
                context.ExecuteQuery();
                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> CreateDocumentLibrary. Completed. Average Execution: {0:0.000}s", createDocumentLibraryPrfMonTriggers.Stop());

                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> UpdateDocumentLibraryList. Started.");
                var updateDocumentLibraryPrfMonTriggers = new PrfMon();
                documentLibraryList.UpdateDocumentLibraryList(web.ContentTypes, ContentTypeGroupNames.CaseDocumentContentTypes, task.CaseTitle);
                context.Load(documentLibraryList, d => d.Id, d => d.Fields);
                var folders = _parameterService.GetParameterByNameAndCache<string>(ParameterNames.CaseLibraryFolderNames);
                documentLibraryList.AddFolders(folders);
                context.ExecuteQuery();
                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> UpdateDocumentLibraryList. Completed. Average Execution: {0:0.000}s", updateDocumentLibraryPrfMonTriggers.Stop());

                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> UpdateDocumentLibraryMetaData. Started.");
                var documentLibraryMetaDataPrfMonTriggers = new PrfMon();
                var dictionary = context.CreateDocumentLibraryMetaData(documentLibraryList, spSite, task.Dictionary);
                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> UpdateDocumentLibraryMetaData. Completed. Average Execution: {0:0.000}s", documentLibraryMetaDataPrfMonTriggers.Stop());

                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> AssignPinMembershipPermissions. Started.");
                var assignPinMembershipPermissionsPrfMonTriggers = new PrfMon();
                if (task.IsPrimary.Value || !task.IsPrimary.Value && site.PrimaryProjectId != null && site.PrimaryProjectId == task.ProjectId.Value)
                {
                    var spPinMembersGroup = colGroup.GetByName(string.Format(SharePointPinGroupNames.PinMembers, site.Pin));
                    context.Load(spPinMembersGroup.Users, x => x.Include(u => u.Id, u => u.PrincipalType, u => u.Title));
                    context.ExecuteQuery();
                    var groupIds = spPinMembersGroup.Users.Where(u => u.PrincipalType == PrincipalType.SecurityGroup &&
                        u.Title.EndsWith("_CONTRIBUTE") || u.Title.EndsWith("_CONTRIBUTE_CLOSED") ||
                        u.Title.EndsWith($"_CONTRIBUTE_{site.Pin}") || u.Title.EndsWith($"_CONTRIBUTE_CLOSED_{site.Pin}")).Select(s => s.Id).ToList();
                    foreach (var id in groupIds)
                    {
                        spPinMembersGroup.Users.RemoveById(id);
                    }
                    if (task.IsPrimary.Value)
                    {
                        spPinMembersGroup.Users.AddUser(projectContributorsGroup);
                    }
                }
                context.Load(spPinVisitorsGroup);
                spPinVisitorsGroup.Users.AddUser(projectReadGroup);
                context.Load(documentLibraryList.RoleAssignments);
                context.ExecuteQuery();
                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> AssignPinMembershipPermissions. Completed. Average Execution: {0:0.000}s", assignPinMembershipPermissionsPrfMonTriggers.Stop());

                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> SetupDocumentLibraryMembershipPermissions. Started.");
                var setupDocumentLibraryMembershipPermissionsPrfMonTriggers = new PrfMon();
                documentLibraryList.BreakRoleInheritanceAndRemoveRoles(true, false, true);
                documentLibraryList.AddRoleWithPermissions(context, web, globalSystemsAdminUser, RoleType.Contributor);
                documentLibraryList.AddRoleWithPermissions(context, web, projectContributorsGroup, RoleType.Contributor);
                documentLibraryList.AddRoleWithPermissions(context, web, spPinVisitorsGroup, RoleType.Reader);
                documentLibraryList.Update();
                context.ExecuteQuery();
                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> SetupDocumentLibraryMembershipPermissions. Completed. Average Execution: {0:0.000}s", setupDocumentLibraryMembershipPermissionsPrfMonTriggers.Stop());

                _contextService.Audit(context, task, string.Format(TaskResources.Audit_AllocateCase, site.Pin, task.CaseId.Value, task.CaseTitle));

                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> DatabaseSynchronisation. Started.");
                var databaseSynchronisationPrfMonTriggers = new PrfMon();
                var now = DateTime.Now;
                var library = new Library
                {
                    CaseId = task.CaseId.Value,
                    Title = task.CaseTitle,
                    ProjectId = task.ProjectId.Value,
                    SiteId = site.Id,
                    ListId = documentLibraryList.Id,
                    IsClosed = false,
                    Url = $"{CaseDocumentLibraryUrl(site.ProvisionedSite.ProvisionedSiteCollection.Name, site.Pin.ToString(), task.CaseId.Value.ToString())}",
                    Dictionary = dictionary,
                    InsertedDate = now,
                    InsertedBy = _userIdentity.Name,
                    UpdatedDate = now,
                    UpdatedBy = _userIdentity.Name
                };
                RetryableOperation.Invoke(ExceptionPolicies.General,
                    () =>
                    {
                        if (task.IsPrimary.Value && (site.PrimaryProjectId == null || site.PrimaryProjectId.Value != task.ProjectId.Value))
                        {
                            site.PrimaryProjectId = task.ProjectId;
                            site.UpdatedDate = now;
                            site.UpdatedBy = _userIdentity.Name;
                            _siteRepository.Update(site);
                        }
                        else if (!task.IsPrimary.Value && site.PrimaryProjectId != null && site.PrimaryProjectId == task.ProjectId.Value)
                        {
                            site.PrimaryProjectId = null;
                            site.UpdatedDate = now;
                            site.UpdatedBy = _userIdentity.Name;
                            _siteRepository.Update(site);
                        }

                        _libraryRepository.Insert(library);
                        _unitOfWork.Save();
                    });
                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> DatabaseSynchronisation. Completed. Average Execution: {0:0.000}s", databaseSynchronisationPrfMonTriggers.Stop());

            }

            Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> Completed Processing AllocateCase. Duration: {0:0.000}s", prfMonMethod.Stop());
            return site.Id;
        }

        [ExcludeFromCodeCoverage]
        public int UpdateCaseTitle(Task task)
        {
            Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> Processing UpdateCaseTitle.");
            var prfMonMethod = new PrfMon();

            var library = _libraryRepository.Single(x => x.CaseId == task.CaseId.Value);

            using (var context = new ClientContext(SiteCollectionUrl(library.Site.ProvisionedSite.ProvisionedSiteCollection.Name)))
            {
                var web = _contextService.Load(context, Credentials);

                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> LoadDocumentLibraryList. Started.");
                var createDocumentLibraryPrfMonTriggers = new PrfMon();
                var spSite = web.Webs.Single(x => x.Url == library.Site.Url);
                var documentLibraryList = spSite.Lists.GetById(library.ListId);
                context.Load(documentLibraryList);
                context.ExecuteQuery();
                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> LoadDocumentLibraryList. Completed. Average Execution: {0:0.000}s", createDocumentLibraryPrfMonTriggers.Stop());

                if (!documentLibraryList.Title.SafeEquals(task.CaseTitle))
                {
                    Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> UpdateDocumentLibraryTitleAndAuditOperation. Started.");
                    var updateDocumentLibraryTitleAndAuditOperationPrfMonTriggers = new PrfMon();
                    documentLibraryList.Title = task.CaseTitle;
                    documentLibraryList.Update();
                    context.ExecuteQuery();
                    _contextService.Audit(context, task, string.Format(TaskResources.Audit_UpdateCaseTitle, library.Site.Pin, library.CaseId, library.Title, task.CaseTitle));
                    Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> UpdateDocumentLibraryTitleAndAuditOperation. Completed. Average Execution: {0:0.000}s", updateDocumentLibraryTitleAndAuditOperationPrfMonTriggers.Stop());
                }

                if (!library.Title.SafeEquals(task.CaseTitle))
                {
                    Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> DatabaseSynchronisation. Started.");
                    var databaseSynchronisationPrfMonTriggers = new PrfMon();
                    library.Title = task.CaseTitle;
                    library.UpdatedDate = DateTime.Now;
                    library.UpdatedBy = _userIdentity.Name;
                    _libraryService.Update(library);
                    Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> DatabaseSynchronisation. Completed. Average Execution: {0:0.000}s", databaseSynchronisationPrfMonTriggers.Stop());
                }
            }

            Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> Completed Processing UpdateCaseTitle. Duration: {0:0.000}s", prfMonMethod.Stop());
            return library.Site.Id;
        }

        [ExcludeFromCodeCoverage]
        public void UpdateCaseTitleByProject(Task task)
        {
            Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> Processing UpdateCaseTitleByProject.");
            var prfMonMethod = new PrfMon();

            var siteCollectionLibraries = _libraryService.GetSiteCollectionLibraryDictionary(task);

            if (siteCollectionLibraries.Any())
            {
                foreach (var siteCollection in siteCollectionLibraries)
                {
                    using (var context = new ClientContext(SiteCollectionUrl(siteCollection.Key)))
                    {
                        var web = _contextService.Load(context, Credentials);

                        foreach (var library in siteCollection.Value)
                        {
                            Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> LoadDocumentLibraryListAndUpdateMetaData. Started.");
                            var documentLibraryMetaDataPrfMonTriggers = new PrfMon();
                            var spSite = web.Webs.Single(x => x.Url == library.Site.Url);
                            var documentLibraryList = spSite.Lists.GetById(library.ListId);
                            context.Load(documentLibraryList, d => d.Id, d => d.Title, d => d.ContentTypes, d => d.Fields);
                            context.ExecuteQuery();
                            var dictionary = context.UpdateDocumentLibraryMetaData(documentLibraryList, spSite, library.Dictionary, task.Dictionary);
                            Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> LoadDocumentLibraryList. Completed. Average Execution: {0:0.000}s", documentLibraryMetaDataPrfMonTriggers.Stop());

                            Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs ->  UpdateDocumentLibraryListTitleAndAuditOperation. Started.");
                            var updateDocumentLibraryListTitleAndAuditOperationPrfMonTriggers = new PrfMon();
                            documentLibraryList.Title = documentLibraryList.Title.UpdateTitle(task.ProjectName);
                            documentLibraryList.Update();
                            context.ExecuteQuery();
                            Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> UpdateDocumentLibraryListTitleAndAuditOperation. Completed. Average Execution: {0:0.000}s", updateDocumentLibraryListTitleAndAuditOperationPrfMonTriggers.Stop());

                            _contextService.Audit(context, task, string.Format(TaskResources.Audit_UpdateCaseTitleByProject, task.ProjectId, library.Site.Pin, library.CaseId, library.Title, documentLibraryList.Title));

                            Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> DatabaseSynchronisation. Started.");
                            var databaseSynchronisationPrfMonTriggers = new PrfMon();
                            library.Title = task.ProjectName;
                            library.Dictionary = dictionary;
                            library.UpdatedDate = DateTime.Now;
                            library.UpdatedBy = _userIdentity.Name;
                            _libraryService.Update(library);
                            Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> DatabaseSynchronisation. Completed. Average Execution: {0:0.000}s", databaseSynchronisationPrfMonTriggers.Stop());

                        }
                    }
                }
            }

            Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> Completed Processing UpdateCaseTitleByProject. Duration: {0:0.000}s", prfMonMethod.Stop());

        }

        [ExcludeFromCodeCoverage]
        public int MoveCase(Task task)
        {
            Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> Processing MoveCase.");
            var prfMonMethod = new PrfMon();

            var library = _libraryRepository.Single(x => x.CaseId == task.CaseId.Value);

            using (var context = new ClientContext(SiteCollectionUrl(library.Site.ProvisionedSite.ProvisionedSiteCollection.Name)))
            {
                var web = _contextService.Load(context, Credentials);

                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> Permissions. Started.");
                var permissionsPrfMonTriggers = new PrfMon();
                var spSite = web.Webs.Single(x => x.Url == library.Site.Url);
                var colGroup = spSite.SiteGroups;
                context.Load(colGroup);
                var spPinMembersGroup = colGroup.GetByName(string.Format(SharePointPinGroupNames.PinMembers, library.Site.Pin));
                var spPinVisitorsGroup = colGroup.GetByName(string.Format(SharePointPinGroupNames.PinVisitors, library.Site.Pin));
                context.Load(spPinMembersGroup.Users, x => x.Include(u => u.Id, u => u.PrincipalType, u => u.Title));
                context.Load(spPinVisitorsGroup.Users, x => x.Include(u => u.Id, u => u.PrincipalType, u => u.Title));
                context.ExecuteQuery();
                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> Permissions. Completed. Average Execution: {0:0.000}s", permissionsPrfMonTriggers.Stop());

                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> CheckRequiredSecurityGroupsExist. Started.");
                var obtainRequiredUsersPrfMonTriggers = new PrfMon();
                var activeDirectoryContributeGroup = _parameterService.GetParameterByNameAndCache<string>(ParameterNames.ActiveDirectoryContributeGroup);
                var activeDirectoryReadGroup = _parameterService.GetParameterByNameAndCache<string>(ParameterNames.ActiveDirectoryReadGroup);
                var currentProjectContributorsGroup = web.GetActiveDirectoryGroup(activeDirectoryContributeGroup, library.Site.RestrictedUser, task.Pin.Value, task.CurrentProjectId.Value);
                var currentProjectReadGroup = web.GetActiveDirectoryGroup(activeDirectoryReadGroup, library.Site.RestrictedUser, task.Pin.Value, task.CurrentProjectId.Value);
                var newProjectContributorsGroup = web.GetActiveDirectoryGroup(activeDirectoryContributeGroup, library.Site.RestrictedUser, task.Pin.Value, task.NewProjectId.Value);
                var newProjectReadGroup = web.GetActiveDirectoryGroup(activeDirectoryReadGroup, library.Site.RestrictedUser, task.Pin.Value, task.NewProjectId.Value);
                context.ExecuteQuery();
                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> CheckRequiredSecurityGroupsExist. Completed. Average Execution: {0:0.000}s", obtainRequiredUsersPrfMonTriggers.Stop());

                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> LoadDocumentLibraryList. Started.");
                var loadDocumentLibraryListPrfMonTriggers = new PrfMon();
                var documentLibraryList = spSite.Lists.GetById(library.ListId);
                context.Load(documentLibraryList);
                context.Load(documentLibraryList.RoleAssignments, r => r.Include(m => m.Member.Id));
                context.Load(currentProjectContributorsGroup, r => r.Id, r => r.Title);
                context.Load(newProjectContributorsGroup);
                context.Load(currentProjectReadGroup, r => r.Title);
                context.Load(newProjectReadGroup);
                context.ExecuteQuery();
                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> LoadDocumentLibraryList. Completed. Average Execution: {0:0.000}s", loadDocumentLibraryListPrfMonTriggers.Stop());

                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> DocumentLibraryMembershipPermissions. Started.");
                var documentLibraryMembershipPermissionsPrfMonTriggers = new PrfMon();
                documentLibraryList.RoleAssignments.Where(r => r.Member.Id == currentProjectContributorsGroup.Id).ToList().ForEach(ra => ra.DeleteObject());
                documentLibraryList.AddRoleWithPermissions(context, web, newProjectContributorsGroup, RoleType.Contributor);
                documentLibraryList.Update();
                context.ExecuteQuery();
                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> DocumentLibraryMembershipPermissions. Completed. Average Execution: {0:0.000}s", documentLibraryMembershipPermissionsPrfMonTriggers.Stop());

                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> PinSitePermissions. Started.");
                var pinSitePermissionsPrfMonTriggers = new PrfMon();
                if (library.Site.PrimaryProjectId != null && library.Site.PrimaryProjectId.Value == task.CurrentProjectId.Value)
                {
                    spPinMembersGroup.Users.RemoveById(spPinMembersGroup.Users.Single(u => u.Title == currentProjectContributorsGroup.Title).Id);
                    spPinMembersGroup.Users.AddUser(newProjectContributorsGroup);
                }

                if (_libraryRepository.Query(x => x.SiteId == library.SiteId && x.ProjectId == task.CurrentProjectId.Value).Count() == 1)
                {
                    spPinVisitorsGroup.Users.RemoveById(spPinVisitorsGroup.Users.Single(u => u.Title == currentProjectReadGroup.Title).Id);
                }
                spPinVisitorsGroup.Users.AddUser(newProjectReadGroup);
                context.Load(documentLibraryList);
                context.Load(documentLibraryList.Fields);
                context.ExecuteQuery();
                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> PinSitePermissions. Completed. Average Execution: {0:0.000}s", pinSitePermissionsPrfMonTriggers.Stop());

                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> UpdateNewProjectDocumentLibraryMetaData. Started.");
                var documentLibraryMetaDataPrfMonTriggers = new PrfMon();
                var dictionary = context.UpdateDocumentLibraryMetaData(documentLibraryList, spSite, library.Dictionary, task.Dictionary);
                context.ExecuteQuery();
                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> UpdateNewProjectDocumentLibraryMetaData. Completed. Average Execution: {0:0.000}s", documentLibraryMetaDataPrfMonTriggers.Stop());

                _contextService.Audit(context, task, string.Format(TaskResources.Audit_MoveCase, task.Pin.Value, task.CaseId.Value, task.CurrentProjectId.Value, task.NewProjectId.Value));

                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> DatabaseSynchronisation. Started.");
                var databaseSynchronisationPrfMonTriggers = new PrfMon();
                var now = DateTime.Now;
                RetryableOperation.Invoke(ExceptionPolicies.General,
                    () =>
                    {
                        if (library.Site.PrimaryProjectId != null && library.Site.PrimaryProjectId.Value == task.CurrentProjectId.Value)
                        {
                            var site = _siteRepository.Single(x => x.Id == library.SiteId);
                            site.PrimaryProjectId = task.NewProjectId;
                            site.UpdatedBy = _userIdentity.Name;
                            site.UpdatedDate = now;
                            _siteRepository.Update(site);
                        }

                        library.ProjectId = task.NewProjectId.Value;
                        library.Dictionary = dictionary;
                        library.InsertedDate = now;
                        library.InsertedBy = _userIdentity.Name;
                        library.UpdatedDate = now;
                        library.UpdatedBy = _userIdentity.Name;
                        _libraryRepository.Update(library);
                        _unitOfWork.Save();
                    });
                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> DatabaseSynchronisation. Completed. Average Execution: {0:0.000}s", databaseSynchronisationPrfMonTriggers.Stop());

            }

            Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> Completed Processing MoveCase. Duration: {0:0.000}s", prfMonMethod.Stop());
            return library.SiteId;
        }

        [ExcludeFromCodeCoverage]
        public int CloseCase(Task task)
        {
            Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> Processing CloseCase.");
            var prfMonMethod = new PrfMon();

            var library = _libraryRepository.Single(x => x.CaseId == task.CaseId.Value);

            using (var context = new ClientContext(SiteCollectionUrl(library.Site.ProvisionedSite.ProvisionedSiteCollection.Name)))
            {
                var web = _contextService.Load(context, Credentials);

                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> CheckRequiredSecurityGroupsExist. Started.");
                var obtainRequiredUsersPrfMonTriggers = new PrfMon();
                var activeDirectoryContributeClosedGroup = _parameterService.GetParameterByNameAndCache<string>(ParameterNames.ActiveDirectoryContributeClosedGroup);
                var activeDirectoryContributeGroup = _parameterService.GetParameterByNameAndCache<string>(ParameterNames.ActiveDirectoryContributeGroup);
                var projectContributorsClosedGroup = web.GetActiveDirectoryGroup(activeDirectoryContributeClosedGroup, library.Site.RestrictedUser, library.Site.Pin, library.ProjectId);
                var projectContributorsGroup = web.GetActiveDirectoryGroup(activeDirectoryContributeGroup, library.Site.RestrictedUser, library.Site.Pin, library.ProjectId);
                context.ExecuteQuery();
                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> CheckRequiredSecurityGroupsExist. Completed. Average Execution: {0:0.000}s", obtainRequiredUsersPrfMonTriggers.Stop());

                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> UpdateDocumentLibraryMetaData. Started.");
                var documentLibraryMetaDataPrfMonTriggers = new PrfMon();
                var spSite = web.Webs.Single(x => x.Url == library.Site.Url);
                var colGroup = spSite.SiteGroups;
                context.Load(colGroup);
                var documentLibraryList = spSite.Lists.GetById(library.ListId);
                context.Load(documentLibraryList, d => d.Id, d => d.Fields);
                context.ExecuteQuery();
                var dictionary = context.UpdateDocumentLibraryMetaData(documentLibraryList, spSite, library.Dictionary, task.Dictionary);
                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> UpdateDocumentLibraryMetaData. Completed. Average Execution: {0:0.000}s", documentLibraryMetaDataPrfMonTriggers.Stop());

                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> AssignPinMembershipPermissions. Started.");
                var assignPinMembershipPermissionsPrfMonTriggers = new PrfMon();
                var spPinMembersGroup = colGroup.GetByName(string.Format(SharePointPinGroupNames.PinMembers, library.Site.Pin));
                context.Load(spPinMembersGroup.Users, x => x.Include(u => u.Id, u => u.PrincipalType, u => u.Title));
                context.Load(projectContributorsGroup, u => u.Title);
                context.ExecuteQuery();
                if (library.Site.PrimaryProjectId != null && library.Site.PrimaryProjectId.Value == library.ProjectId && _libraryRepository.Query(x => x.SiteId == library.SiteId && x.ProjectId == library.ProjectId && x.IsClosed == false).Count() == 1)
                {
                    spPinMembersGroup.Users.RemoveById(spPinMembersGroup.Users.Single(u => u.Title == projectContributorsGroup.Title).Id);
                    spPinMembersGroup.Users.AddUser(projectContributorsClosedGroup);
                    context.ExecuteQuery();
                }
                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> AssignPinMembershipPermissions. Completed. Average Execution: {0:0.000}s", assignPinMembershipPermissionsPrfMonTriggers.Stop());

                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> DocumentLibraryMembershipPermissions. Started.");
                var documentLibraryMembershipPermissionsPrfMonTriggers = new PrfMon();
                context.Load(documentLibraryList);
                context.Load(documentLibraryList.RoleAssignments, r => r.Include(m => m.Member.Id));
                context.Load(projectContributorsGroup, r => r.Id);
                context.ExecuteQuery();
                documentLibraryList.RoleAssignments.Where(r => r.Member.Id == projectContributorsGroup.Id).ToList().ForEach(ra => ra.DeleteObject());
                documentLibraryList.AddRoleWithPermissions(context, web, projectContributorsClosedGroup, RoleType.Contributor);
                documentLibraryList.Update();
                context.ExecuteQuery();
                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> DocumentLibraryMembershipPermissions. Completed. Average Execution: {0:0.000}s", documentLibraryMembershipPermissionsPrfMonTriggers.Stop());

                _contextService.Audit(context, task, string.Format(TaskResources.Audit_CloseCase, library.Site.Pin, library.CaseId, library.Title));

                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> DatabaseSynchronisation. Started.");
                var databaseSynchronisationPrfMonTriggers = new PrfMon();
                library.IsClosed = true;
                library.Dictionary = dictionary;
                library.UpdatedDate = DateTime.Now;
                library.UpdatedBy = _userIdentity.Name;
                _libraryService.Update(library);
                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> DatabaseSynchronisation. Completed. Average Execution: {0:0.000}s", databaseSynchronisationPrfMonTriggers.Stop());
            }

            Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> Completed Processing UpdateCaseTitle. Duration: {0:0.000}s", prfMonMethod.Stop());
            return library.SiteId;
        }

        [ExcludeFromCodeCoverage]
        public void DeleteCase(Task task)
        {
            Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> Processing DeleteCase.");
            var prfMonMethod = new PrfMon();

            var library = _libraryRepository.Single(x => x.CaseId == task.CaseId.Value);

            using (var context = new ClientContext(SiteCollectionUrl(library.Site.ProvisionedSite.ProvisionedSiteCollection.Name)))
            {
                var web = _contextService.Load(context, Credentials);

                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> DeleteDocumentLibrary. Started.");
                var deleteDocumentLibraryPrfMonTriggers = new PrfMon();
                var spSite = web.Webs.Single(x => x.Url == library.Site.Url);
                var documentLibraryList = spSite.Lists.GetById(library.ListId);
                context.Load(documentLibraryList);
                documentLibraryList.DeleteObject();
                context.ExecuteQuery();
                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> DeleteDocumentLibrary. Completed. Average Execution: {0:0.000}s", deleteDocumentLibraryPrfMonTriggers.Stop());

                _contextService.Audit(context, task, string.Format(TaskResources.Audit_DeleteCase, library.Site.Pin, library.CaseId, library.Title));

                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> DatabaseSynchronisation. Started.");
                var databaseSynchronisationPrfMonTriggers = new PrfMon();
                _libraryService.Delete(library);
                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> DatabaseSynchronisation. Completed. Average Execution: {0:0.000}s", databaseSynchronisationPrfMonTriggers.Stop());

            }

            Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> Completed Processing DeleteCase. Duration: {0:0.000}s", prfMonMethod.Stop());
        }

        [ExcludeFromCodeCoverage]
        public int ArchiveCase(Task task)
        {
            Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> Processing ArchiveCase.");
            var prfMonMethod = new PrfMon();

            var library = _libraryRepository.Single(x => x.CaseId == task.CaseId.Value);

            using (var context = new ClientContext(SiteCollectionUrl(library.Site.ProvisionedSite.ProvisionedSiteCollection.Name)))
            {
                _contextService.Load(context, Credentials);
                _contextService.Audit(context, task, string.Format(TaskResources.Audit_ArchiveCase, library.Site.Pin, library.CaseId, library.Title));
            }

            Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> Completed Processing UpdateCaseTitle. Duration: {0:0.000}s", prfMonMethod.Stop());
            return library.SiteId;
        }

        [ExcludeFromCodeCoverage]
        public int UpdateCaseWithDictionaryValues(Task task)
        {
            Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> Processing UpdateCaseWithDictionaryValues.");
            var prfMonMethod = new PrfMon();

            var library = _libraryRepository.Single(x => x.CaseId == task.CaseId.Value);

            using (var context = new ClientContext(SiteCollectionUrl(library.Site.ProvisionedSite.ProvisionedSiteCollection.Name)))
            {
                var web = _contextService.Load(context, Credentials);

                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> UpdateDocumentLibraryMetaData. Started.");
                var documentLibraryMetaDataPrfMonTriggers = new PrfMon();
                var spSite = web.Webs.Single(x => x.Url == library.Site.Url);
                var documentLibraryList = spSite.Lists.GetById(library.ListId);
                context.Load(documentLibraryList, d => d.Id, d => d.Fields);
                web.AddAuditListItem(SharePointListNames.Audit, task, string.Format(TaskResources.Audit_UpdateCaseWithDictionaryValues, library.Site.Pin, library.CaseId));
                context.ExecuteQuery();
                var dictionary = context.UpdateDocumentLibraryMetaData(documentLibraryList, spSite, library.Dictionary, task.Dictionary);
                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> UpdateDocumentLibraryMetaData. Completed. Average Execution: {0:0.000}s", documentLibraryMetaDataPrfMonTriggers.Stop());

                _contextService.Audit(context, task, string.Format(TaskResources.Audit_ArchiveCase, library.Site.Pin, library.CaseId, library.Title));

                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> DatabaseSynchronisation. Started.");
                var databaseSynchronisationPrfMonTriggers = new PrfMon();
                library.Dictionary = dictionary;
                library.UpdatedDate = DateTime.Now;
                library.UpdatedBy = _userIdentity.Name;
                _libraryService.Update(library);
                Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> DatabaseSynchronisation. Completed. Average Execution: {0:0.000}s", databaseSynchronisationPrfMonTriggers.Stop());
            }

            Debug.WriteLine("Fujitsu.AFC.Services.CaseService.cs -> Completed Processing UpdateCaseWithDictionaryValues. Duration: {0:0.000}s", prfMonMethod.Stop());
            return library.SiteId;
        }
    }
}
