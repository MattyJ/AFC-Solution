using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.SharePoint.Client;

namespace Fujitsu.AFC.Extensions.CSOM
{
    [ExcludeFromCodeCoverage]
    public static class ListExtensions
    {
        public static List CreateNumberField(this List value, string name)
        {
            value.Fields.AddFieldAsXml($"<Field DisplayName='{name}' Type='Number' />", true, AddFieldOptions.DefaultValue);

            return value;
        }

        public static List CreateTextField(this List value, string name)
        {
            value.Fields.AddFieldAsXml($"<Field DisplayName='{name}' Type='Text' />", true, AddFieldOptions.DefaultValue);

            return value;
        }

        public static void RemoveContentTypesFromList(this List value)
        {
            foreach (var contentType in value.ContentTypes)
            {
                contentType.DeleteObject();
            }
        }

        public static void AddContentTypesToList(this List value, ContentTypeCollection contentTypeCollection, string contentTypeGroupName)
        {
            var documentGroupContentTypes = contentTypeCollection.GetGroupContentTypesByGroupName(contentTypeGroupName);
            foreach (var documentContentType in documentGroupContentTypes)
            {
                value.ContentTypes.AddExistingContentType(documentContentType);
            }
        }

        public static List AddListItem(this List value, Dictionary<string, object> itemsDictionary)
        {
            var listItemCreationInformation = new ListItemCreationInformation();
            var listItem = value.AddItem(listItemCreationInformation);
            foreach (var item in itemsDictionary)
            {
                listItem[item.Key] = item.Value;
            }
            listItem.Update();

            return value;
        }

        public static void UpdateDocumentLibraryList(this List value, ContentTypeCollection contentTypes, string documentContentTypeGroupName, string title)
        {
            value.RemoveContentTypesFromList();
            value.AddContentTypesToList(contentTypes, documentContentTypeGroupName);
            value.Title = title;
            value.ContentTypesEnabled = true;
            value.EnableMinorVersions = true;
            value.MajorVersionLimit = 500;
            value.MajorWithMinorVersionsLimit = 500;
            value.OnQuickLaunch = true;
            value.Update();
        }

        public static void BreakRoleInheritanceAndRemoveRoles(this List value, bool copyRoleAssignments, bool clearSubscopes, bool removeRoleAssignments)
        {
            value.BreakRoleInheritance(copyRoleAssignments, clearSubscopes);
            if (removeRoleAssignments)
            {
                value.RoleAssignments.ToList().ForEach(ra => ra.DeleteObject());
            }
        }

        public static void AddRoleWithPermissions(this List value, ClientRuntimeContext context, Web web, Principal principal, RoleType roleType)
        {
            var ownersRoleDefinition = web.RoleDefinitions.GetByType(roleType);
            var ownersRoleDefinitionBindingColl = new RoleDefinitionBindingCollection(context)
                {
                    ownersRoleDefinition
                };
            value.RoleAssignments.Add(principal, ownersRoleDefinitionBindingColl);
        }

        public static void AddFolders(this List value, string folders)
        {
            if (string.IsNullOrEmpty(folders)) return;
            var number = 0;
            foreach (var folder in folders.Split(';'))
            {
                number++;
                var info = new ListItemCreationInformation
                {
                    UnderlyingObjectType = FileSystemObjectType.Folder,
                    LeafName = $"{number:00}. {folder}"
                };
                var newItem = value.AddItem(info);
                newItem["Title"] = info.LeafName;
                newItem.Update();
            }
        }

        public static Folder GetRootFolder(this List value, ClientContext context)
        {
            var folder = value.RootFolder;
            context.Load(folder, fldr => fldr.ServerRelativeUrl, fldr => fldr.Files);
            context.ExecuteQuery();
            return folder;
        }

    }
}
