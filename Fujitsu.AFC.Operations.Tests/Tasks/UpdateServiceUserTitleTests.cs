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
    public class UpdateServiceUserTitleTests
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
                    Pin = 12345,
                    Title = "Test"
                }
            };

            _mockSiteRepository = MockRepositoryHelper.Create(_sites, (entity, id) => entity.Id == (int)id);
            _siteService = new SiteService(_mockSiteRepository.Object, _mockUnitOfWork.Object);

            _operationTaskProcessor = new UpdateServiceUserTitle(_mockPinService.Object, _mockTaskService.Object, _siteService);
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateServiceUserTitle_Constructor_NoPinService()
        {
            #region Arrange

            #endregion

            #region Act

            new UpdateServiceUserTitle(
                null,
                _mockTaskService.Object,
                _mockSiteService.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateServiceUserTitle_Constructor_NoTaskService()
        {
            #region Arrange

            #endregion

            #region Act

            new UpdateServiceUserTitle(
                _mockPinService.Object,
                null,
                 _mockSiteService.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateServiceUserTitle_Constructor_NoSiteService()
        {
            #region Arrange

            #endregion

            #region Act

            new UpdateServiceUserTitle(
                _mockPinService.Object,
                _mockTaskService.Object,
                null);

            #endregion

            #region Assert

            #endregion
        }

        #endregion

        [TestMethod]
        public void UpdateServiceUserTitle_Execute_ValidTaskWithUnallocatedSitesAvailablePassesPreLimChecksAndExecutesTask()
        {
            _mockTaskService.Setup(s => s.PendingMergeFromPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(false);

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.UpdateServiceUserTitle,
                Pin = 12345,
                SiteTitle = "Example PIN Site"
            };

            _operationTaskProcessor.Execute(task);

            _mockPinService.Verify(v => v.UpdatePinTitle(It.IsAny<Task>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void UpdateServiceUserTitle_Execute_TaskThrowsException_ExceptionIsPropogated()
        {
            _mockTaskService.Setup(s => s.PendingMergeFromPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(false);
            _mockPinService.Setup(s => s.UpdatePinTitle(It.IsAny<Task>())).Throws(new Exception());

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.UpdateServiceUserTitle,
                Pin = 12345,
                SiteTitle = "Example PIN Site"
            };

            _operationTaskProcessor.Execute(task);
        }



        [TestMethod]
        [ExpectedException(typeof(UnRecoverableErrorException))]
        public void UpdateServiceUserTitle_Execute_PinAlreadyExistsThrowsUnRecoverableErrorException()
        {
            _mockTaskService.Setup(s => s.PendingMergeFromPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(false);

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.UpdateServiceUserTitle,
                Pin = 12346,
                SiteTitle = "An Example Site"
            };

            _mockSiteService.Setup(s => s.Query(x => x.Pin == task.Pin.Value)).Returns(_sites.AsQueryable());

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void UpdateServiceUserTitle_Execute_PinDoesNotExceptionWithInvalidRequestPinDoesNotExistExceptionMessage()
        {
            _mockTaskService.Setup(s => s.PendingMergeFromPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(false);

            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.UpdateServiceUserTitle,
                Pin = 12346,
                SiteTitle = "An Example Site"
            };

            _mockSiteService.Setup(s => s.Query(x => x.Pin == task.Pin.Value)).Returns(_sites.AsQueryable());

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
        public void UpdateServiceUserTitle_Execute_PinDoesNotExistsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            _mockTaskService.Setup(s => s.PendingMergeFromPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(true);

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.UpdateServiceUserTitle,
                Pin = 12345,
                SiteTitle = "An Example Site"
            };

            _mockSiteService.Setup(s => s.Query(x => x.Pin == task.Pin.Value)).Returns(_sites.AsQueryable());

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
        public void UpdateServiceUserTitle_Execute_NoPinThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.UpdateServiceUserTitle,
                SiteTitle = "An Example Site"
            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void UpdateServiceUserTitle_Execute_NoPinThrowsExceptionWithInvalidRequestNoPinExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.UpdateServiceUserTitle,
                SiteTitle = "An Example Site"
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
        public void UpdateServiceUserTitle_Execute_NoPinThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.UpdateServiceUserTitle,
                SiteTitle = "An Example Site"
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
        public void UpdateServiceUserTitle_Execute_NoSiteTitleThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.UpdateServiceUserTitle,
                Pin = 12345
            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void UpdateServiceUserTitle_Execute_NoSiteTitleThrowsExceptionWithInvalidRequestNoSiteTitleExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.UpdateServiceUserTitle,
                Pin = 12345
            };

            var expectedMessage = string.Format(TaskResources.OperationsTaskRequest_InvalidRequestNoSiteTitle, task.Name, task.Pin.Value);

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
        public void UpdateServiceUserTitle_Execute_NoSiteTitleThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.UpdateServiceUserTitle,
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
        public void UpdateServiceUserTitle_Execute_PendingMergePinOperationThrowsUnRecoverableErrorException()
        {
            _mockTaskService.Setup(s => s.PendingMergeFromPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(true);

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.UpdateServiceUserTitle,
                Pin = 12345,
                SiteTitle = "An Example Site"
            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void UpdateServiceUserTitle_Execute_PendingMergePinOperationExceptionWithInvalidRequestMergePinRequestPendingExceptionMessage()
        {
            _mockTaskService.Setup(s => s.PendingMergeFromPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(true);


            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.UpdateServiceUserTitle,
                Pin = 12345,
                SiteTitle = "An Example Site"
            };

            _mockSiteService.Setup(s => s.Query(x => x.Pin == task.Pin.Value)).Returns(_sites.AsQueryable());

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
        public void UpdateServiceUserTitle_Execute_PendingMergePinOperationExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            _mockTaskService.Setup(s => s.PendingMergeFromPinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(true);

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.UpdateServiceUserTitle,
                Pin = 12345,
                SiteTitle = "An Example Site"
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

    }
}
