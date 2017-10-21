using System;
using System.Collections.Generic;
using System.Linq;
using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Core.Interfaces;
using Fujitsu.AFC.Data.Interfaces;
using Fujitsu.AFC.Enumerations;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Services.Interfaces;
using Fujitsu.AFC.Tasks.Interfaces;
using Fujitsu.AFC.Tasks.Managers;
using Fujitsu.AFC.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fujitsu.AFC.Handlers.Tests
{
    [TestClass]
    public class ProvisioningHandlerManagerTests
    {
        private Mock<IObjectBuilder> _mockObjectBuilder;
        private Mock<ILoggingManager> _mockLoggingManager;

        private Mock<ITaskService> _mockTaskService;
        private Mock<IUserIdentity> _mockUserIdentity;

        private Mock<ITaskHandler> _mockHandlerOne;
        private Mock<ITaskHandler> _mockHandlerTwo;
        private Mock<ITaskHandler> _mockHandlerThree;

        private List<Task> _tasks;
        private ITaskHandlerManager _provisioningHandlerManager;

        [TestInitialize]
        public void TestInitialize()
        {
            _tasks = new List<Task>
            {
                UnitTestHelper.GenerateRandomData<Task>(task =>
                {
                    task.Handler = TaskHandlerNames.ProvisioningHandler;
                    task.Name = TaskNames.CaseSiteProvisioning;
                    task.NextScheduledDate = DateTime.Now.AddDays(-1);
                }),
                UnitTestHelper.GenerateRandomData<Task>(task =>
                {
                    task.Handler = TaskHandlerNames.OperationsHandler;
                }),
                UnitTestHelper.GenerateRandomData<Task>(task =>
                {
                    task.Handler = TaskHandlerNames.OperationsHandler;
                }),
                UnitTestHelper.GenerateRandomData<Task>(task =>
                {
                    task.Handler = TaskHandlerNames.OperationsHandler;
                }),
                UnitTestHelper.GenerateRandomData<Task>(task =>
                {
                    task.Handler = TaskHandlerNames.SupportHandler;
                })
            };

            _mockTaskService = new Mock<ITaskService>();
            _mockTaskService.Setup(s => s.AllProvisioningHandlerTasks()).Returns(_tasks.Where(x => x.Handler == TaskHandlerNames.ProvisioningHandler));

            _mockUserIdentity = new Mock<IUserIdentity>();

            _mockHandlerOne = new Mock<ITaskHandler>();
            _mockHandlerOne.Setup(s => s.CanExecute(It.IsAny<Task>(), null)).Returns(true);

            _mockHandlerTwo = new Mock<ITaskHandler>();
            _mockHandlerThree = new Mock<ITaskHandler>();

            _mockObjectBuilder = new Mock<IObjectBuilder>();
            _mockObjectBuilder.Setup(s => s.Resolve<ITaskHandler>(TaskHandlerNames.ProvisioningHandler))
                .Returns(_mockHandlerOne.Object);
            _mockObjectBuilder.Setup(s => s.Resolve<ITaskHandler>(TaskHandlerNames.OperationsHandler))
                .Returns(_mockHandlerTwo.Object);
            _mockObjectBuilder.Setup(s => s.Resolve<ITaskHandler>(TaskHandlerNames.SupportHandler))
                .Returns(_mockHandlerThree.Object);

            _mockLoggingManager = new Mock<ILoggingManager>();

            _provisioningHandlerManager = new ProvisioningHandlerManager(_mockTaskService.Object, _mockObjectBuilder.Object, _mockLoggingManager.Object);
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProvisioningHandlerManager_Constructor_TaskServiceNull_ThrowsArgumentNullException()
        {
            new ProvisioningHandlerManager(null, _mockObjectBuilder.Object, _mockLoggingManager.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProvisioningHandlerManager_Constructor_ObjectBuilderNull_ThrowsArgumentNullException()
        {
            new ProvisioningHandlerManager(_mockTaskService.Object, null, _mockLoggingManager.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProvisioningHandlerManager_Constructor_LoggingManagerNull_ThrowsArgumentNullException()
        {
            new ProvisioningHandlerManager(_mockTaskService.Object, _mockObjectBuilder.Object, null);
        }

        #endregion

        [TestMethod]
        public void ProvisioningHandlerManager_Execute_GetProvisioningTasksToBeProcessed_TaskServiceInvoked()
        {
            _provisioningHandlerManager.Execute();
            _mockTaskService.Verify(v => v.AllProvisioningHandlerTasks(), Times.Once);
        }

        [TestMethod]
        public void ProvisioningHandlerManager_Execute_ProvisioningHandlerIsResolved_ObjectBuilderResolveInvoked()
        {
            _provisioningHandlerManager.Execute();

            _mockObjectBuilder.Verify(v => v.Resolve<ITaskHandler>(TaskHandlerNames.ProvisioningHandler), Times.Once);
        }

        [TestMethod]
        public void ProvisioningHandlerManager_Execute_ProvisioningHandlerCanExecuteIsInvoked()
        {
            _provisioningHandlerManager.Execute();

            _mockObjectBuilder.Verify(v => v.Resolve<ITaskHandler>(TaskHandlerNames.ProvisioningHandler), Times.Once);
            _mockHandlerOne.Verify(v => v.Execute(It.IsAny<Task>(), null), Times.Once);
        }

        [TestMethod]
        public void ProvisioningHandlerManager_Execute_ProvisioningHandlerExecuteIsInvoked()
        {
            _provisioningHandlerManager.Execute();

            _mockObjectBuilder.Verify(v => v.Resolve<ITaskHandler>(TaskHandlerNames.ProvisioningHandler), Times.Once);
            _mockHandlerOne.Verify(v => v.CanExecute(It.IsAny<Task>(), null), Times.Once);
            _mockHandlerOne.Verify(v => v.Execute(It.IsAny<Task>(), null), Times.Once);
        }

        [TestMethod]
        public void ProvisioningHandlerManager_Execute_TaskHandlerObjectBuilderException_CreatesAnExceptionLog()
        {
            #region Arrange

            _mockObjectBuilder.Setup(s => s.Resolve<ITaskHandler>(TaskHandlerNames.ProvisioningHandler)).Throws(new Exception());

            #endregion

            #region Act

            _provisioningHandlerManager.Execute();

            #endregion

            #region Assert

            _mockTaskService.Verify(v => v.AllProvisioningHandlerTasks(), Times.Once);
            _mockLoggingManager.Verify(v => v.Write(LoggingEventSource.ProvisioningService, LoggingEventType.Error, It.IsAny<string>()));

            #endregion
        }

        [TestMethod]
        public void ProvisioningHandlerManager_Execute_TaskHandlerException_CreatesAnExceptionLog()
        {
            #region Arrange

            _mockHandlerOne.Setup(s => s.Execute(It.IsAny<Task>(), null)).Throws(new Exception());

            #endregion

            #region Act

            _provisioningHandlerManager.Execute();

            #endregion

            #region Assert

            _mockTaskService.Verify(v => v.AllProvisioningHandlerTasks(), Times.Once);
            _mockLoggingManager.Verify(v => v.Write(LoggingEventSource.ProvisioningService, LoggingEventType.Error, It.IsAny<string>()));

            #endregion
        }
    }
}
