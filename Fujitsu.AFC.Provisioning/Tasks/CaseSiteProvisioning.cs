using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Core.Metrics;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Provisioning.Interfaces;
using Fujitsu.AFC.Services.Interfaces;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Fujitsu.AFC.Provisioning.Tasks
{
    public class CaseSiteProvisioning : IProvisioningTaskProcessor
    {
        private readonly IProvisioningService _provisioningService;
        private readonly IParameterService _parameterService;

        public CaseSiteProvisioning(IProvisioningService provisioningService,
            IParameterService parameterService)
        {
            if (provisioningService == null)
            {
                throw new ArgumentNullException(nameof(provisioningService));
            }
            if (parameterService == null)
            {
                throw new ArgumentNullException(nameof(parameterService));
            }

            _provisioningService = provisioningService;
            _parameterService = parameterService;
        }

        [ExcludeFromCodeCoverage]
        public void Execute(Task task)
        {
            Debug.WriteLine("Fujitsu.AFC.Provisioning.CaseSiteProvisioning.cs -> Processing Started.");
            var prfMonMethod = new PrfMon();

            var maximumNumberOfUnallocatedSites = _parameterService.GetParameterByNameAndCache<int>(ParameterNames.SiteProvisionerTotalUnallocatedSitesMaximum);
            var maximumNumberOfSitesPerCollection = _parameterService.GetParameterByNameAndCache<int>(ParameterNames.SiteProvisionerSitesPerSiteCollection);
            var totalNumberOfUnallocatedSites = _provisioningService.GetNumberOfUnallocatedSites();

            if (totalNumberOfUnallocatedSites < maximumNumberOfUnallocatedSites)
            {
                var currentSiteCollectionId = _provisioningService.GetSiteCollectionId();

                while (totalNumberOfUnallocatedSites < maximumNumberOfUnallocatedSites)
                {
                    var numberOfSitesProvisionedWithinSiteCollection = _provisioningService.GetNumberOfProvisionedSitesWithinCollection(currentSiteCollectionId);
                    if (numberOfSitesProvisionedWithinSiteCollection == 0)
                    {
                        currentSiteCollectionId = _provisioningService.ProvisionSiteCollection(currentSiteCollectionId);
                        _provisioningService.ConfigureSiteCollection(currentSiteCollectionId);
                        _provisioningService.CreateSiteAudit(currentSiteCollectionId, task);
                    }

                    _provisioningService.ProvisionSite(currentSiteCollectionId, task);

                    numberOfSitesProvisionedWithinSiteCollection++;
                    totalNumberOfUnallocatedSites++;

                    if (numberOfSitesProvisionedWithinSiteCollection >= maximumNumberOfSitesPerCollection && totalNumberOfUnallocatedSites < maximumNumberOfUnallocatedSites)
                    {
                        currentSiteCollectionId = _provisioningService.GetSiteCollectionId();
                    }
                }
            }
            Debug.WriteLine("Fujitsu.AFC.Provisioning.CaseSiteProvisioning.cs -> Completed Processing. Duration: {0:0.000}s", prfMonMethod.Stop());
        }
    }
}
