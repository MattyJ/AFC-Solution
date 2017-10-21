using System;
using System.Collections.Generic;
using System.Linq;
using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Core.Interfaces;
using Fujitsu.AFC.Data.Interfaces;
using Fujitsu.AFC.Enumerations;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Services.Interfaces;
using Fujitsu.AFC.Tasks;
using Fujitsu.AFC.Tasks.Interfaces;
using Fujitsu.AFC.Tasks.Managers;
using Fujitsu.AFC.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fujitsu.AFC.Handlers.Tests
{
    [TestClass]
    public class OperationsHandlerManagerTests
    {
        private Mock<IObjectBuilder> _mockObjectBuilder;
        private Mock<ILoggingManager> _mockLoggingManager;

        private Mock<ITaskService> _mockTaskService;
        private Mock<IService<HistoryLog>> _mockHistoryLogService;
        private Mock<IParameterService> _mockParameterService;
        private Mock<IUserIdentity> _mockUserIdentity;

        private Mock<ITaskHandler> _mockHandlerOne;

        private List<Task> _tasks;
        private ITaskHandlerManager _operationsHandlerManager;

        private readonly Guid _serviceInstanceId = new Guid("AB3617C5-CDA8-4463-8F8A-40D2095C8A60");

        private const string UserName = "matthew.jordan@uk.fujitsu.com";

        [TestInitialize]
        public void TestInitialize()
        {
            _tasks = new List<Task>
            {
                UnitTestHelper.GenerateRandomData<Task>(task =>
                {
                    task.Handler = TaskHandlerNames.ProvisioningHandler;
                }),
                UnitTestHelper.GenerateRandomData<Task>(task =>
                {
                    task.Handler = TaskHandlerNames.OperationsHandler;
                    task.Name = OperationTaskType.AllocatePin.ToString();
                    task.Frequency = TaskFrequencyNames.OneTime;
                    task.Pin = 12345;
                }),
                UnitTestHelper.GenerateRandomData<Task>(task =>
                {
                    task.Handler = TaskHandlerNames.OperationsHandler;
                    task.Name = OperationTaskType.AllocateCase.ToString();
                    task.Frequency = TaskFrequencyNames.OneTime;
                    task.Pin = 67890;
                }),
                UnitTestHelper.GenerateRandomData<Task>(task =>
                {
                    task.Handler = TaskHandlerNames.OperationsHandler;
                    task.Name = OperationTaskType.UpdateCaseTitle.ToString();
                    task.CompletedDate = DateTime.Now;
                    task.NextScheduledDate = DateTime.Now.AddDays(1);
                    task.Frequency = TaskFrequencyNames.Daily;
                    task.Pin = 34567;
                }),
                UnitTestHelper.GenerateRandomData<Task>(task =>
                {
                    task.Handler = TaskHandlerNames.SupportHandler;
                })
            };

            _mockTaskService = new Mock<ITaskService>();
            _mockTaskService.Setup(s => s.AllOperationsHandlerTasks()).Returns(_tasks.Where(x => x.Handler == TaskHandlerNames.OperationsHandler));

            _mockUserIdentity = new Mock<IUserIdentity>();
            _mockUserIdentity.Setup(s => s.Name).Returns(UserName);

            _mockHistoryLogService = new Mock<IService<HistoryLog>>();

            _mockParameterService = new Mock<IParameterService>();

            _mockHandlerOne = new Mock<ITaskHandler>();
            _mockHandlerOne.Setup(s => s.CanExecute(It.IsAny<Task>(), It.IsAny<Guid>())).Returns(true);

            _mockObjectBuilder = new Mock<IObjectBuilder>();
            _mockObjectBuilder.Setup(s => s.Resolve<ITaskHandler>(TaskHandlerNames.OperationsHandler))
                .Returns(_mockHandlerOne.Object);

            _mockLoggingManager = new Mock<ILoggingManager>();

            _operationsHandlerManager = new OperationsHandlerManager(_mockTaskService.Object, _mockHistoryLogService.Object, _mockParameterService.Object, _mockObjectBuilder.Object, _mockLoggingManager.Object, _mockUserIdentity.Object);

            Fujitsu.AFC.Tasks.Properties.Settings.Default["ServiceInstanceId"] = _serviceInstanceId;

            Bootstrapper.Initialise();
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OperationsHandlerManager_Constructor_TaskServiceNull_ThrowsArgumentNullException()
        {
            new OperationsHandlerManager(null, _mockHistoryLogService.Object, _mockParameterService.Object, _mockObjectBuilder.Object, _mockLoggingManager.Object, _mockUserIdentity.Object);
        }

        [ExpectedException(typeof(ArgumentNullException))]
        public void OperationsHandlerManager_Constructor_HistoryLogServiceNull_ThrowsArgumentNullException()
        {
            new OperationsHandlerManager(_mockTaskService.Object, null, _mockParameterService.Object, _mockObjectBuilder.Object, _mockLoggingManager.Object, _mockUserIdentity.Object);
        }

        [ExpectedException(typeof(ArgumentNullException))]
        public void OperationsHandlerManager_Constructor_ParameterServiceNull_ThrowsArgumentNullException()
        {
            new OperationsHandlerManager(_mockTaskService.Object, _mockHistoryLogService.Object, null, _mockObjectBuilder.Object, _mockLoggingManager.Object, _mockUserIdentity.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OperationsHandlerManager_Constructor_ObjectBuilderNull_ThrowsArgumentNullException()
        {
            new OperationsHandlerManager(_mockTaskService.Object, _mockHistoryLogService.Object, _mockParameterService.Object, null, _mockLoggingManager.Object, _mockUserIdentity.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OperationsHandlerManager_Constructor_LoggingManagerNull_ThrowsArgumentNullException()
        {
            new OperationsHandlerManager(_mockTaskService.Object, _mockHistoryLogService.Object, _mockParameterService.Object, _mockObjectBuilder.Object, null, _mockUserIdentity.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OperationsHandlerManager_Constructor_UserIdentityManagerNull_ThrowsArgumentNullException()
        {
            new OperationsHandlerManager(_mockTaskService.Object, _mockHistoryLogService.Object, _mockParameterService.Object, _mockObjectBuilder.Object, _mockLoggingManager.Object, null);
        }

        #endregion

        [TestMethod]
        public void OperationsHandlerManager_Execute_NoServiceInstanceIdSpecified_CreatesAnExceptionLog()
        {
            #region Arrange

            Fujitsu.AFC.Tasks.Properties.Settings.Default["ServiceInstanceId"] = Guid.Empty;

            #endregion

            #region Act

            _operationsHandlerManager.Execute();

            #endregion

            #region Assert

            _mockTaskService.Verify(v => v.AllOperationsHandlerTasks(), Times.Never);
            _mockLoggingManager.Verify(v => v.Write(LoggingEventSource.OperationsService, LoggingEventType.Error, It.IsAny<string>()));

            #endregion
        }

        [TestMethod]
        public void OperationsHandlerManager_Execute_TaskHandlerException_CreatesAHistoryLogGetsRetryDelayAndUpdatesTheTask()
        {
            #region Arrange

            _mockObjectBuilder.Setup(s => s.Resolve<ITaskHandler>(TaskHandlerNames.OperationsHandler)).Throws(new Exception());

            #endregion

            #region Act

            _operationsHandlerManager.Execute();

            #endregion

            #region Assert

            _mockTaskService.Verify(v => v.AllOperationsHandlerTasks(), Times.Once);
            _mockHistoryLogService.Verify(v => v.Create(It.IsAny<HistoryLog>()), Times.Exactly(3));
            _mockParameterService.Verify(v => v.GetParameterByNameAndCache<int>(ParameterNames.RecoverableExceptionRetryDelayIntervalInMinutes), Times.Exactly(3));
            _mockTaskService.Verify(v => v.Update(It.IsAny<Task>()), Times.Exactly(3));

            #endregion
        }

        [TestMethod]
        public void OperationsHandlerManager_Execute_GetOperationsTasksToBeProcessed_TaskServiceInvoked()
        {
            _operationsHandlerManager.Execute();
            _mockTaskService.Verify(v => v.AllOperationsHandlerTasks(), Times.Once);
        }

        [TestMethod]
        public void OperationsHandlerManager_Execute_OperationsHandlerIsResolved_ObjectBuilderResolveInvokedForEachTask()
        {
            _operationsHandlerManager.Execute();

            _mockObjectBuilder.Verify(v => v.Resolve<ITaskHandler>(It.IsAny<string>()), Times.Exactly(3));
        }

        [TestMethod]
        public void OperationsHandlerManager_Execute_OperationsHandlerIsResolved_TaskHandlerInvokedForEachTask()
        {
            _operationsHandlerManager.Execute();

            _mockHandlerOne.Verify(v => v.Execute(It.IsAny<Task>(), It.IsAny<Guid>()), Times.Exactly(3));
        }

        [TestMethod]
        public void OperationsHandlerManager_Execute_NoPinOnTaskTwo_OnlyExecutesAllThreeTasks()
        {
            #region Arrange

            _mockHandlerOne.Setup(s => s.Execute(_tasks[2], It.IsAny<Guid>())).Throws(new Exception());

            #endregion

            #region Act

            _operationsHandlerManager.Execute();

            #endregion

            #region Assert

            _mockHandlerOne.Verify(v => v.Execute(It.IsAny<Task>(), It.IsAny<Guid>()), Times.Exactly(3));
            _mockHistoryLogService.Verify(v => v.Create(It.IsAny<HistoryLog>()), Times.AtLeastOnce);
            _mockTaskService.Verify(v => v.Update(It.IsAny<Task>()), Times.AtLeastOnce);

            #endregion
        }
    }
}
