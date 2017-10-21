using System;
using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Core.Interfaces;
using Fujitsu.AFC.Data.Interfaces;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Provisioning.Handler;
using Fujitsu.AFC.Provisioning.Interfaces;
using Fujitsu.AFC.Services.Interfaces;
using Fujitsu.AFC.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fujitsu.AFC.Provisioning.Tests
{
    [TestClass]
    public class ProvisioningHandlerTests
    {

        private Mock<IObjectBuilder> _mockObjectBuilder;
        private Mock<ITaskService> _mockTaskService;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ILoggingManager> _mockLoggingManager;
        private Mock<IService<HistoryLog>> _mockHistoryLogService;
        private Mock<IUserIdentity> _mockUserIdentity;

        private Mock<IRepositoryTransaction> _mockRepositoryTransaction;

        private Mock<IProvisioningTaskProcessor> _mockCaseSiteProvisioningTaskProcessor;

        private ITaskHandler _provisioningHandler;

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

            _mockCaseSiteProvisioningTaskProcessor = new Mock<IProvisioningTaskProcessor>();

            _mockObjectBuilder = new Mock<IObjectBuilder>();
            _mockObjectBuilder.Setup(s => s.Resolve<IProvisioningTaskProcessor>(TaskNames.CaseSiteProvisioning))
                .Returns(_mockCaseSiteProvisioningTaskProcessor.Object);

            _provisioningHandler = new ProvisioningHandler(_mockObjectBuilder.Object, _mockTaskService.Object, _mockHistoryLogService.Object, _mockLoggingManager.Object, _mockUserIdentity.Object, _mockUnitOfWork.Object);

            Bootstrapper.Initialise();
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProvisioningHandler_Constructor_ObjectBuilderNull_ThrowsArgumentNullException()
        {
            new ProvisioningHandler(null, _mockTaskService.Object, _mockHistoryLogService.Object, _mockLoggingManager.Object, _mockUserIdentity.Object, _mockUnitOfWork.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProvisioningHandler_Constructor_TaskServiceNull_ThrowsArgumentNullException()
        {
            new ProvisioningHandler(_mockObjectBuilder.Object, null, _mockHistoryLogService.Object, _mockLoggingManager.Object, _mockUserIdentity.Object, _mockUnitOfWork.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProvisioningHandler_Constructor_HistoryLogServiceNull_ThrowsArgumentNullException()
        {
            new ProvisioningHandler(_mockObjectBuilder.Object, _mockTaskService.Object, null, _mockLoggingManager.Object, _mockUserIdentity.Object, _mockUnitOfWork.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProvisioningHandler_Constructor_LoggingManagerNull_ThrowsArgumentNullException()
        {
            new ProvisioningHandler(_mockObjectBuilder.Object, _mockTaskService.Object, _mockHistoryLogService.Object, null, _mockUserIdentity.Object, _mockUnitOfWork.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProvisioningHandler_Constructor_UserIdentityNull_ThrowsArgumentNullException()
        {
            new ProvisioningHandler(_mockObjectBuilder.Object, _mockTaskService.Object, _mockHistoryLogService.Object, _mockLoggingManager.Object, null, _mockUnitOfWork.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProvisioningHandler_Constructor_UnitOfWorkNull_ThrowsArgumentNullException()
        {
            new ProvisioningHandler(_mockObjectBuilder.Object, _mockTaskService.Object, _mockHistoryLogService.Object, _mockLoggingManager.Object, _mockUserIdentity.Object, null);
        }


        #endregion

        [TestMethod]
        public void ProvisioningHandler_Execute_InvalidProvisioningTaskType_DoesNotPropogateApplicationException()
        {
            var newTask = new Task
            {
                Name = TaskNames.AllocateCase
            };

            _provisioningHandler.Execute(newTask, null);
        }

        [TestMethod]
        public void ProvisioningHandler_Dispose_UnitOfWorkInstantiated_UnitOfWorkDisposeInvoked()
        {
            var task = new Task
            {
                Id = 7,
                Handler = TaskHandlerNames.ProvisioningHandler,
                Name = TaskNames.CaseSiteProvisioning,
                Frequency = TaskFrequencyNames.Daily
            };

            _provisioningHandler.Execute(task, null);
            _provisioningHandler.Dispose();
            _mockUnitOfWork.Verify(v => v.Dispose(), Times.Once);
        }

        [TestMethod]
        public void ProvisioningHandler_Execute_ValidProvisioningTask_ExecutesTask()
        {
            var task = new Task
            {
                Id = 7,
                Handler = TaskHandlerNames.ProvisioningHandler,
                Name = TaskNames.CaseSiteProvisioning,
                Frequency = TaskFrequencyNames.Daily
            };

            _provisioningHandler.Execute(task, null);

            _mockTaskService.Verify(v => v.CompleteTask(task, It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void ProvisioningHandler_Execute_ProvisioningTaskThrowsException_AttemptsToCompleteTaskAfterException()
        {
            var task = new Task
            {
                Id = 7,
                Handler = TaskHandlerNames.ProvisioningHandler,
                Name = TaskNames.CaseSiteProvisioning,
                Frequency = TaskFrequencyNames.Daily
            };

            _mockCaseSiteProvisioningTaskProcessor.Setup(s => s.Execute(It.IsAny<Task>())).Throws(new Exception());

            try
            {
                _provisioningHandler.Execute(task, null);
            }
            catch (Exception)
            {
                // This will be handled in the Provisioning Handler Manager
            }

            _mockObjectBuilder.Verify(v => v.Resolve<IProvisioningTaskProcessor>(TaskNames.CaseSiteProvisioning), Times.Once);
            _mockCaseSiteProvisioningTaskProcessor.Verify(v => v.Execute(It.IsAny<Task>()), Times.Once);
            _mockTaskService.Verify(v => v.CompleteTask(task, It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void ProvisioningHandler_Execute_ProvisioningTaskThrowsException_NoExceptionIsPropogated()
        {
            var task = new Task
            {
                Id = 7,
                Handler = TaskHandlerNames.ProvisioningHandler,
                Name = TaskNames.CaseSiteProvisioning,
                Frequency = TaskFrequencyNames.Daily
            };

            _mockCaseSiteProvisioningTaskProcessor.Setup(s => s.Execute(It.IsAny<Task>())).Throws(new Exception());

            _provisioningHandler.Execute(task, null);
        }

    }
}
