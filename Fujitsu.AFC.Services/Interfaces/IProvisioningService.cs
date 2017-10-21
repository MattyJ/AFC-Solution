using Fujitsu.AFC.Model;

namespace Fujitsu.AFC.Services.Interfaces
{
    public interface IProvisioningService
    {
        int GetNumberOfUnallocatedSites();
        int GetSiteCollectionId();
        int GetNumberOfProvisionedSitesWithinCollection(int id);
        int ProvisionSiteCollection(int siteCollectionId);
        void ProvisionSite(int siteCollectionId, Task task);
        void ConfigureSiteCollection(int siteCollectionId);
        void CreateSiteAudit(int siteCollectionId, Task task);
    }
}
