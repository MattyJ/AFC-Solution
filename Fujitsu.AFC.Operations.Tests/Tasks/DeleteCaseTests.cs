using System;
using System.Collections.Generic;
using System.Linq;
using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Data.Interfaces;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Operations.Interfaces;
using Fujitsu.AFC.Operations.Tasks;
using Fujitsu.AFC.Services;
using Fujitsu.AFC.Services.Interfaces;
using Fujitsu.AFC.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fujitsu.AFC.Operations.Tests.Tasks
{
    [TestClass]
    public class DeleteCaseTests
    {
        private Mock<ICaseService> _mockCaseService;
        private Mock<ITaskService> _mockTaskService;
        private Mock<ILibraryService> _mockLibraryService;
        private Mock<IParameterService> _mockParameterService;

        private IOperationsTaskProcessor _operationTaskProcessor;
        private ILibraryService _libraryService;

        private Mock<IRepository<Site>> _mockSiteRepository;
        private Mock<IRepository<Library>> _mockLibraryRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;

        private List<Site> _sites;
        private List<Library> _libraries;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockCaseService = new Mock<ICaseService>();
            _mockTaskService = new Mock<ITaskService>();
            _mockLibraryService = new Mock<ILibraryService>();
            _mockParameterService = new Mock<IParameterService>();

            _sites = new List<Site>
            {
                new Site
                {
                    Id = 1,
                    Pin = 12346,
                    Title = "Test"
                },
                new Site
                {
                    Id = 2,
                    Pin = 12347,
                    Title = "Test"
                }
            };

            _libraries = new List<Library>
            {
                new Library
                {
                    Id = 1,
                    SiteId = 1,
                    Site = _sites.First(x => x.Id == 1),
                    CaseId = 12345,
                    ProjectId = 1,
                    Title = "Library One",
                    IsClosed = true,
                    UpdatedDate = DateTime.Now.AddYears(-26)
                },
                new Library
                {
                    Id = 2,
                    SiteId = 2,
                    Site = _sites.First(x => x.Id == 2),
                    CaseId = 12346,
                    ProjectId = 1,
                    Title = "Library Two",
                    IsClosed = true,
                    UpdatedDate = DateTime.Now.AddYears(-27)
                },
                new Library
                {
                    Id = 3,
                    SiteId = 2,
                    Site = _sites.First(x => x.Id == 2),
                    CaseId = 12347,
                    ProjectId = 1,
                    Title = "Library Three",
                    IsClosed = false,
                    UpdatedDate = DateTime.Now
                },
                new Library
                {
                    Id = 4,
                    SiteId = 2,
                    Site = _sites.First(x => x.Id == 2),
                    CaseId = 12346,
                    ProjectId = 1,
                    Title = "Library Four",
                    IsClosed = true,
                    UpdatedDate = DateTime.Now.AddYears(5)
                }
            };

            _mockSiteRepository = MockRepositoryHelper.Create(_sites, (entity, id) => entity.Id == (int)id);
            _mockLibraryRepository = MockRepositoryHelper.Create(_libraries, (entity, id) => entity.Id == (int)id);
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _libraryService = new LibraryService(_mockTaskService.Object, _mockLibraryRepository.Object, _mockUnitOfWork.Object);

            _operationTaskProcessor = new DeleteCase(_mockCaseService.Object, _libraryService, _mockParameterService.Object);
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DeleteCase_Constructor_NoCaseService()
        {
            #region Arrange

            #endregion

            #region Act

            new DeleteCase(
                null,
                _mockLibraryService.Object,
                _mockParameterService.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DeleteCase_Constructor_NoLibraryService()
        {
            #region Arrange

            #endregion

            #region Act

            new DeleteCase(
                _mockCaseService.Object,
                null,
                _mockParameterService.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DeleteCase_Constructor_NoParameterService()
        {
            #region Arrange

            #endregion

            #region Act

            new DeleteCase(
                _mockCaseService.Object,
                _mockLibraryService.Object,
                null);

            #endregion

            #region Assert

            #endregion
        }

        #endregion

        [TestMethod]
        public void DeleteCase_Execute_ExecutesTask()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.DeleteCase
            };

            _mockParameterService.Setup(s => s.GetParameterByNameAndCache<int>(ParameterNames.DigitialCaseFileRetentionPeriodInWeeks)).Returns(1300);

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void DeleteCase_Execute_TaskThrowsException_ExceptionIsPropogated()
        {
            _mockCaseService.Setup(s => s.DeleteCase(It.IsAny<Task>())).Throws(new Exception());

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.DeleteCase
            };

            _mockParameterService.Setup(s => s.GetParameterByNameAndCache<int>(ParameterNames.DigitialCaseFileRetentionPeriodInWeeks)).Returns(1300);

            _operationTaskProcessor.Execute(task);

            _mockCaseService.Verify(v => v.DeleteCase(It.IsAny<Task>()), Times.Once);
        }

        [TestMethod]
        public void DeleteCase_Execute_CallDeleteCaseTwice()
        {

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.DeleteCase
            };

            _mockParameterService.Setup(s => s.GetParameterByNameAndCache<int>(ParameterNames.DigitialCaseFileRetentionPeriodInWeeks)).Returns(1300);

            _operationTaskProcessor.Execute(task);

            _mockCaseService.Verify(v => v.DeleteCase(It.IsAny<Task>()), Times.Exactly(2));
        }
    }
}
