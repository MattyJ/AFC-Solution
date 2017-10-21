using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Core.Metrics;
using Fujitsu.AFC.Extensions.CSOM;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Services.Interfaces;
using Microsoft.SharePoint.Client;


namespace Fujitsu.AFC.Services
{
    [ExcludeFromCodeCoverage]
    public class ContextService : IContextService
    {
        public Web Load(ClientContext context, ICredentials credentials)
        {
            Debug.WriteLine("Fujitsu.AFC.Services.ContextService.cs -> Load. Started.");
            var loadContextWebListsPrfMonTriggers = new PrfMon();
            var web = context.LoadWebContextWebsAndLists(credentials);
            loadContextWebListsPrfMonTriggers.Stop();
            Debug.WriteLine("Fujitsu.AFC.Services.ContextService.cs -> Completed Processing Load. Duration: {0:0.000}s", loadContextWebListsPrfMonTriggers.Stop());
            return web;
        }

        public void Audit(ClientContext context, Task task, string auditMessage)
        {
            Debug.WriteLine("Fujitsu.AFC.Services.ContextService.cs -> AuditOperation. Started.");
            var auditOperationPrfMonTriggers = new PrfMon();
            context.Web.AddAuditListItem(SharePointListNames.Audit, task, auditMessage);
            context.ExecuteQuery();
            Debug.WriteLine("Fujitsu.AFC.Services.ContextService.cs -> AuditOperation. Completed. Average Execution: {0:0.000}s", auditOperationPrfMonTriggers.Stop());
        }
    }
}
