﻿using System;
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
    public class CloseCaseTests
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
                    Title = "Test"
                },
                new Site
                {
                    Id = 1,
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
                    Title = "Library One"
                }
            };

            _mockSiteRepository = MockRepositoryHelper.Create(_sites, (entity, id) => entity.Id == (int)id);
            _mockLibraryRepository = MockRepositoryHelper.Create(_libraries, (entity, id) => entity.Id == (int)id);
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _libraryService = new LibraryService(_mockTaskService.Object, _mockLibraryRepository.Object, _mockUnitOfWork.Object);
            _siteService = new SiteService(_mockSiteRepository.Object, _mockUnitOfWork.Object);

            _operationTaskProcessor = new CloseCase(_mockCaseService.Object, _mockTaskService.Object, _libraryService, _siteService);
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CloseCase_Constructor_NoCaseService()
        {
            #region Arrange

            #endregion

            #region Act

            new CloseCase(
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
        public void CloseCase_Constructor_NoTaskService()
        {
            #region Arrange

            #endregion

            #region Act

            new CloseCase(
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
        public void CloseCase_Constructor_NoLibraryService()
        {
            #region Arrange

            #endregion

            #region Act

            new CloseCase(
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
        public void CloseCase_Constructor_NoSiteService()
        {
            #region Arrange

            #endregion

            #region Act

            new CloseCase(
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
        public void CloseCase_Execute_ExecutesTask()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.CloseCase,
                Pin = 12346,
                ProjectId = 12345,
                CaseId = 12345,
                Dictionary = ValidDictionaryXml
            };

            _mockTaskService.Setup(s => s.PendingMergeFromPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(false);

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CloseCase_Execute_TaskThrowsException_ExceptionIsPropogated()
        {
            _mockTaskService.Setup(s => s.PendingMergeFromPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(false);
            _mockCaseService.Setup(s => s.CloseCase(It.IsAny<Task>())).Throws(new Exception());

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.CloseCase,
                Pin = 12346,
                ProjectId = 12345,
                CaseId = 12345,
                Dictionary = ValidDictionaryXml
            };

            _operationTaskProcessor.Execute(task);

            _mockCaseService.Verify(v => v.AllocateCase(It.IsAny<Task>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(UnRecoverableErrorException))]
        public void CloseCase_Execute_NoPinThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.CloseCase,
                ProjectId = 12345,
                CaseId = 12345,
                IsPrimary = true,
                Dictionary = ValidDictionaryXml
            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void CloseCase_Execute_NoPinThrowsExceptionWithInvalidRequestNoPinExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.CloseCase,
                ProjectId = 12345,
                CaseId = 12345,
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
        public void CloseCase_Execute_NoPinThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.CloseCase,
                ProjectId = 12345,
                CaseId = 12345,
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
        public void CloseCase_Execute_NoProjectIdThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.CloseCase,
                Pin = 12345,
                CaseId = 12345,
                Dictionary = ValidDictionaryXml
            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void CloseCase_Execute_NoProjectIdThrowsExceptionWithInvalidRequestNoProjectIdExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.CloseCase,
                Pin = 12345,
                CaseId = 12345,
                Dictionary = ValidDictionaryXml
            };

            var expectedMessage = string.Format(TaskResources.OperationsTaskRequest_InvalidRequestNoProjectId, task.Name, task.Pin.Value);

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
        public void CloseCase_Execute_NoProjectIdThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.CloseCase,
                Pin = 12345,
                CaseId = 12345,
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
        public void CloseCase_Execute_NoCaseIdThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.CloseCase,
                Pin = 12345,
                ProjectId = 12345,
                Dictionary = ValidDictionaryXml
            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void CloseCase_Execute_NoCaseIdThrowsExceptionWithInvalidRequestNoCaseIdExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.CloseCase,
                Pin = 12345,
                ProjectId = 12345,
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
        public void CloseCase_Execute_NoCaseIdThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.CloseCase,
                Pin = 12345,
                ProjectId = 12345,
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
        public void CloseCase_Execute_NoDictionaryXmlThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.CloseCase,
                Pin = 12345,
                ProjectId = 12345,
                CaseId = 12345

            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void CloseCase_Execute_NoDictionaryXmlThrowsExceptionWithInvalidRequestNoDictionaryXmlExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.CloseCase,
                Pin = 12345,
                ProjectId = 12345,
                CaseId = 12345

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
        public void CloseCase_Execute_NoDictionaryXmlThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.CloseCase,
                Pin = 12345,
                ProjectId = 12345,
                CaseId = 12345
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
        public void CloseCase_Execute_InvalidXmlThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.CloseCase,
                Pin = 12345,
                ProjectId = 12345,
                CaseId = 54321,
                Dictionary = "<items><item>hello</item></items>"
            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void CloseCase_Execute_InvalidXmlThrowsExceptionWithInvalidRequestInvalidDictionaryXMLExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.CloseCase,
                Pin = 12345,
                ProjectId = 12345,
                CaseId = 54321,
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
        public void CloseCase_Execute_InvalidXmlThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.CloseCase,
                Pin = 12345,
                ProjectId = 12345,
                CaseId = 54321,
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
        public void CloseCase_Execute_PinDoesNotExistsThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.CloseCase,
                Pin = 12345,
                ProjectId = 12345,
                CaseId = 54321,
                Dictionary = ValidDictionaryXml
            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void CloseCase_Execute_PinDoesNotExistsThrowsExceptionWithInvalidRequestPinDoesNotExisExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.CloseCase,
                Pin = 12345,
                ProjectId = 12345,
                CaseId = 54321,
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
        public void CloseCase_Execute_PinDoesNotExistThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.CloseCase,
                Pin = 12345,
                ProjectId = 12345,
                CaseId = 54321,
                CaseTitle = "An Example Case Title",
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
        public void CloseCase_Execute_CaseIdDoesNotExistsThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.CloseCase,
                Pin = 12346,
                ProjectId = 11111,
                CaseId = 123456,
                Dictionary = ValidDictionaryXml
            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void CloseCase_Execute_CaseIdDoesNotExistsThrowsExceptionWithInvalidRequestNoCaseIdExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.CloseCase,
                Pin = 12346,
                ProjectId = 11111,
                CaseId = 123456,
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
        public void CloseCase_Execute_CaseIdDoesNotExistThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.CloseCase,
                Pin = 12346,
                ProjectId = 11111,
                CaseId = 123456,
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
        public void CloseCase_Execute_PinDoesNotExistForTheSpecifiedCaseThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.CloseCase,
                Pin = 12347,
                ProjectId = 11111,
                CaseId = 123456,
                Dictionary = ValidDictionaryXml
            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void CloseCase_Execute_PinDoesNotExistForTheSpecifiedCaseThrowsExceptionWithInvalidRequestPinDoesNotExistForTheSpecifiedCaseExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.CloseCase,
                Pin = 12347,
                ProjectId = 11111,
                CaseId = 12345,
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
        public void CloseCase_Execute_PinDoesNotExistForTheSpecifiedCaseThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.CloseCase,
                Pin = 12347,
                ProjectId = 11111,
                CaseId = 12345,
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
        public void CloseCase_Execute_PendingMergePinOperationThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.CloseCase,
                Pin = 12346,
                ProjectId = 12345,
                CaseId = 12346,
                Dictionary = ValidDictionaryXml
            };

            _mockTaskService.Setup(s => s.PendingMergeFromPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(true);

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void CloseCase_Execute_PendingMergePinOperationThrowsExceptionWithInvalidRequestMergePinRequestPendingExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.CloseCase,
                Pin = 12346,
                ProjectId = 12345,
                CaseId = 12345,
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
        public void CloseCase_Execute_PendingMergePinOperationThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.CloseCase,
                Pin = 12346,
                ProjectId = 12345,
                CaseId = 12346,
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
