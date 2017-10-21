using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Fujitsu.AFC.Services.Model
{
    public class PinSite
    {
        public ClientContext Context { get; set; }
        public List<List> Lists { get; } = new List<List>();

        public PinSite(string siteUrl, ICredentials credentials)
        {
            Context = new ClientContext(siteUrl)
            {
                AuthenticationMode = ClientAuthenticationMode.Default,
                Credentials = credentials
            };

            GetLists();
        }

        private void GetLists()
        {
            var allLists = Context.Web.Lists;

            Context.Load(allLists, lc => lc.Include(l => l.Id, l => l.Title, l => l.IsSiteAssetsLibrary, l => l.Hidden, l => l.BaseType, l => l.ContentTypesEnabled, l => l.IsApplicationList, l => l.IsCatalog, l => l.IsPrivate, l => l.RootFolder.ServerRelativeUrl));
            Context.ExecuteQuery();
            foreach (var list in allLists)
            {
                if (!list.Hidden && (list.BaseType == BaseType.DocumentLibrary) && list.ContentTypesEnabled && !list.IsApplicationList && !list.IsCatalog && !list.IsSiteAssetsLibrary && !list.IsPrivate)
                {
                    Lists.Add(list);
                }
            }
        }

        public List GetList(List list, Dictionary<Guid, Guid> caseDocumentLibraries)
        {
            return caseDocumentLibraries.ContainsKey(list.Id) ? Lists.FirstOrDefault(l => l.Id == caseDocumentLibraries[list.Id]) : Lists.FirstOrDefault(l => l.Title == list.Title);
        }
    }
}
