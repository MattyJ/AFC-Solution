using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Core.Interfaces;
using Fujitsu.AFC.Data.Interfaces;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Services.Interfaces;
using Fujitsu.AFC.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fujitsu.AFC.Services.Tests
{
    [TestClass]
    public class ProvisioningServiceTests
    {
        private Mock<IRepository<ProvisionedSiteCollection>> _mockProvisionedSiteCollectionRepository;
        private Mock<IRepository<ProvisionedSite>> _mockProvisionedSiteRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IUserIdentity> _mockUserIdentity;

        private List<ProvisionedSiteCollection> _provisionedSiteCollections;
        private List<ProvisionedSite> _provisionedSites;

        private Mock<IParameterService> _mockParameterService;
        private IProvisioningService _provisioningService;

        private const string UserName = "matthew.jordan@uk.fujitsu.com";
        private readonly DateTime _dateTime = DateTime.Now;

        [TestInitialize]
        public void Intialize()
        {
            _mockParameterService = new Mock<IParameterService>();

            _provisionedSiteCollections = new List<ProvisionedSiteCollection>
            {
                new ProvisionedSiteCollection
                {
                    Id = 1,
                    Name = "DCF-D10",
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime
                },
                new ProvisionedSiteCollection
                {
                    Id = 2,
                    Name = "DCF-D11",
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime
                },
                new ProvisionedSiteCollection
                {
                    Id = 3,
                    Name = "DCF-D12",
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
                    ProvisionedSiteCollectionId = 1,
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
                    ProvisionedSiteCollectionId = 1,
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
                    ProvisionedSiteCollectionId = 1,
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
                    ProvisionedSiteCollectionId = 1,
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
                    ProvisionedSiteCollectionId = 2,
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime
                }

            };

            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserIdentity = new Mock<IUserIdentity>();

            _mockParameterService = new Mock<IParameterService>();

            _mockProvisionedSiteCollectionRepository = MockRepositoryHelper.Create(_provisionedSiteCollections, (entity, id) => entity.Id == (int)id);
            _mockProvisionedSiteRepository = MockRepositoryHelper.Create(_provisionedSites, (entity, id) => entity.Id == (int)id);
            _provisioningService = new ProvisioningService(_mockParameterService.Object, _mockProvisionedSiteCollectionRepository.Object, _mockProvisionedSiteRepository.Object, _mockUserIdentity.Object, _mockUnitOfWork.Object);
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProvisioningService_Constructor_NoParameterService()
        {
            #region Arrange

            #endregion

            #region Act

            new ProvisioningService(
                null,
                _mockProvisionedSiteCollectionRepository.Object,
                _mockProvisionedSiteRepository.Object,
                _mockUserIdentity.Object,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProvisioningService_Constructor_NoProvisioningSiteCollectionRepository()
        {
            #region Arrange

            #endregion

            #region Act

            new ProvisioningService(
                _mockParameterService.Object,
                null,
                _mockProvisionedSiteRepository.Object,
                _mockUserIdentity.Object,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProvisioningService_Constructor_NoProvisionedSiteRepository()
        {
            #region Arrange

            #endregion

            #region Act

            new ProvisioningService(
                _mockParameterService.Object,
                _mockProvisionedSiteCollectionRepository.Object,
                null,
                _mockUserIdentity.Object,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProvisioningService_Constructor_NoUserIdentity()
        {
            #region Arrange

            #endregion

            #region Act

            new ProvisioningService(
                _mockParameterService.Object,
                _mockProvisionedSiteCollectionRepository.Object,
                _mockProvisionedSiteRepository.Object,
                null,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProvisioningService_Constructor_NoUnitOfWork()
        {
            #region Arrange

            #endregion

            #region Act

            new ProvisioningService(
                _mockParameterService.Object,
                _mockProvisionedSiteCollectionRepository.Object,
                _mockProvisionedSiteRepository.Object,
                _mockUserIdentity.Object,
                null);

            #endregion

            #region Assert

            #endregion
        }



        #endregion

        [TestMethod]
        public void ProvisioningService_GetNumberOfAllocatedSites_QueriesRepository()
        {
            _provisioningService.GetNumberOfUnallocatedSites();

            _mockProvisionedSiteRepository.Verify(v => v.Query(It.IsAny<Expression<Func<ProvisionedSite, bool>>>()), Times.Once);
        }

        [TestMethod]
        public void ProvisioningService_GetNumberOfAllocatedSites_ReturnsCorrectResult()
        {
            var result = _provisioningService.GetNumberOfUnallocatedSites();

            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void ProvisioningService_GetNumberOfProvisionedSitesWithinCollection_ReturnsCorrectResult()
        {
            var result = _provisioningService.GetNumberOfProvisionedSitesWithinCollection(1);

            Assert.AreEqual(4, result);
            _mockParameterService.Verify(v => v.GetParameterByName<int>(ParameterNames.SiteProvisionerCurrentSiteCollectionId), Times.Never);
            _mockParameterService.Verify(v => v.SaveParameter<int>(ParameterNames.SiteProvisionerCurrentSiteCollectionId, It.IsAny<int>()), Times.Never);

        }

        [TestMethod]
        public void ProvisioningService_GetSiteCollectionIdTenSitesPerCollection_ReturnsCorrectResult()
        {
            _mockParameterService.Setup(s => s.GetParameterByNameAndCache<int>(It.Is<string>(m => m == ParameterNames.SiteProvisionerSitesPerSiteCollection))).Returns(10);
            var result = _provisioningService.GetSiteCollectionId();

            Assert.AreEqual(1, result);
            _mockParameterService.Verify(v => v.GetParameterByName<int>(ParameterNames.SiteProvisionerCurrentSiteCollectionId), Times.Never);
            _mockParameterService.Verify(v => v.SaveParameter<int>(ParameterNames.SiteProvisionerCurrentSiteCollectionId, It.IsAny<int>()), Times.Never);

        }

        [TestMethod]
        public void ProvisioningService_GetSiteCollectionIdFiveSitesPerCollection_ReturnsCorrectResult()
        {
            _mockParameterService.Setup(s => s.GetParameterByNameAndCache<int>(It.Is<string>(m => m == ParameterNames.SiteProvisionerSitesPerSiteCollection))).Returns(5);

            var result = _provisioningService.GetSiteCollectionId();

            Assert.AreEqual(1, result);
            _mockParameterService.Verify(v => v.GetParameterByName<int>(ParameterNames.SiteProvisionerCurrentSiteCollectionId), Times.Never);
            _mockParameterService.Verify(v => v.SaveParameter<int>(ParameterNames.SiteProvisionerCurrentSiteCollectionId, It.IsAny<int>()), Times.Never);

        }

        [TestMethod]
        public void ProvisioningService_GetSiteCollectionIdFourSitesPerCollection_ReturnsCorrectResult()
        {
            _mockParameterService.Setup(s => s.GetParameterByNameAndCache<int>(It.Is<string>(m => m == ParameterNames.SiteProvisionerSitesPerSiteCollection))).Returns(4);

            var result = _provisioningService.GetSiteCollectionId();

            Assert.AreEqual(2, result);

            _mockParameterService.Verify(v => v.GetParameterByName<int>(ParameterNames.SiteProvisionerCurrentSiteCollectionId), Times.Never);
            _mockParameterService.Verify(v => v.SaveParameter<int>(ParameterNames.SiteProvisionerCurrentSiteCollectionId, It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void ProvisioningService_GetSiteCollectionIdOneSitePerCollection_ReturnsCorrectResult()
        {
            _mockParameterService.Setup(s => s.GetParameterByNameAndCache<int>(It.Is<string>(m => m == ParameterNames.SiteProvisionerSitesPerSiteCollection))).Returns(1);
            _mockParameterService.Setup(s => s.GetParameterByName<int>(ParameterNames.SiteProvisionerCurrentSiteCollectionId)).Returns(2);

            var result = _provisioningService.GetSiteCollectionId();

            Assert.AreEqual(3, result);

            _mockParameterService.Verify(v => v.GetParameterByName<int>(ParameterNames.SiteProvisionerCurrentSiteCollectionId), Times.Once);
            _mockParameterService.Verify(v => v.SaveParameter<int>(ParameterNames.SiteProvisionerCurrentSiteCollectionId, 3), Times.Once);
        }
    }
}
