using System;
using System.Collections.Generic;
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
    public class PinServiceTests
    {
        private Mock<IRepository<Site>> _mockSiteRepository;
        private Mock<IRepository<Library>> _mockLibraryRepository;
        private Mock<IRepository<ProvisionedSite>> _mockProvisionedSiteRepository;
        private Mock<IRepository<ProvisionedSiteCollection>> _mockProvisionedSiteCollectionRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IUserIdentity> _mockUserIdentity;
        private List<Site> _sites;
        private List<ProvisionedSite> _provisionedSites;
        private List<ProvisionedSiteCollection> _provisionedSiteCollections;

        private Mock<IParameterService> _mockParameterService;
        private IPinService _pinService;
        private Mock<IService<Site>> _mockSiteService;
        private Mock<ILibraryService> _mockLibraryService;

        private const string UserName = "matthew.jordan@uk.fujitsu.com";
        private readonly DateTime _dateTime = DateTime.Now;
        private Mock<IContextService> _mockContextService;

        [TestInitialize]
        public void Intialize()
        {
            _sites = new List<Site>();

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
                    Id = 1,
                    Name = "DCF-D11",
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime
                },
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
            _mockContextService = new Mock<IContextService>();
            _mockSiteService = new Mock<IService<Site>>();
            _mockLibraryService = new Mock<ILibraryService>();

            _mockLibraryRepository = new Mock<IRepository<Library>>();

            _mockSiteRepository = MockRepositoryHelper.Create(_sites, (entity, id) => entity.Id == (int)id);
            _mockProvisionedSiteCollectionRepository = MockRepositoryHelper.Create(_provisionedSiteCollections, (entity, id) => entity.Id == (int)id);
            _mockProvisionedSiteRepository = MockRepositoryHelper.Create(_provisionedSites, (entity, id) => entity.Id == (int)id);
            _mockSiteRepository = new Mock<IRepository<Site>>();
            _pinService = new PinService(_mockParameterService.Object, _mockContextService.Object, _mockSiteService.Object, _mockLibraryService.Object, _mockProvisionedSiteCollectionRepository.Object, _mockProvisionedSiteRepository.Object, _mockSiteRepository.Object, _mockLibraryRepository.Object, _mockUserIdentity.Object, _mockUnitOfWork.Object);
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PinService_Constructor_NoParameterService()
        {
            #region Arrange

            #endregion

            #region Act

            new PinService(
                null,
                _mockContextService.Object,
                _mockSiteService.Object,
                _mockLibraryService.Object,
                _mockProvisionedSiteCollectionRepository.Object,
                _mockProvisionedSiteRepository.Object,
                _mockSiteRepository.Object,
                _mockLibraryRepository.Object,
                _mockUserIdentity.Object,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PinService_Constructor_NoContextService()
        {
            #region Arrange

            #endregion

            #region Act

            new PinService(
                _mockParameterService.Object,
                null,
                _mockSiteService.Object,
                _mockLibraryService.Object,
                _mockProvisionedSiteCollectionRepository.Object,
                _mockProvisionedSiteRepository.Object,
                _mockSiteRepository.Object,
                _mockLibraryRepository.Object,
                _mockUserIdentity.Object,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PinService_Constructor_NoSiteService()
        {
            #region Arrange

            #endregion

            #region Act

            new PinService(
                _mockParameterService.Object,
                _mockContextService.Object,
                null,
                _mockLibraryService.Object,
                _mockProvisionedSiteCollectionRepository.Object,
                _mockProvisionedSiteRepository.Object,
                _mockSiteRepository.Object,
                _mockLibraryRepository.Object,
                _mockUserIdentity.Object,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PinService_Constructor_NoLibraryService()
        {
            #region Arrange

            #endregion

            #region Act

            new PinService(
                _mockParameterService.Object,
                _mockContextService.Object,
                _mockSiteService.Object,
                null,
                _mockProvisionedSiteCollectionRepository.Object,
                _mockProvisionedSiteRepository.Object,
                _mockSiteRepository.Object,
                _mockLibraryRepository.Object,
                _mockUserIdentity.Object,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PinService_Constructor_NoProvisioningSiteCollectionRepository()
        {
            #region Arrange

            #endregion

            #region Act

            new PinService(
                _mockParameterService.Object,
                _mockContextService.Object,
                _mockSiteService.Object,
                _mockLibraryService.Object,
                null,
                _mockProvisionedSiteRepository.Object,
                _mockSiteRepository.Object,
                _mockLibraryRepository.Object,
                _mockUserIdentity.Object,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PinService_Constructor_NoProvisioningSiteRepository()
        {
            #region Arrange

            #endregion

            #region Act

            new PinService(
                _mockParameterService.Object,
                _mockContextService.Object,
                _mockSiteService.Object,
                _mockLibraryService.Object,
                _mockProvisionedSiteCollectionRepository.Object,
                null,
                _mockSiteRepository.Object,
                _mockLibraryRepository.Object,
                _mockUserIdentity.Object,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PinService_Constructor_NoSiteRepository()
        {
            #region Arrange

            #endregion

            #region Act

            new PinService(
                _mockParameterService.Object,
                _mockContextService.Object,
                _mockSiteService.Object,
                _mockLibraryService.Object,
                _mockProvisionedSiteCollectionRepository.Object,
                _mockProvisionedSiteRepository.Object,
                null,
                _mockLibraryRepository.Object,
                _mockUserIdentity.Object,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PinService_Constructor_NoLibaryRepository()
        {
            #region Arrange

            #endregion

            #region Act

            new PinService(
                _mockParameterService.Object,
                _mockContextService.Object,
                _mockSiteService.Object,
                _mockLibraryService.Object,
                _mockProvisionedSiteCollectionRepository.Object,
                _mockProvisionedSiteRepository.Object,
                _mockSiteRepository.Object,
                null,
                _mockUserIdentity.Object,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PinService_Constructor_NoUserIdentity()
        {
            #region Arrange

            #endregion

            #region Act

            new PinService(
                _mockParameterService.Object,
                _mockContextService.Object,
                _mockSiteService.Object,
                _mockLibraryService.Object,
                _mockProvisionedSiteCollectionRepository.Object,
                _mockProvisionedSiteRepository.Object,
                _mockSiteRepository.Object,
                _mockLibraryRepository.Object,
                null,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PinService_Constructor_NoUnitOfWork()
        {
            #region Arrange

            #endregion

            #region Act

            new PinService(
                _mockParameterService.Object,
                _mockContextService.Object,
                _mockSiteService.Object,
                _mockLibraryService.Object,
                _mockProvisionedSiteCollectionRepository.Object,
                _mockProvisionedSiteRepository.Object,
                _mockSiteRepository.Object,
                _mockLibraryRepository.Object,
                _mockUserIdentity.Object,
                null);

            #endregion

            #region Assert

            #endregion
        }

        #endregion
    }
}
