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
    public class RestrictUserTests
    {
        private Mock<IUserService> _mockUserService;
        private Mock<ITaskService> _mockTaskService;
        private Mock<IService<Site>> _mockSiteService;

        private IOperationsTaskProcessor _operationTaskProcessor;
        private IService<Site> _siteService;

        private Mock<IRepository<Site>> _mockSiteRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;

        private List<Site> _sites;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockUserService = new Mock<IUserService>();
            _mockTaskService = new Mock<ITaskService>();
            _mockSiteService = new Mock<IService<Site>>();

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

            _mockSiteRepository = MockRepositoryHelper.Create(_sites, (entity, id) => entity.Id == (int)id);
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _siteService = new SiteService(_mockSiteRepository.Object, _mockUnitOfWork.Object);

            _operationTaskProcessor = new RestrictUser(_mockUserService.Object, _mockTaskService.Object, _siteService);
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RestrictUser_Constructor_NoUserService()
        {
            #region Arrange

            #endregion

            #region Act

            new RestrictUser(
                null,
                _mockTaskService.Object,
                _mockSiteService.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RestrictUser_Constructor_NoTaskService()
        {
            #region Arrange

            #endregion

            #region Act

            new RestrictUser(
                _mockUserService.Object,
                null,
                _mockSiteService.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RestrictUser_Constructor_NoSiteService()
        {
            #region Arrange

            #endregion

            #region Act

            new RestrictUser(
                _mockUserService.Object,
                _mockTaskService.Object,
                null);

            #endregion

            #region Assert

            #endregion
        }

        #endregion

        [TestMethod]
        public void RestrictUser_Execute_ExecutesTask()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.RestrictUser,
                Pin = 12346
            };

            _mockTaskService.Setup(s => s.PendingMergeFromPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(false);

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void RestrictUser_Execute_TaskThrowsException_ExceptionIsPropogated()
        {
            _mockTaskService.Setup(s => s.PendingMergeFromPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(false);
            _mockUserService.Setup(s => s.RestictUser(It.IsAny<Task>())).Throws(new Exception());

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.RestrictUser,
                Pin = 12346
            };

            _operationTaskProcessor.Execute(task);

            _mockUserService.Verify(v => v.RestictUser(It.IsAny<Task>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(UnRecoverableErrorException))]
        public void RestrictUser_Execute_NoPinThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.RestrictUser
            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void RestrictUser_Execute_NoPinThrowsExceptionWithInvalidRequestNoPinExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.RestrictUser
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
        public void RestrictUser_Execute_NoPinThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.RestrictUser
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
        public void RestrictUser_Execute_PinDoesNotExistsThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.RestrictUser,
                Pin = 12399
            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void RestrictUser_Execute_PinDoesNotExistsThrowsExceptionWithInvalidRequestPinDoesNotExisExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.RestrictUser,
                Pin = 12399
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
        public void RestrictUser_Execute_PinDoesNotExistThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.RestrictUser,
                Pin = 12399
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
        public void RestrictUser_Execute_PendingMergePinOperationThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.RestrictUser,
                Pin = 12346
            };

            _mockTaskService.Setup(s => s.PendingMergeFromPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(true);

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void RestrictUser_Execute_PendingMergePinOperationThrowsExceptionWithInvalidRequestMergePinRequestPendingExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.RestrictUser,
                Pin = 12346
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
        public void RestrictUser_Execute_PendingMergePinOperationThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.RestrictUser,
                Pin = 12346
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
