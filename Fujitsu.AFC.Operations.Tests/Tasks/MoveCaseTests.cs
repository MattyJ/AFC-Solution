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
using Fujitsu.AFC.Tasks.Resources;
using Fujitsu.AFC.UnitTesting;
using Fujitsu.Exceptions.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fujitsu.AFC.Operations.Tests.Tasks
{
    [TestClass]
    public class MoveCaseTests
    {
        private Mock<ICaseService> _mockCaseService;
        private Mock<ITaskService> _mockTaskService;
        private Mock<ILibraryService> _mockLibraryService;
        private Mock<IService<Site>> _mockSiteService;

        private IOperationsTaskProcessor _operationTaskProcessor;
        private IService<Site> _siteService;
        private ILibraryService _libraryService;

        private Mock<IRepository<Site>> _mockSiteRepository;
        private Mock<IRepository<Library>> _mockLibraryRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;

        private List<Site> _sites;
        private List<Library> _libraries;

        private const string ValidDictionaryXml = "<items><item><key>Service Type</key><value>Early Years</value></item><item><key>Service User Pin</key><value>987876678</value></item></items>";

        [TestInitialize]
        public void TestInitialize()
        {
            _mockCaseService = new Mock<ICaseService>();
            _mockTaskService = new Mock<ITaskService>();
            _mockSiteService = new Mock<IService<Site>>();
            _mockLibraryService = new Mock<ILibraryService>();

            _sites = new List<Site>
            {
                new Site
                {
                    Id = 1,
                    Pin = 12346,
                    Title = "Test One"
                },
                new Site
                {
                    Id = 2,
                    Pin = 12347,
                    Title = "Test Two"
                },
                new Site
                {
                    Id = 3,
                    Pin = 12348,
                    Title = "Test Three"
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
                    Title = "Library One"
                },
                new Library
                {
                    Id = 2,
                    SiteId = 2,
                    Site = _sites.First(x => x.Id == 2),
                    CaseId = 12348,
                    ProjectId = 2,
                    Title = "Library Two"
                },
                new Library
                {
                    Id = 3,
                    SiteId = 3,
                    Site = _sites.First(x => x.Id == 3),
                    CaseId = 12349,
                    ProjectId = 3,
                    Title = "Library Three"
                }
            };

            _mockSiteRepository = MockRepositoryHelper.Create(_sites, (entity, id) => entity.Id == (int)id);
            _mockLibraryRepository = MockRepositoryHelper.Create(_libraries, (entity, id) => entity.Id == (int)id);
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _libraryService = new LibraryService(_mockTaskService.Object, _mockLibraryRepository.Object, _mockUnitOfWork.Object);
            _siteService = new SiteService(_mockSiteRepository.Object, _mockUnitOfWork.Object);

            _operationTaskProcessor = new MoveCase(_mockCaseService.Object, _mockTaskService.Object, _libraryService, _siteService);
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MoveCase_Constructor_NoCaseService()
        {
            #region Arrange

            #endregion

            #region Act

            new MoveCase(
                null,
                _mockTaskService.Object,
                _mockLibraryService.Object,
                _mockSiteService.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MoveCase_Constructor_NoTaskService()
        {
            #region Arrange

            #endregion

            #region Act

            new MoveCase(
                _mockCaseService.Object,
                null,
                _mockLibraryService.Object,
                _mockSiteService.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MoveCase_Constructor_NoLibraryService()
        {
            #region Arrange

            #endregion

            #region Act

            new MoveCase(
                _mockCaseService.Object,
                _mockTaskService.Object,
                null,
                _mockSiteService.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MoveCase_Constructor_NoSiteService()
        {
            #region Arrange

            #endregion

            #region Act

            new MoveCase(
                _mockCaseService.Object,
                _mockTaskService.Object,
                _mockLibraryService.Object,
                null);

            #endregion

            #region Assert

            #endregion
        }

        #endregion

        [TestMethod]
        public void MoveCase_Execute_ExecutesTask()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                Pin = 12346,
                CaseId = 12345,
                CurrentProjectId = 1,
                NewProjectId = 22222,
                IsPrimary = true,
                Dictionary = ValidDictionaryXml
            };

            _mockTaskService.Setup(s => s.PendingMergeFromPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(false);

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void MoveCase_Execute_TaskThrowsException_ExceptionIsPropogated()
        {
            _mockTaskService.Setup(s => s.PendingMergeFromPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(false);
            _mockCaseService.Setup(s => s.MoveCase(It.IsAny<Task>())).Throws(new Exception());

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                Pin = 12346,
                CaseId = 12345,
                CurrentProjectId = 1,
                NewProjectId = 22222,
                IsPrimary = true,
                Dictionary = ValidDictionaryXml
            };

            _operationTaskProcessor.Execute(task);

            _mockCaseService.Verify(v => v.MoveCase(It.IsAny<Task>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(UnRecoverableErrorException))]
        public void MoveCase_Execute_NoPinThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                CaseId = 12345,
                CurrentProjectId = 1,
                NewProjectId = 22222,
                IsPrimary = true,
                Dictionary = ValidDictionaryXml
            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void MoveCase_Execute_NoPinThrowsExceptionWithInvalidRequestNoPinExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                CaseId = 12345,
                CurrentProjectId = 1,
                NewProjectId = 22222,
                IsPrimary = true,
                Dictionary = ValidDictionaryXml
            };

            var expectedMessage = string.Format(TaskResources.OperationsTaskRequest_InvalidRequestNoPin, task.Name);

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException ex)
            {
                message = ex.Message;
            }

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void MoveCase_Execute_NoPinThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                CaseId = 12345,
                CurrentProjectId = 1,
                NewProjectId = 22222,
                IsPrimary = true,
                Dictionary = ValidDictionaryXml
            };

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException)
            {

            }

            _mockTaskService.Verify(v => v.CompleteUnrecoverableTaskException(It.IsAny<Task>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(UnRecoverableErrorException))]
        public void MoveCase_Execute_NoCaseIdThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                Pin = 12345,
                CurrentProjectId = 1,
                NewProjectId = 22222,
                IsPrimary = true,
                Dictionary = ValidDictionaryXml
            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void MoveCase_Execute_NoCaseIdThrowsExceptionWithInvalidRequestNoCaseIdExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                Pin = 12345,
                CurrentProjectId = 1,
                NewProjectId = 22222,
                IsPrimary = true,
                Dictionary = ValidDictionaryXml
            };

            var expectedMessage = string.Format(TaskResources.OperationsTaskRequest_InvalidRequestNoCaseId, task.Name, task.Pin.Value);

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException ex)
            {
                message = ex.Message;
            }

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void MoveCase_Execute_NoCaseIdThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                Pin = 12345,
                CurrentProjectId = 1,
                NewProjectId = 22222,
                IsPrimary = true,
                Dictionary = ValidDictionaryXml
            };

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException)
            {

            }

            _mockTaskService.Verify(v => v.CompleteUnrecoverableTaskException(It.IsAny<Task>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(UnRecoverableErrorException))]
        public void MoveCase_Execute_NoCurrentProjectIdThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                Pin = 12345,
                CaseId = 1,
                NewProjectId = 22222,
                IsPrimary = true,
                Dictionary = ValidDictionaryXml
            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void MoveCase_Execute_NoCurrentProjectIdThrowsExceptionWithInvalidRequestNoCurrentProjectIdExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                Pin = 12345,
                CaseId = 1,
                NewProjectId = 22222,
                IsPrimary = true,
                Dictionary = ValidDictionaryXml
            };

            var expectedMessage = string.Format(TaskResources.OperationsTaskRequest_InvalidRequestNoCurrentProjectId, task.Name, task.Pin.Value);

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException ex)
            {
                message = ex.Message;
            }

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void MoveCase_Execute_NoCurrentProjectIdThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                Pin = 12345,
                CaseId = 1,
                NewProjectId = 22222,
                IsPrimary = true,
                Dictionary = ValidDictionaryXml
            };

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException)
            {

            }

            _mockTaskService.Verify(v => v.CompleteUnrecoverableTaskException(It.IsAny<Task>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(UnRecoverableErrorException))]
        public void MoveCase_Execute_NoNewProjectIdThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                Pin = 12345,
                CaseId = 12345,
                CurrentProjectId = 12345,
                IsPrimary = true,
                Dictionary = ValidDictionaryXml
            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void MoveCase_Execute_NoNewProjectIdThrowsExceptionWithInvalidRequestNoCurrentProjectIdExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                Pin = 12345,
                CaseId = 12345,
                CurrentProjectId = 12345,
                Dictionary = ValidDictionaryXml
            };

            var expectedMessage = string.Format(TaskResources.OperationsTaskRequest_InvalidRequestNoNewProjectId, task.Name, task.Pin.Value);

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException ex)
            {
                message = ex.Message;
            }

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void MoveCase_Execute_NoNewProjectIdThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                Pin = 12345,
                CaseId = 12345,
                CurrentProjectId = 12345,
                Dictionary = ValidDictionaryXml
            };

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException)
            {

            }

            _mockTaskService.Verify(v => v.CompleteUnrecoverableTaskException(It.IsAny<Task>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(UnRecoverableErrorException))]
        public void MoveCase_Execute_NoIsPrimaryThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                Pin = 12346,
                CaseId = 12345,
                CurrentProjectId = 1,
                NewProjectId = 22222,
                Dictionary = ValidDictionaryXml
            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void MoveCase_Execute_NoIsPrimaryThrowsExceptionWithInvalidRequestNoIsPrimaryExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                Pin = 12346,
                CaseId = 12345,
                CurrentProjectId = 1,
                NewProjectId = 22222,
                Dictionary = ValidDictionaryXml
            };

            var expectedMessage = string.Format(TaskResources.OperationsTaskRequest_InvalidRequestNoIsPrimary, task.Name, task.Pin.Value, task.CaseId.Value);

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException ex)
            {
                message = ex.Message;
            }

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void MoveCase_Execute_NoIsPrimaryThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                Pin = 12346,
                CaseId = 12345,
                CurrentProjectId = 1,
                NewProjectId = 22222,
                Dictionary = ValidDictionaryXml
            };

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException)
            {

            }

            _mockTaskService.Verify(v => v.CompleteUnrecoverableTaskException(It.IsAny<Task>(), It.IsAny<string>()), Times.Once);
        }


        [TestMethod]
        [ExpectedException(typeof(UnRecoverableErrorException))]
        public void MoveCase_Execute_NoDictionaryXmlThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                Pin = 12346,
                CaseId = 12345,
                CurrentProjectId = 1,
                NewProjectId = 22222,
                IsPrimary = true

            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void MoveCase_Execute_NoDictionaryXmlThrowsExceptionWithInvalidRequestNoDictionaryXmlExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                Pin = 12346,
                CaseId = 12345,
                CurrentProjectId = 1,
                NewProjectId = 22222,
                IsPrimary = true

            };

            var expectedMessage = string.Format(TaskResources.OperationsTaskRequest_InvalidRequestNoDictionaryXML, task.Name);

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException ex)
            {
                message = ex.Message;
            }

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void MoveCase_Execute_NoDictionaryXmlThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                Pin = 12346,
                CaseId = 12345,
                CurrentProjectId = 1,
                NewProjectId = 22222,
                IsPrimary = true
            };

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException)
            {

            }

            _mockTaskService.Verify(v => v.CompleteUnrecoverableTaskException(It.IsAny<Task>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(UnRecoverableErrorException))]
        public void MoveCase_Execute_InvalidXmlThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                Pin = 12346,
                CaseId = 12345,
                CurrentProjectId = 1,
                NewProjectId = 22222,
                IsPrimary = true,
                Dictionary = "<items><item>hello</item></items>"
            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void MoveCase_Execute_InvalidXmlThrowsExceptionWithInvalidRequestInvalidDictionaryXMLExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                Pin = 12346,
                CaseId = 12345,
                CurrentProjectId = 1,
                NewProjectId = 22222,
                IsPrimary = true,
                Dictionary = "<items><item>hello</item></items>"
            };

            var expectedMessage = string.Format(TaskResources.OperationsTaskRequest_InvalidDictionaryXML, task.Name, task.Pin.Value);

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException ex)
            {
                message = ex.Message;
            }

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void MoveCase_Execute_InvalidXmlThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                Pin = 12346,
                CaseId = 12345,
                CurrentProjectId = 1,
                NewProjectId = 22222,
                IsPrimary = true,
                Dictionary = "<items><item>hello</item></items>"
            };

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException)
            {

            }

            _mockTaskService.Verify(v => v.CompleteUnrecoverableTaskException(It.IsAny<Task>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(UnRecoverableErrorException))]
        public void MoveCase_Execute_PinDoesNotExistsThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                Pin = 123461,
                CaseId = 12345,
                CurrentProjectId = 1,
                NewProjectId = 22222,
                IsPrimary = true,
                Dictionary = ValidDictionaryXml
            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void MoveCase_Execute_PinDoesNotExistsThrowsExceptionWithInvalidRequestPinDoesNotExisExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                Pin = 1234611,
                CaseId = 12345,
                CurrentProjectId = 1,
                NewProjectId = 22222,
                IsPrimary = true,
                Dictionary = ValidDictionaryXml
            };

            var expectedMessage = string.Format(TaskResources.OperationsTaskRequest_PinDoesNotExist, task.Name, task.Pin.Value);

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException ex)
            {
                message = ex.Message;
            }

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void MoveCase_Execute_PinDoesNotExistThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                Pin = 1234611,
                CaseId = 12345,
                CurrentProjectId = 1,
                NewProjectId = 22222,
                IsPrimary = true,
                Dictionary = ValidDictionaryXml
            };

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException)
            {

            }

            _mockTaskService.Verify(v => v.CompleteUnrecoverableTaskException(It.IsAny<Task>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(UnRecoverableErrorException))]
        public void MoveCase_Execute_PinDoesNotExistForTheSpecifiedCaseThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                Pin = 12347,
                CaseId = 12345,
                CurrentProjectId = 1,
                NewProjectId = 22222,
                IsPrimary = true,
                Dictionary = ValidDictionaryXml
            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void MoveCase_Execute_PinDoesNotExistForTheSpecifiedCaseThrowsExceptionWithInvalidRequestPinDoesNotExistForTheSpecifiedCaseExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                Pin = 12347,
                CaseId = 12345,
                CurrentProjectId = 1,
                NewProjectId = 22222,
                IsPrimary = true,
                Dictionary = ValidDictionaryXml
            };

            var expectedMessage = string.Format(TaskResources.OperationsTaskRequest_PinDoesNotExistForTheSpecifiedCase, task.Name, task.Pin.Value, task.CaseId.Value);

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException ex)
            {
                message = ex.Message;
            }

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void MoveCase_Execute_PinDoesNotExistForTheSpecifiedCaseThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                Pin = 12347,
                CaseId = 12345,
                CurrentProjectId = 1,
                NewProjectId = 22222,
                IsPrimary = true,
                Dictionary = ValidDictionaryXml
            };

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException)
            {

            }

            _mockTaskService.Verify(v => v.CompleteUnrecoverableTaskException(It.IsAny<Task>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(UnRecoverableErrorException))]
        public void MoveCase_Execute_CaseIdDoesNotExistsThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                Pin = 12346,
                CaseId = 12348,
                CurrentProjectId = 1,
                NewProjectId = 22222,
                IsPrimary = true,
                Dictionary = ValidDictionaryXml
            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void MoveCase_Execute_CaseIdDoesNotExistsThrowsExceptionWithInvalidRequestNoCaseIdExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                Pin = 12346,
                CaseId = 1234511,
                CurrentProjectId = 1,
                NewProjectId = 22222,
                IsPrimary = true,
                Dictionary = ValidDictionaryXml
            };

            var expectedMessage = string.Format(TaskResources.OperationsTaskRequest_CaseIdDoesNotExist, task.Name, task.Pin.Value, task.CaseId.Value);

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException ex)
            {
                message = ex.Message;
            }

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void MoveCase_Execute_CaseIdDoesNotExistThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                Pin = 12346,
                CaseId = 12348,
                CurrentProjectId = 1,
                NewProjectId = 22222,
                IsPrimary = true,
                Dictionary = ValidDictionaryXml

            };

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException)
            {

            }

            _mockTaskService.Verify(v => v.CompleteUnrecoverableTaskException(It.IsAny<Task>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(UnRecoverableErrorException))]
        public void MoveCase_Execute_CurrentProjectIsNotAssignedToTheCaseCurrentProjectIdThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                Pin = 12346,
                CaseId = 12345,
                CurrentProjectId = 3,
                NewProjectId = 22222,
                IsPrimary = true,
                Dictionary = ValidDictionaryXml
            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void MoveCase_Execute_CurrentProjectIsNotAssignedToTheCaseThrowsExceptionWithOperationsTaskRequest_CurrentProjectIsNotAssignedToTheCaseExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                Pin = 12346,
                CaseId = 12345,
                CurrentProjectId = 3,
                NewProjectId = 4,
                Dictionary = ValidDictionaryXml,
                IsPrimary = true
            };

            var expectedMessage = string.Format(TaskResources.OperationsTaskRequest_CurrentProjectIsNotAssignedToTheCase, task.Name, task.CurrentProjectId.Value, task.CaseId.Value);

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException ex)
            {
                message = ex.Message;
            }

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void MoveCase_Execute_CurrentProjectIsNotAssignedToTheCaseThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                Pin = 12346,
                CaseId = 12345,
                CurrentProjectId = 3,
                NewProjectId = 12345,
                Dictionary = ValidDictionaryXml,
                IsPrimary = true
            };

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException)
            {

            }

            _mockTaskService.Verify(v => v.CompleteUnrecoverableTaskException(It.IsAny<Task>(), It.IsAny<string>()), Times.Once);
        }


        [TestMethod]
        [ExpectedException(typeof(UnRecoverableErrorException))]
        public void MoveCase_Execute_PendingMergePinOperationThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                Pin = 12346,
                CaseId = 12345,
                CurrentProjectId = 1,
                NewProjectId = 22222,
                IsPrimary = true,
                Dictionary = ValidDictionaryXml
            };

            _mockTaskService.Setup(s => s.PendingMergeFromPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(true);

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void MoveCase_Execute_PendingMergePinOperationThrowsExceptionWithInvalidRequestMergePinRequestPendingExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                Pin = 12346,
                CaseId = 12345,
                CurrentProjectId = 1,
                NewProjectId = 22222,
                IsPrimary = true,
                Dictionary = ValidDictionaryXml
            };

            var expectedMessage = string.Format(TaskResources.OperationsTaskRequest_MergePinRequestPending, task.Name, task.Pin.Value);

            _mockTaskService.Setup(s => s.PendingMergeFromPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(true);

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException ex)
            {
                message = ex.Message;
            }

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void MoveCase_Execute_PendingMergePinOperationThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.MoveCase,
                Pin = 12346,
                CaseId = 12345,
                CurrentProjectId = 1,
                NewProjectId = 22222,
                IsPrimary = true,
                Dictionary = ValidDictionaryXml
            };

            _mockTaskService.Setup(s => s.PendingMergeFromPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(true);

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException)
            {

            }

            _mockTaskService.Verify(v => v.CompleteUnrecoverableTaskException(It.IsAny<Task>(), It.IsAny<string>()), Times.Once);
        }
    }
}
