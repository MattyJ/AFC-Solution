using System;
using System.Data.Entity;
using Fujitsu.AFC.Model;

namespace Fujitsu.AFC.Data.Interfaces
{
    public interface IAFCDataContext : IDisposable
    {
        IDbSet<Parameter> Parameters { get; set; }
        IDbSet<TimerLock> TimerLocks { get; set; }
        IDbSet<Task> Tasks { get; set; }
        IDbSet<HistoryLog> TaskHistoryLogs { get; set; }
        IDbSet<ProvisionedSiteCollection> ProvisionedSiteCollections { get; set; }
        IDbSet<ProvisionedSite> ProvisionedSites { get; set; }
        IDbSet<Site> Sites { get; set; }
        IDbSet<Library> Libraries { get; set; }
    }
}
