using System.Net;
using Fujitsu.AFC.Model;
using Microsoft.SharePoint.Client;

namespace Fujitsu.AFC.Services.Interfaces
{
    public interface IContextService
    {
        Web Load(ClientContext context, ICredentials credentials);
        void Audit(ClientContext context, Task task, string auditMessage);
    }
}
