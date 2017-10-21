using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics.CodeAnalysis;
using Fujitsu.AFC.Data.Interfaces;
using Fujitsu.AFC.Model;

namespace Fujitsu.AFC.Data
{
    [ExcludeFromCodeCoverage]
    public class AFCDataContext : DbContext, IAFCDataContext
    {
        static AFCDataContext()
        {
            Database.SetInitializer<AFCDataContext>(null);
            //SqlProviderServices.TruncateDecimalsToScale = false;
        }

        public AFCDataContext()
            : base("AFCDataContext")
        {
        }

        public AFCDataContext(string connectionString)
            : base(connectionString)
        {
        }

        public IDbSet<Parameter> Parameters { get; set; }
        public IDbSet<TimerLock> TimerLocks { get; set; }
        public IDbSet<Task> Tasks { get; set; }
        public IDbSet<HistoryLog> TaskHistoryLogs { get; set; }
        public IDbSet<ProvisionedSiteCollection> ProvisionedSiteCollections { get; set; }
        public IDbSet<ProvisionedSite> ProvisionedSites { get; set; }
        public IDbSet<Site> Sites { get; set; }
        public IDbSet<Library> Libraries { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
