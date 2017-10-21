using Fujitsu.AFC.Model;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace Fujitsu.AFC.Extensions.CSOM
{
    [ExcludeFromCodeCoverage]
    public static class ClientContextExtensions
    {
        private const string PinKeyValue = "PIN";
        public static Web LoadWebContextWebsAndLists(this ClientContext value, ICredentials credentials)
        {
            value.Credentials = credentials;
            value.Load(value.Web, w => w.Webs, w => w.Lists);
            value.ExecuteQuery();
            return value.Web;
        }
        public static List GetListByTitle(this ClientContext value, Web web, string listTitle)
        {
            var list = web.Lists.GetByTitle(listTitle);
            value.Load(list);
            value.ExecuteQuery();

            return list;
        }

        public static List GetListById(this ClientContext value, Web web, Guid id)
        {
            var list = web.Lists.GetById(id);
            value.Load(list);
            value.ExecuteQuery();

            return list;
        }

        public static string CreateDocumentLibraryMetaData(this ClientContext value, List list, Web web, string dictionary)
        {
            if (dictionary == null) return null;
            var keyValuePairDictionary = dictionary.ToKeyValueDictionaryIgnoreEmptyValues();
            value.CreateDocumentLibraryMetadata(list, web, keyValuePairDictionary);
            return keyValuePairDictionary.ToXmlString();
        }

        public static string CreateDocumentLibraryMetaData(this ClientContext value, List list, Web web, string dictionary, int pin)
        {
            if (dictionary == null) return null;
            var keyValuePairDictionary = dictionary.ToKeyValueDictionaryIgnoreEmptyValues();
            if (keyValuePairDictionary.ContainsKey(PinKeyValue))
            {
                keyValuePairDictionary[PinKeyValue] = pin.ToString();
            }

            value.CreateDocumentLibraryMetadata(list, web, keyValuePairDictionary);
            return keyValuePairDictionary.ToXmlString();
        }

        public static string UpdateDocumentLibraryMetaData(this ClientContext value, List list, Web web, string masterMetadata, string deltaMetadata)
        {
            var masterKeyValuePairDictionary = masterMetadata.ToKeyValueDictionary();
            var deltaKeyValuePairDictionary = deltaMetadata.ToKeyValueDictionary();

            masterKeyValuePairDictionary.MergeDictionary(deltaKeyValuePairDictionary);

            value.CreateDocumentLibraryMetadata(list, web, masterKeyValuePairDictionary);

            return masterKeyValuePairDictionary.ToXmlString();
        }

        private static void CreateDocumentLibraryMetadata(this ClientContext value, List list, Web web, IReadOnlyDictionary<string, string> dictionary)
        {
            var matchingMetadataItems = list.Fields.Where(field => !field.Hidden && dictionary.ContainsKey(field.Title))
                .ToDictionary(field => field.StaticName, field => dictionary[field.Title]);
            value.CreateDefaultLibraryMetata(web, list.Id, matchingMetadataItems);
        }

        private static void CreateDefaultLibraryMetata(this ClientContext value, Web web, Guid id, Dictionary<string, string> matchingMetadataItems)
        {
            if (!matchingMetadataItems.Any()) return;

            string content;

            var list = value.GetListById(web, id);

            // Create the file object to store the xhtml content
            var folderUrl = string.Concat(list.EntityTypeName, "/Forms/");
            var formsFolder = web.GetFolderByServerRelativeUrl(folderUrl);

            // Derive the objects to create the xhtml content
            var hrefValue = string.Concat(list.ParentWebUrl.TrimEnd('/'), "/", list.EntityTypeName);
            using (var str = new StringWriter())
            using (var xml = new XmlTextWriter(str))
            {
                xml.WriteStartElement("MetadataDefaults");
                xml.WriteStartElement("a");
                xml.WriteAttributeString("href", hrefValue);

                foreach (var metadataItem in matchingMetadataItems)
                {
                    xml.WriteStartElement("DefaultValue");
                    xml.WriteAttributeString("FieldName", metadataItem.Key);
                    xml.WriteString(metadataItem.Value);
                    xml.WriteEndElement(); // </DefaultValue>
                }

                xml.WriteEndElement(); // </a>
                xml.WriteEndElement(); // </MetadataDefaults>

                content = str.ToString();
            }

            var fci = new FileCreationInformation
            {
                Content = Encoding.UTF8.GetBytes(content),
                Url = "client_LocationBasedDefaults.html",
                Overwrite = true
            };
            var metaDataFile = formsFolder.Files.Add(fci);

            // Save the information
            value.Load(metaDataFile);
            value.ExecuteQuery();

            // Binding the OOTB event receiver if it isn't there
            var receiverCheck = value.LoadQuery(list.EventReceivers.Where(r => r.ReceiverClass == "Microsoft.Office.DocumentManagement.LocationBasedMetadataDefaultsReceiver"));
            value.ExecuteQuery();

            if (!receiverCheck.Any())
            {
                var erci = new EventReceiverDefinitionCreationInformation
                {
                    ReceiverName = "LocationBasedMetadataDefaultsReceiver ItemAdded",
                    SequenceNumber = 1000,
                    ReceiverClass = "Microsoft.Office.DocumentManagement.LocationBasedMetadataDefaultsReceiver",
                    ReceiverAssembly = "Microsoft.Office.DocumentManagement, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c",
                    EventType = EventReceiverType.ItemAdded,
                    Synchronization = EventReceiverSynchronization.Synchronous
                };

                var receiver = list.EventReceivers.Add(erci);
                value.Load(receiver);
                value.ExecuteQuery();
            }
        }

        public static void ActivateFeature(this ClientContext value, string featureId)
        {
            var features = value.Site.Features;
            value.Load(features);
            value.ExecuteQuery();

            var guid = new Guid(featureId);
            if (!IsFeatureActive(value, features, guid))
            {
                features.Add(guid, true, FeatureDefinitionScope.None);
                value.ExecuteQuery();
            }
        }

        public static bool IsFeatureActive(this ClientContext value, FeatureCollection features, Guid featureId)
        {
            var featureIsActive = false;
            value.Load(features);
            value.ExecuteQuery();

            var iprFeature = features.GetById(featureId);

            if (iprFeature != null && iprFeature.IsPropertyAvailable("DefinitionId") && !iprFeature.ServerObjectIsNull.Value && iprFeature.DefinitionId.Equals(featureId))
            {
                featureIsActive = true;
            }

            return featureIsActive;
        }

        public static void SetPropertyBagValue(this ClientContext value, string key, object objectValue)
        {

            var props = value.Web.AllProperties;
            value.Load(props);
            value.ExecuteQuery();

            // Get the value, if the web properties are already loaded
            if (props.FieldValues.Count > 0)
            {
                props[key] = objectValue;
            }
            else
            {
                // Load the web properties
                value.Load(props);
                value.ExecuteQuery();

                props[key] = objectValue;
            }

            value.Web.Update();
            value.ExecuteQuery();
        }

        public static void RestrictPinGroupPermissions(this ClientContext context, List<User> users, Group pinGroup, string pin, bool restrict)
        {
            if (!users.Any()) return;

            foreach (var user in users)
            {
                var projectSecurityGroup = context.Web.EnsureUser($"{user.Title}_{pin}");
                context.Load(projectSecurityGroup);
                context.ExecuteQuery();
                if (restrict)
                {
                    pinGroup.Users.AddUser(projectSecurityGroup);
                    pinGroup.Users.RemoveById(user.Id);
                }
            }
        }

        public static void RemoveRestrictedPinGroupPermissions(this ClientContext context, List<User> users, Group pinGroup, string pin, bool remove)
        {
            if (!users.Any()) return;

            foreach (var user in users)
            {
                var projectSecurityGroupName = user.Title.Contains("CONTRIBUTE") ? user.Title.Replace($"CONTRIBUTE_{pin}", "CONTRIBUTE") : user.Title.Replace($"READ_{pin}", "READ");
                var projectSecurityGroup = context.Web.EnsureUser(projectSecurityGroupName);
                context.Load(projectSecurityGroup);
                context.ExecuteQuery();
                if (remove)
                {
                    pinGroup.Users.AddUser(projectSecurityGroup);
                    pinGroup.Users.RemoveById(user.Id);
                }
            }
        }

        public static void RestrictDocumentLibraryContributePermissions(this ClientContext value, Web web, Web spSite, List<Library> libraries, int pin, string contributorGroupName, string contributorClosedGroupName, bool restrict)
        {
            if (!libraries.Any()) return;

            foreach (var library in libraries)
            {
                var documentLibraryList = spSite.Lists.GetById(library.ListId);
                value.Load(documentLibraryList);
                value.Load(documentLibraryList.RoleAssignments, r => r.Include(m => m.Member.Id));
                value.ExecuteQuery();
                var contributorProjectGroup = value.Web.GetActiveDirectoryContributorGroup(contributorGroupName, contributorClosedGroupName, false, library.IsClosed, pin, library.ProjectId);
                var pinProjectContributorGroup = value.Web.GetActiveDirectoryContributorGroup(contributorGroupName, contributorClosedGroupName, true, library.IsClosed, pin, library.ProjectId);
                value.Load(contributorProjectGroup, r => r.Id);
                value.Load(pinProjectContributorGroup);
                value.ExecuteQuery();
                if (restrict)
                {
                    documentLibraryList.RoleAssignments.Where(r => r.Member.Id == contributorProjectGroup.Id).ToList().ForEach(ra => ra.DeleteObject());
                    documentLibraryList.AddRoleWithPermissions(value, web, pinProjectContributorGroup, RoleType.Contributor);
                    value.ExecuteQuery();
                }

            }
        }

        public static void RemoveRestrictedDocumentLibraryContributePermissions(this ClientContext value, Web web, Web spSite, List<Library> libraries, int pin, string contributorGroupName, string contributorClosedGroupName, bool removeRestriction)
        {
            if (!libraries.Any()) return;

            foreach (var library in libraries)
            {
                var documentLibraryList = spSite.Lists.GetById(library.ListId);
                value.Load(documentLibraryList);
                value.Load(documentLibraryList.RoleAssignments, r => r.Include(m => m.Member.Id));
                value.ExecuteQuery();
                var contributorProjectGroup = value.Web.GetActiveDirectoryContributorGroup(contributorGroupName, contributorClosedGroupName, true, library.IsClosed, pin, library.ProjectId);
                var pinProjectContributorGroup = value.Web.GetActiveDirectoryContributorGroup(contributorGroupName, contributorClosedGroupName, false, library.IsClosed, pin, library.ProjectId);
                value.Load(contributorProjectGroup, r => r.Id);
                value.Load(pinProjectContributorGroup);
                value.ExecuteQuery();
                if (removeRestriction)
                {
                    documentLibraryList.RoleAssignments.Where(r => r.Member.Id == contributorProjectGroup.Id).ToList().ForEach(ra => ra.DeleteObject());
                    documentLibraryList.AddRoleWithPermissions(value, web, pinProjectContributorGroup, RoleType.Contributor);
                    value.ExecuteQuery();
                }
            }
        }
    }
}
