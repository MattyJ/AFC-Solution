using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    public class LibraryServiceTests
    {
        private Mock<ITaskService> _mockTaskService;
        private Mock<IRepository<Library>> _mockLibraryRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;

        private List<Site> _sites;
        private List<Library> _libraries;
        private List<ProvisionedSite> _provisionedSites;
        private List<ProvisionedSiteCollection> _provisionedSiteCollections;

        private ILibraryService _libraryService;

        private const string UserName = "matthew.jordan@uk.fujitsu.com";
        private readonly DateTime _dateTime = DateTime.Now;

        [TestInitialize]
        public void Intialize()
        {
            _provisionedSiteCollections = new List<ProvisionedSiteCollection>
            {
                new ProvisionedSiteCollection
                {
                    Id = 1,
                    Name = "DCF-T1",
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime
                },
                new ProvisionedSiteCollection
                {
                    Id = 2,
                    Name = "DCF-T2",
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime
                },
                new ProvisionedSiteCollection
                {
                    Id = 3,
                    Name = "DCF-T3",
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
                    IsAllocated = true,
                    Url = "https://fujitsuuki.sharepoint.com/sites/DCF-T1/12345",
                    ProvisionedSiteCollection = _provisionedSiteCollections.First(x => x.Id == 1),
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
                    Url = "https://fujitsuuki.sharepoint.com/sites/DCF-T1/23456",
                    ProvisionedSiteCollection = _provisionedSiteCollections.First(x => x.Id == 1),
                    ProvisionedSiteCollectionId = 1,
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime
                },
                new ProvisionedSite
                {
                    Id = 3,
                    IsAllocated = true,
                    Url = "https://fujitsuuki.sharepoint.com/sites/DCF-T1/34567",
                    ProvisionedSiteCollection = _provisionedSiteCollections.First(x => x.Id == 1),
                    ProvisionedSiteCollectionId = 1,
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime
                },
                new ProvisionedSite
                {
                    Id = 4,
                    IsAllocated = true,
                    Url = "https://fujitsuuki.sharepoint.com/sites/DCF-T2/34568",
                    ProvisionedSiteCollection = _provisionedSiteCollections.First(x => x.Id == 2),
                    ProvisionedSiteCollectionId = 2,
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime
                }

            };

            _sites = new List<Site>
            {
                new Site
                {
                    Id = 1,
                    Url = "https://fujitsuuki.sharepoint.com/sites/DCF-T1/12345",
                    ProvisionedSiteId = 1,
                    ProvisionedSite = _provisionedSites.First(x => x.Id == 1),
                    Pin = 12345,
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime
                },
                new Site
                {
                    Id = 2,
                    Url = "https://fujitsuuki.sharepoint.com/sites/DCF-T1/23456",
                    ProvisionedSiteId = 2,
                    ProvisionedSite = _provisionedSites.First(x => x.Id == 2),
                    Pin = 12346,
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime
                },
                new Site
                {
                    Id = 3,
                    Url = "https://fujitsuuki.sharepoint.com/sites/DCF-T1/34567",
                    ProvisionedSiteId = 3,
                    ProvisionedSite = _provisionedSites.First(x => x.Id == 3),
                    Pin = 12347,
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime
                },
                new Site
                {
                    Id = 4,
                    Url = "https://fujitsuuki.sharepoint.com/sites/DCF-T1/34568",
                    ProvisionedSiteId = 3,
                    Pin = 12348,
                    ProvisionedSite = _provisionedSites.First(x => x.Id == 4),
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
                    ProjectId = 1,
                    Title = "Library Three",
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime
                },
                new Library
                {
                    Id = 4,
                    SiteId = 1,
                    Site = _sites.First(x => x.Id == 4),
                    CaseId = 3,
                    ProjectId = 1,
                    Title = "Library Four",
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime
                },
                new Library
                {
                    Id = 3,
                    SiteId = 1,
                    Site = _sites.First(x => x.Id == 3),
                    CaseId = 3,
                    ProjectId = 5,
                    Title = "Library Five",
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime
                },

            };

            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockLibraryRepository = MockRepositoryHelper.Create(_libraries, (entity, id) => entity.Id == (int)id);
            _mockTaskService = new Mock<ITaskService>();
            _libraryService = new LibraryService(_mockTaskService.Object, _mockLibraryRepository.Object, _mockUnitOfWork.Object);

            Bootstrapper.Initialise();
        }

        #region Constructor Tests


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LibraryService_Constructor_NoTaskService()
        {
            #region Arrange

            #endregion

            #region Act

            new LibraryService(
                null,
                _mockLibraryRepository.Object,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LibraryService_Constructor_NoLibraryRepository()
        {
            #region Arrange

            #endregion

            #region Act

            new LibraryService(
                _mockTaskService.Object,
                null,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LibraryService_Constructor_NoUnitOfWork()
        {
            #region Arrange

            #endregion

            #region Act

            new LibraryService(
                _mockTaskService.Object,
                _mockLibraryRepository.Object,
                null);

            #endregion

            #region Assert

            #endregion
        }

        #endregion

        [TestMethod]
        public void LibraryService_Create_CallsInsertAndSaveChanges()
        {
            #region Arrange

            var library = new Library
            {
                Id = 4,
                SiteId = 1,
                Site = _sites.First(x => x.Id == 1),
                CaseId = 4,
                ProjectId = 4,
                Title = "Library Four",
                InsertedBy = UserName,
                InsertedDate = _dateTime,
                UpdatedBy = UserName,
                UpdatedDate = _dateTime
            };


            #endregion

            #region Act

            _libraryService.Create(library);

            #endregion

            #region Assert

            _mockLibraryRepository.Verify(x => x.Insert(It.IsAny<Library>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void LibraryService_Update_CallUpdateAndUnitOfWorkSave()
        {
            #region Arrange

            var library = new Library
            {
                Id = 1,
            };

            #endregion

            #region Act

            _libraryService.Update(library);

            #endregion

            #region Assert

            _mockLibraryRepository.Verify(x => x.Update(It.IsAny<Library>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void LibraryService_Delete_ByEntity_CallsDeleteAndUnitOfWorkSave()
        {
            #region Arrange

            var library = new Library
            {
                Id = 1
            };

            #endregion

            #region Act

            _libraryService.Delete(library);

            #endregion

            #region Assert

            _mockLibraryRepository.Verify(x => x.Delete(It.IsAny<Library>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void LibraryService_Delete_ById_CallsDeleteAndUnitOfWorkSave()
        {
            #region Arrange

            #endregion

            #region Act

            _libraryService.Delete(1);

            #endregion

            #region Assert

            _mockLibraryRepository.Verify(x => x.Delete(It.IsAny<int>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void LibraryService_GetAll_CallsRepositoryAll()
        {
            #region Arrange

            #endregion

            #region Act

            var libraries = _libraryService.All();

            #endregion

            #region Assert

            _mockLibraryRepository.Verify(x => x.All(), Times.Once);
            Assert.AreEqual(_libraries.Count, libraries.Count());

            #endregion
        }

        [TestMethod]
        public void LibraryService_GetById_CallsRepositoryGetById()
        {
            #region Arrange

            #endregion

            #region Act

            var library = _libraryService.GetById(1);

            #endregion

            #region Assert

            _mockLibraryRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(1, library.Id);

            #endregion
        }

        [TestMethod]
        public void LibraryService_Query_CallsRepositoryQuery()
        {
            #region Arrange

            #endregion

            #region Act

            var query = _libraryService.Query(x => x.Id == 1).ToList();

            #endregion

            #region Assert


            Assert.IsNotNull(query);
            _mockLibraryRepository.Verify(x => x.Query(It.IsAny<Expression<Func<Library, bool>>>()), Times.Once());

            #endregion
        }

        [TestMethod]
        public void LibraryService_GetSiteCollectionLibraryDictionary_ReturnsCorrectNumberDictionaryElements()
        {
            #region Arrange

            var task = new Task
            {
                Id = 1,
                ProjectId = 1,
            };

            _mockTaskService.Setup(v => v.PendingMergeFromPinOperation(It.IsAny<int>(), It.IsAny<DateTime>()))
                .Returns(false);

            #endregion

            #region Act

            var dictionary = _libraryService.GetSiteCollectionLibraryDictionary(task);

            #endregion

            #region Assert


            Assert.IsNotNull(dictionary);
            Assert.AreEqual(2, dictionary.Count);

            #endregion
        }

        [TestMethod]
        public void LibraryService_GetSiteCollectionLibraryDictionary_ReturnsCorrectKeys()
        {
            #region Arrange

            var task = new Task
            {
                Id = 1,
                ProjectId = 1,
            };

            _mockTaskService.Setup(v => v.PendingMergeFromPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(false);

            #endregion

            #region Act

            var dictionary = _libraryService.GetSiteCollectionLibraryDictionary(task);

            #endregion

            #region Assert


            Assert.IsTrue(dictionary.ContainsKey("DCF-T1"));
            Assert.IsTrue(dictionary.ContainsKey("DCF-T2"));
            Assert.IsFalse(dictionary.ContainsKey("DCF-T3"));

            #endregion
        }

        [TestMethod]
        public void LibraryService_GetSiteCollectionLibraryDictionary_ReturnsExpectedValuesInKeys()
        {
            #region Arrange

            var task = new Task
            {
                Id = 1,
                ProjectId = 1,
            };

            _mockTaskService.Setup(v => v.PendingMergeFromPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(false);

            #endregion

            #region Act

            var dictionary = _libraryService.GetSiteCollectionLibraryDictionary(task);

            #endregion

            #region Assert

            Assert.AreEqual(2, dictionary["DCF-T1"].ToList().Count);
            Assert.AreEqual(1, dictionary["DCF-T2"].ToList().Count);
            Assert.IsTrue(dictionary["DCF-T1"].ToList().Select(x => x.Title).Contains("Library One"));
            Assert.IsTrue(dictionary["DCF-T1"].ToList().Select(x => x.Title).Contains("Library Three"));
            Assert.IsTrue(dictionary["DCF-T2"].ToList().Select(x => x.Title).Contains("Library Four"));

            #endregion
        }

        [TestMethod]
        public void LibraryService_GetSiteCollectionLibraryDictionary_DoesNotReturnsSiteWithPendingMergeOperation()
        {
            #region Arrange

            var task = new Task
            {
                Id = 1,
                ProjectId = 1,
            };

            _mockTaskService.Setup(v => v.PendingMergeFromPinOperation(12345, It.IsAny<DateTime>())).Returns(true);
            _mockTaskService.Setup(v => v.PendingMergeFromPinOperation(12346, It.IsAny<DateTime>())).Returns(false);
            _mockTaskService.Setup(v => v.PendingMergeFromPinOperation(12347, It.IsAny<DateTime>())).Returns(false);
            _mockTaskService.Setup(v => v.PendingMergeFromPinOperation(12348, It.IsAny<DateTime>())).Returns(false);

            #endregion

            #region Act

            var dictionary = _libraryService.GetSiteCollectionLibraryDictionary(task);

            #endregion

            #region Assert

            Assert.IsFalse(dictionary.ContainsKey("DCF-T1"));
            Assert.IsTrue(dictionary.ContainsKey("DCF-T2"));
            Assert.AreEqual(1, dictionary["DCF-T2"].ToList().Count);
            Assert.IsTrue(dictionary["DCF-T2"].ToList().Select(x => x.Title).Contains("Library Four"));

            #endregion
        }
    }
}
