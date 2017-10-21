using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Core.Interfaces;
using Fujitsu.AFC.Data.Interfaces;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Services.Interfaces;
using Fujitsu.AFC.Tasks;
using Fujitsu.AFC.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fujitsu.AFC.Services.Tests
{
    [TestClass]
    public class CaseServiceTests
    {
        private Mock<IRepository<Site>> _mockSiteRepository;
        private Mock<IRepository<Library>> _mockLibraryRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IUserIdentity> _mockUserIdentity;

        private Mock<IParameterService> _mockParameterService;
        private Mock<ILibraryService> _mockLibraryService;
        private ICaseService _caseService;
        private Mock<IContextService> _mockContextService;

        private List<Site> _sites;
        private List<Library> _libraries;

        private const string UserName = "matthew.jordan@uk.fujitsu.com";
        private readonly DateTime _dateTime = DateTime.Now;


        [TestInitialize]
        public void Intialize()
        {
            _sites = new List<Site>
            {
                new Site
                {
                    Id = 1,
                    Url = "https://fujitsuuki.sharepoint.com/sites/DCF-T1/12345",
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime
                },
                new Site
                {
                    Id = 2,
                    Url = "https://fujitsuuki.sharepoint.com/sites/DCF-T1/23456",
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime
                },
                new Site
                {
                    Id = 3,
                    Url = "https://fujitsuuki.sharepoint.com/sites/DCF-T1/34567",
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime
                }

            };

            _libraries = new List<Library>
            {
                new Library
                {
                    Id = 1,
                    SiteId = 1,
                    Site = _sites.First(x => x.Id == 1),
                    CaseId = 1,
                    ProjectId = 1,
                    Title = "Library One",
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime
                },
                new Library
                {
                    Id = 2,
                    SiteId = 2,
                    Site = _sites.First(x => x.Id == 2),
                    CaseId = 2,
                    ProjectId = 2,
                    Title = "Library Two",
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime
                },
                new Library
                {
                    Id = 3,
                    SiteId = 1,
                    Site = _sites.First(x => x.Id == 1),
                    CaseId = 3,
                    ProjectId = 3,
                    Title = "Library Three",
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime
                }

            };

            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserIdentity = new Mock<IUserIdentity>();
            _mockParameterService = new Mock<IParameterService>();
            _mockSiteRepository = MockRepositoryHelper.Create(_sites);
            _mockLibraryRepository = MockRepositoryHelper.Create(_libraries);
            _mockLibraryService = new Mock<ILibraryService>();
            _mockContextService = new Mock<IContextService>();

            _caseService = new CaseService(_mockParameterService.Object, _mockContextService.Object, _mockLibraryService.Object, _mockSiteRepository.Object, _mockLibraryRepository.Object, _mockUserIdentity.Object, _mockUnitOfWork.Object);

            Bootstrapper.Initialise();
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CaseService_Constructor_NoParameterService()
        {
            #region Arrange

            #endregion

            #region Act

            new CaseService(
                null,
                _mockContextService.Object,
                _mockLibraryService.Object,
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
        public void CaseService_Constructor_NoContextService()
        {
            #region Arrange

            #endregion

            #region Act

            new CaseService(
                _mockParameterService.Object,
                null,
                _mockLibraryService.Object,
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
        public void CaseService_Constructor_NoLibraryService()
        {
            #region Arrange

            #endregion

            #region Act

            new CaseService(
                _mockParameterService.Object,
                _mockContextService.Object,
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
        public void CaseService_Constructor_NoSiteRepository()
        {
            #region Arrange

            #endregion

            #region Act

            new CaseService(
                _mockParameterService.Object,
                _mockContextService.Object,
                _mockLibraryService.Object,
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
        public void CaseService_Constructor_NoLibraryRepository()
        {
            #region Arrange

            #endregion

            #region Act

            new CaseService(
                _mockParameterService.Object,
                _mockContextService.Object,
                _mockLibraryService.Object,
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
        public void CaseService_Constructor_NoUserIdentity()
        {
            #region Arrange

            #endregion

            #region Act

            new CaseService(
                _mockParameterService.Object,
                _mockContextService.Object,
                _mockLibraryService.Object,
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
        public void CaseService_Constructor_NoUnitOfWork()
        {
            #region Arrange

            #endregion

            #region Act

            new CaseService(
                _mockParameterService.Object,
                _mockContextService.Object,
                _mockLibraryService.Object,
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
