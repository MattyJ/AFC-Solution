using System;
using System.Collections.Generic;
using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Core.Interfaces;
using Fujitsu.AFC.Data.Interfaces;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Provisioning.Interfaces;
using Fujitsu.AFC.Provisioning.Tasks;
using Fujitsu.AFC.Services;
using Fujitsu.AFC.Services.Interfaces;
using Fujitsu.AFC.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fujitsu.AFC.Provisioning.Tests.Tasks
{
    [TestClass]
    public class CaseSiteProvisioningTests
    {
        private Mock<IParameterService> _mockParameterService;
        private Mock<IProvisioningService> _mockProvisioningService;
        private Mock<IRepository<ProvisionedSiteCollection>> _mockProvisionedSiteCollectionRepository;
        private Mock<IRepository<ProvisionedSite>> _mockProvisionedSiteRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IUserIdentity> _mockUserIdentity;

        private IProvisioningService _provisioningService;

        private List<ProvisionedSiteCollection> _provisionedSiteCollections;
        private List<ProvisionedSite> _provisionedSites;

        private IProvisioningTaskProcessor _provisioningTaskProcessor;

        private const string UserName = "matthew.jordan@uk.fujitsu.com";
        private readonly DateTime _dateTime = DateTime.Now;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockParameterService = new Mock<IParameterService>();
            _mockProvisioningService = new Mock<IProvisioningService>();

            _provisionedSiteCollections = new List<ProvisionedSiteCollection>
            {
                new ProvisionedSiteCollection
                {
                    Id = 1,
                    Name = "DCF-L1",
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime
                },
                new ProvisionedSiteCollection
                {
                    Id = 2,
                    Name = "DCF-L2",
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime
                },
                new ProvisionedSiteCollection
                {
                    Id = 3,
                    Name = "DCF-L3",
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime
                }
            };

            _provisionedSites = new List<ProvisionedSite>
            {
                new ProvisionedSite
                {
                    Id = 1,
                    IsAllocated = false,
                    Url = "https://mattjordan.sharepoint.com/sites/DCF-D10/97ce61fb-736f-4cbc-9c48-751120380da2",
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime
                },
                new ProvisionedSite
                {
                    Id = 2,
                    IsAllocated = true,
                    Url = "https://mattjordan.sharepoint.com/sites/DCF-D10/fd240ec3-e872-490c-9d4c-ee1b4756d3d7",
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime
                },
                new ProvisionedSite
                {
                    Id = 3,
                    IsAllocated = false,
                    Url = "https://mattjordan.sharepoint.com/sites/DCF-D10/ab2e5c1c-8cd2-4264-b60e-13f89f96810b",
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime
                },
                new ProvisionedSite
                {
                    Id = 4,
                    IsAllocated = false,
                    Url = "https://mattjordan.sharepoint.com/sites/DCF-D10/bf0c92bd-2b08-46eb-9b41-19259c258807",
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime
                },
                new ProvisionedSite
                {
                    Id = 5,
                    IsAllocated = true,
                    Url = "https://mattjordan.sharepoint.com/sites/DCF-D11/13b5319a-9ce5-4f25-90ab-45a4d15b91d1",
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime
                }

            };

            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserIdentity = new Mock<IUserIdentity>();
            _mockProvisionedSiteCollectionRepository = MockRepositoryHelper.Create(_provisionedSiteCollections, (entity, id) => entity.Id == (int)id);
            _mockProvisionedSiteRepository = MockRepositoryHelper.Create(_provisionedSites, (entity, id) => entity.Id == (int)id);

            _provisioningService = new ProvisioningService(_mockParameterService.Object, _mockProvisionedSiteCollectionRepository.Object, _mockProvisionedSiteRepository.Object, _mockUserIdentity.Object, _mockUnitOfWork.Object);

            _provisioningTaskProcessor = new CaseSiteProvisioning(_provisioningService, _mockParameterService.Object);
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CaseSiteProvisioning_Constructor_NoProvisioningService()
        {
            #region Arrange

            #endregion

            #region Act

            new CaseSiteProvisioning(
                null,
                _mockParameterService.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CaseSiteProvisioning_Constructor_NoParameterService()
        {
            #region Arrange

            #endregion

            #region Act

            new CaseSiteProvisioning(
                _mockProvisioningService.Object,
                null);

            #endregion

            #region Assert

            #endregion
        }

        #endregion       
    }
}
