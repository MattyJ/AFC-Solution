using System;
using System.Collections.Generic;
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
    public class DeletePinTests
    {
        private Mock<IPinService> _mockPinService;
        private Mock<ITaskService> _mockTaskService;
        private Mock<IService<Site>> _mockSiteService;

        private IService<Site> _siteService;
        private Mock<IRepository<Site>> _mockSiteRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;

        private List<Site> _sites;

        private IOperationsTaskProcessor _operationTaskProcessor;

        [TestInitialize]
        public void TestInitialize()
        {

            _mockPinService = new Mock<IPinService>();
            _mockTaskService = new Mock<ITaskService>();
            _mockSiteService = new Mock<IService<Site>>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _sites = new List<Site>
            {
                new Site
                {
                    Id = 1,
                    Pin = 12346,
                    Title = "Test"
                }
            };

            _mockSiteRepository = MockRepositoryHelper.Create(_sites, (entity, id) => entity.Id == (int)id);
            _siteService = new SiteService(_mockSiteRepository.Object, _mockUnitOfWork.Object);

            _operationTaskProcessor = new DeletePin(_mockPinService.Object, _mockTaskService.Object, _siteService);
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DeletePin_Constructor_NoPinService()
        {
            #region Arrange

            #endregion

            #region Act

            new DeletePin(
                null,
                _mockTaskService.Object,
                _mockSiteService.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DeletePin_Constructor_NoTaskService()
        {
            #region Arrange

            #endregion

            #region Act

            new DeletePin(
                _mockPinService.Object,
                null,
                _mockSiteService.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DeletePin_Constructor_NoSiteService()
        {
            #region Arrange

            #endregion

            #region Act

            new DeletePin(
                _mockPinService.Object,
                _mockTaskService.Object,
                null);

            #endregion

            #region Assert

            #endregion
        }

        #endregion

        [TestMethod]
        public void DeletePin_Execute_ValidTaskPassesPreLimChecksAndExecutesTask()
        {
            _mockTaskService.Setup(s => s.PendingMergeToPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(false);
            _mockTaskService.Setup(s => s.PendingMergeFromPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(false);

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.DeletePin,
                Pin = 12346
            };

            _operationTaskProcessor.Execute(task);

            _mockPinService.Verify(v => v.DeletePin(It.IsAny<Task>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void DeletePin_Execute_TaskThrowsException_ExceptionIsPropogated()
        {
            _mockTaskService.Setup(s => s.PendingMergeToPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(false);
            _mockTaskService.Setup(s => s.PendingMergeFromPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(false);
            _mockPinService.Setup(s => s.DeletePin(It.IsAny<Task>())).Throws(new Exception());

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.DeletePin,
                Pin = 12346
            };

            _operationTaskProcessor.Execute(task);

            _mockPinService.Verify(v => v.DeletePin(It.IsAny<Task>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(UnRecoverableErrorException))]
        public void DeletePin_Execute_NoPinThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.DeletePin
            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void DeletePin_Execute_NoPinThrowsExceptionWithInvalidRequestNoPinExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.DeletePin,
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
        public void DeletePin_Execute_NoPinThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.DeletePin,
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
        public void DeletePin_Execute_PinDoesNotExistsThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.DeletePin,
                Pin = 12345
            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void DeletePin_Execute_PinDoesNotExistsThrowsExceptionWithInvalidRequestPinDoesNotExistExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.DeletePin,
                Pin = 12345
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
        public void DeletePin_Execute_PinDoesNotExistThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.DeletePin,
                Pin = 12345
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
        public void DeletePin_Execute_PendingMergeFromPinOperationThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.DeletePin,
                Pin = 12346
            };

            _mockTaskService.Setup(s => s.PendingMergeFromPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(true);
            _mockTaskService.Setup(s => s.PendingMergeToPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(false);

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void DeletePin_Execute_PendingMergeFromPinOperationThrowsExceptionWithInvalidRequestMergePinRequestPendingExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.DeletePin,
                Pin = 12346
            };

            _mockTaskService.Setup(s => s.PendingMergeFromPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(true);
            _mockTaskService.Setup(s => s.PendingMergeToPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(false);

            var expectedMessage = string.Format(TaskResources.OperationsTaskRequest_MergePinRequestPending, task.Name, task.Pin.Value);

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
        public void DeletePin_Execute_PendingMergeFromPinOperationThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.DeletePin,
                Pin = 12346
            };

            _mockTaskService.Setup(s => s.PendingMergeFromPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(true);
            _mockTaskService.Setup(s => s.PendingMergeToPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(false);

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
        public void DeletePin_Execute_PendingMergeToPinOperationThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.DeletePin,
                Pin = 12346
            };

            _mockTaskService.Setup(s => s.PendingMergeToPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(true);
            _mockTaskService.Setup(s => s.PendingMergeFromPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(false);

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void DeletePin_Execute_PendingMergeToPinOperationThrowsExceptionWithInvalidRequestMergePinRequestPendingExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.DeletePin,
                Pin = 12346
            };

            _mockTaskService.Setup(s => s.PendingMergeToPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(true);
            _mockTaskService.Setup(s => s.PendingMergeFromPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(false);

            var expectedMessage = string.Format(TaskResources.OperationsTaskRequest_MergePinRequestPending, task.Name, task.Pin.Value);

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
        public void DeletePin_Execute_PendingMergeToPinOperationThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.DeletePin,
                Pin = 12346
            };

            _mockTaskService.Setup(s => s.PendingMergeToPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(true);
            _mockTaskService.Setup(s => s.PendingMergeFromPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(false);

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
