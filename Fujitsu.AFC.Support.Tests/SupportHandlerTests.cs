using System;
using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Core.Interfaces;
using Fujitsu.AFC.Data.Interfaces;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Services.Interfaces;
using Fujitsu.AFC.Support.Handler;
using Fujitsu.AFC.Support.Interfaces;
using Fujitsu.AFC.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fujitsu.AFC.Support.Tests
{
    [TestClass]
    public class SupportHandlerTests
    {

        private Mock<IObjectBuilder> _mockObjectBuilder;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ITaskService> _mockTaskService;
        private Mock<IService<HistoryLog>> _mockHistoryLogService;
        private Mock<ILoggingManager> _mockLoggingManager;

        private Mock<IRepositoryTransaction> _mockRepositoryTransaction;

        private Mock<ISupportTaskProcessor> _mockHistoryErrorLogMonitoringSupportTaskProcessor;

        private ITaskHandler _supportHandler;
        private Mock<IUserIdentity> _mockUserIdentity;


        private const string UserName = "matthew.jordan@uk.fujitsu.com";

        [TestInitialize]
        public void TestInitialize()
        {
            _mockObjectBuilder = new Mock<IObjectBuilder>();

            _mockRepositoryTransaction = new Mock<IRepositoryTransaction>();

            _mockUserIdentity = new Mock<IUserIdentity>();
            _mockUserIdentity.Setup(s => s.Name).Returns(UserName);

            _mockHistoryLogService = new Mock<IService<HistoryLog>>();

            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUnitOfWork.Setup(s => s.BeginTransaction()).Returns(_mockRepositoryTransaction.Object);

            _mockLoggingManager = new Mock<ILoggingManager>();

            _mockTaskService = new Mock<ITaskService>();

            _mockHistoryErrorLogMonitoringSupportTaskProcessor = new Mock<ISupportTaskProcessor>();

            _mockObjectBuilder = new Mock<IObjectBuilder>();
            _mockObjectBuilder.Setup(s => s.Resolve<ISupportTaskProcessor>(TaskNames.HistoryErrorLogMonitoring))
                .Returns(_mockHistoryErrorLogMonitoringSupportTaskProcessor.Object);

            _supportHandler = new SupportHandler(_mockObjectBuilder.Object, _mockTaskService.Object, _mockHistoryLogService.Object, _mockLoggingManager.Object, _mockUserIdentity.Object, _mockUnitOfWork.Object);

            Bootstrapper.Initialise();
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SupportHandler_Constructor_ObjectBuilderNull_ThrowsArgumentNullException()
        {
            new SupportHandler(null, _mockTaskService.Object, _mockHistoryLogService.Object, _mockLoggingManager.Object, _mockUserIdentity.Object, _mockUnitOfWork.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SupportHandler_Constructor_TaskServiceNull_ThrowsArgumentNullException()
        {
            new SupportHandler(_mockObjectBuilder.Object, null, _mockHistoryLogService.Object, _mockLoggingManager.Object, _mockUserIdentity.Object, _mockUnitOfWork.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SupportHandler_Constructor_HistoryLogServiceNull_ThrowsArgumentNullException()
        {
            new SupportHandler(_mockObjectBuilder.Object, _mockTaskService.Object, null, _mockLoggingManager.Object, _mockUserIdentity.Object, _mockUnitOfWork.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SupportHandler_Constructor_LoggingManagerNull_ThrowsArgumentNullException()
        {
            new SupportHandler(_mockObjectBuilder.Object, _mockTaskService.Object, _mockHistoryLogService.Object, null, _mockUserIdentity.Object, _mockUnitOfWork.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SupportHandler_Constructor_UserIdentityNull_ThrowsArgumentNullException()
        {
            new SupportHandler(_mockObjectBuilder.Object, _mockTaskService.Object, _mockHistoryLogService.Object, _mockLoggingManager.Object, null, _mockUnitOfWork.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SupportHandler_Constructor_UnitOfWorkNull_ThrowsArgumentNullException()
        {
            new SupportHandler(_mockObjectBuilder.Object, _mockTaskService.Object, _mockHistoryLogService.Object, _mockLoggingManager.Object, _mockUserIdentity.Object, null);
        }

        [TestMethod]
        public void SupportHandler_Execute_InvalidOperationTaskType_DoesNotPropogateApplicationException()
        {
            var newTask = new Task
            {
                Name = TaskNames.AllocateCase
            };

            _supportHandler.Execute(newTask, null);
        }

        #endregion

        [TestMethod]
        public void SupportHandler_Execute_ValidSupportTask_ExecutesTask()
        {
            const int pin = 34567;
            var task = new Task
            {
                Id = 7,
                Handler = TaskHandlerNames.SupportHandler,
                Name = TaskNames.HistoryErrorLogMonitoring,
                Frequency = TaskFrequencyNames.Daily,
                Pin = pin
            };

            _supportHandler.Execute(task, null);

            _mockTaskService.Verify(v => v.CompleteTask(task, It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void SupportHandler_Execute_SupportTaskThrowsException_NoExceptionIsPropogated()
        {
            const int pin = 34567;
            var task = new Task
            {
                Id = 7,
                Handler = TaskHandlerNames.SupportHandler,
                Name = TaskNames.HistoryErrorLogMonitoring,
                Frequency = TaskFrequencyNames.Daily,
                Pin = pin
            };

            _mockHistoryErrorLogMonitoringSupportTaskProcessor.Setup(s => s.Execute(It.IsAny<Task>())).Throws(new Exception());

            _supportHandler.Execute(task, null);
        }

        [TestMethod]
        public void SupportHandler_Execute_ProvisioningTaskThrowsException_AttemptsToCompleteTaskAfterException()
        {
            var task = new Task
            {
                Id = 7,
                Handler = TaskHandlerNames.SupportHandler,
                Name = TaskNames.HistoryErrorLogMonitoring,
                Frequency = TaskFrequencyNames.Daily
            };

            _mockHistoryErrorLogMonitoringSupportTaskProcessor.Setup(s => s.Execute(It.IsAny<Task>())).Throws(new Exception());

            try
            {
                _supportHandler.Execute(task, null);
            }
            catch (Exception)
            {
                // This will be handled in the Support Handler Manager
            }

            _mockObjectBuilder.Verify(v => v.Resolve<ISupportTaskProcessor>(TaskNames.HistoryErrorLogMonitoring), Times.Once);
            _mockHistoryErrorLogMonitoringSupportTaskProcessor.Verify(v => v.Execute(It.IsAny<Task>()), Times.Once);
            _mockTaskService.Verify(v => v.CompleteTask(task, It.IsAny<bool>()), Times.Once);
        }
    }
}
