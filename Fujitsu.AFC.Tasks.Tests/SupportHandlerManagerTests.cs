using System;
using System.Collections.Generic;
using System.Linq;
using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Core.Interfaces;
using Fujitsu.AFC.Data.Interfaces;
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
    public class SupportHandlerManagerTests
    {
        private Mock<IObjectBuilder> _mockObjectBuilder;
        private Mock<ILoggingManager> _mockLoggingManager;

        private Mock<ITaskService> _mockTaskService;

        private Mock<ITaskHandler> _mockHandlerOne;
        private Mock<ITaskHandler> _mockHandlerTwo;
        private Mock<ITaskHandler> _mockHandlerThree;

        private List<Task> _tasks;
        private ITaskHandlerManager _supportHandlerManager;


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
            _mockTaskService.Setup(s => s.AllSupportHandlerTasks()).Returns(_tasks.Where(x => x.Handler == TaskHandlerNames.SupportHandler));

            _mockHandlerOne = new Mock<ITaskHandler>();
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

            _supportHandlerManager = new SupportHandlerManager(_mockTaskService.Object, _mockObjectBuilder.Object, _mockLoggingManager.Object);
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SupportHandlerManager_Constructor_TaskServiceNull_ThrowsArgumentNullException()
        {
            new SupportHandlerManager(null, _mockObjectBuilder.Object, _mockLoggingManager.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SupportHandlerManager_Constructor_ObjectBuilderNull_ThrowsArgumentNullException()
        {
            new SupportHandlerManager(_mockTaskService.Object, null, _mockLoggingManager.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SupportHandlerManager_Constructor_LoggingManagerNull_ThrowsArgumentNullException()
        {
            new SupportHandlerManager(_mockTaskService.Object, _mockObjectBuilder.Object, null);
        }

        #endregion

        [TestMethod]
        public void SupportHandlerManager_Execute_GetSupportTasksToBeProcessed_TaskServiceInvoked()
        {
            _supportHandlerManager.Execute();
            _mockTaskService.Verify(v => v.AllSupportHandlerTasks(), Times.Once);
        }

        [TestMethod]
        public void SupportHandlerManager_Execute_SupportHandlerIsResolved_ObjectBuilderResolveInvoked()
        {
            _supportHandlerManager.Execute();
            _mockObjectBuilder.Verify(v => v.Resolve<ITaskHandler>(It.IsAny<string>()), Times.Once);
        }


    }
}
