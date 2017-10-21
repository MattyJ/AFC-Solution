using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Model;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WebParts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Fujitsu.AFC.Extensions.CSOM
{
    [ExcludeFromCodeCoverage]
    public static class WebExtensions
    {
        public static void DeleteListFromWebByTitle(this Web value, string name)
        {
            var documentsList = value.Lists.GetByTitle(name);
            documentsList.DeleteObject();
        }

        public static List CreateDocumentLibrary(this Web value, string name)
        {
            var pinDocumentListInfo = new ListCreationInformation
            {
                Title = name,
                TemplateType = (int)ListTemplateType.DocumentLibrary
            };

            if (!value.Lists.Any(x => x.Title == name))
            {
                return value.Lists.Add(pinDocumentListInfo);
            }

            return value.Lists.Single(x => x.Title == name);
        }

        public static void AddAuditListItem(this Web value, string listName, Task task, string summary)
        {
            var list = value.Lists.GetByTitle(listName);
            var dictionary = new Dictionary<string, object>
            {
                {"TaskId", task.Id},
                {"Title", task.Name},
                {"Event", summary}
            };
            list.AddListItem(dictionary);
        }

        public static void AddEscalationListItem(this Web value, string listName, HistoryLog historyLog)
        {
            var list = value.Lists.GetByTitle(listName);
            var dictionary = new Dictionary<string, object>
            {
                { "Title" ,historyLog.Name },
                { "Comment", historyLog.EventDetail},  // This is in fact the Description field!
                { "Category",historyLog.Handler}
            };
            list.AddListItem(dictionary);
        }

        public static void InitialiseAllocatedSite(this Web value, ProvisionedSite provisionedSite, Task task, string serverRelativeUrl)
        {
            value.Title = task.SiteTitle;
            value.ServerRelativeUrl = serverRelativeUrl;
            value.DeleteListFromWebByTitle(SharePointListNames.SharedDocuments);
            value.Update();
        }

        public static void AddRoleWithPermissions(this Web value, ClientRuntimeContext context, string title, RoleType roleType)
        {
            var groupCreationInfo = new GroupCreationInformation
            {
                Title = title
            };
            var ownersGroup = value.SiteGroups.Add(groupCreationInfo);
            var ownersRoleDefinition = value.RoleDefinitions.GetByType(roleType);
            var ownersRoleDefinitionBindingColl = new RoleDefinitionBindingCollection(context)
            {
                ownersRoleDefinition
            };

            value.RoleAssignments.Add(ownersGroup, ownersRoleDefinitionBindingColl);
        }

        public static string GetWelcomePageUrl(this Web value, ClientContext context)
        {
            var webRootFolder = value.RootFolder;
            context.Load(webRootFolder);
            context.ExecuteQuery();
            return webRootFolder.WelcomePage;
        }

        public static void AddWebToPartToPage(this Web value, ClientContext context, string listWebViewPartXml, string pageUrl)
        {
            var page = value.GetFileByServerRelativeUrl(pageUrl);
            var wpm = page.GetLimitedWebPartManager(PersonalizationScope.Shared);
            var webPartXml = listWebViewPartXml;
            var wpdImported = wpm.ImportWebPart(webPartXml);
            var wpdAdded = wpm.AddWebPart(wpdImported.WebPart, "wpz", 0);
            context.Load(wpdAdded, w => w.Id);
            context.ExecuteQuery();

            var wpString = GetEmbeddedWpString(wpdAdded.Id);
            var item = page.ListItemAllFields;
            context.Load(item);
            context.ExecuteQuery();

            const string key = "WikiField";
            if (item[key] == null)
            {
                item[key] = wpString;
            }
            else
            {
                item[key] = $"{item[key]}{wpString}";
            }
            item.Update();
            context.ExecuteQuery();
        }

        public static void AddPageToNavigation(this Web value, ClientContext ctx, string pageUrl)
        {
            var quickLaunch = value.Navigation.QuickLaunch;
            var docIndex = new NavigationNodeCreationInformation
            {
                Title = "Document Index",
                Url = pageUrl,
                AsLastNode = true
            };
            quickLaunch.Add(docIndex);
            ctx.Load(quickLaunch);
            ctx.ExecuteQuery();
        }

        public static User GetActiveDirectoryGroup(this Web value, string activeDirectoryGroupName, bool restricted, int pin, int projectId)
        {
            var activeDirectorySecurityGroupName = restricted ? $"{string.Format(activeDirectoryGroupName, projectId)}_{pin}" : string.Format(activeDirectoryGroupName, projectId);
            return value.EnsureUser(activeDirectorySecurityGroupName);
        }

        public static User GetActiveDirectoryContributorGroup(this Web value, string contributorGroupName, string contributorClosedGroupName, bool restricted, bool closed, int pin, int projectId)
        {
            var activeDirectorySecurityGroupName = $"{string.Format(closed ? contributorClosedGroupName : contributorGroupName, projectId)}";
            return value.EnsureUser(restricted ? $"{activeDirectorySecurityGroupName}_{pin}" : activeDirectorySecurityGroupName);
        }

        private static string GetEmbeddedWpString(Guid wpGuid)
        {
            // set the web part's ID as part of the ID-s of the div elements
            var wpForm = @"<div class=""ms-rtestate-read ms-rte-wpbox"" contenteditable=""false"">
                                <div class=""ms-rtestate-notify ms-rtestate-read {0}"" id=""div_{0}""></div>
                                <div id=""vid_{0}"" style=""display:none""></div>
                             </div>";

            return string.Format(wpForm, wpGuid);
        }
    }
}
