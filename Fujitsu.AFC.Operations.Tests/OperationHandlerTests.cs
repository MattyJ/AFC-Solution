using System;
using System.Collections.Generic;
using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Core.Interfaces;
using Fujitsu.AFC.Data.Interfaces;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Operations.Handler;
using Fujitsu.AFC.Operations.Interfaces;
using Fujitsu.AFC.Services.Interfaces;
using Fujitsu.AFC.Tasks;
using Fujitsu.Exceptions.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fujitsu.AFC.Operations.Tests
{
    [TestClass]
    public class OperationsHandlerTests
    {

        private Mock<IObjectBuilder> _mockObjectBuilder;
        private Mock<ITaskService> _mockTaskService;
        private Mock<ITimerLockService> _mockTimerLockService;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IRepositoryTransaction> _mockRepositoryTransaction;

        private ITaskHandler _operationsHandler;
        private Mock<IOperationsTaskProcessor> _mockAllocatePinOperationTaskProcessor;

        private List<TimerLock> _timerLocks;

        private readonly Guid _serviceInstanceOne = new Guid("71D5C7C8-3B7B-481D-B489-C9178F22A2DE");
        private readonly Guid _serviceInstanceTwo = new Guid("AB3617C5-CDA8-4463-8F8A-40D2095C8A60");

        private int _lockedPinOne = 12345;

        [TestInitialize]
        public void TestInitialize()
        {
            _timerLocks = new List<TimerLock>
            {
                new TimerLock
                {
                    TaskId = 1,
                    Task = new Task
                    {
                        Id = 1,
                        Pin = _lockedPinOne
                    },
                    LockedInstance = _serviceInstanceOne,
                    LockedPin = _lockedPinOne
                }
            };

            _mockObjectBuilder = new Mock<IObjectBuilder>();
            _mockTimerLockService = new Mock<ITimerLockService>();
            _mockTimerLockService.Setup(s => s.All()).Returns(_timerLocks);

            _mockRepositoryTransaction = new Mock<IRepositoryTransaction>();

            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUnitOfWork.Setup(s => s.BeginTransaction()).Returns(_mockRepositoryTransaction.Object);
            _mockTaskService = new Mock<ITaskService>();

            _mockAllocatePinOperationTaskProcessor = new Mock<IOperationsTaskProcessor>();

            _mockObjectBuilder = new Mock<IObjectBuilder>();
            _mockObjectBuilder.Setup(s => s.Resolve<IOperationsTaskProcessor>(TaskNames.AllocatePin))
                .Returns(_mockAllocatePinOperationTaskProcessor.Object);


            _operationsHandler = new OperationsHandler(_mockObjectBuilder.Object, _mockTimerLockService.Object, _mockTaskService.Object, _mockUnitOfWork.Object);

            Bootstrapper.Initialise();
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OperationsHandler_Constructor_ObjectBuilderNull_ThrowsArgumentNullException()
        {
            new OperationsHandler(null, _mockTimerLockService.Object, _mockTaskService.Object, _mockUnitOfWork.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OperationsHandler_Constructor_TimerLockServiceNull_ThrowsArgumentNullException()
        {
            new OperationsHandler(_mockObjectBuilder.Object, null, _mockTaskService.Object, _mockUnitOfWork.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OperationsHandler_Constructor_TaskServiceNull_ThrowsArgumentNullException()
        {
            new OperationsHandler(_mockObjectBuilder.Object, _mockTimerLockService.Object, null, _mockUnitOfWork.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OperationsHandler_Constructor_UnitOfWorkNull_ThrowsArgumentNullException()
        {
            new OperationsHandler(_mockObjectBuilder.Object, _mockTimerLockService.Object, _mockTaskService.Object, null);
        }

        #endregion

        [TestMethod]
        [ExpectedException(typeof(UnRecoverableErrorException))]
        public void OperationsHandler_Execute_InvalidOperationTaskType_ThrowsException()
        {
            var newTask = new Task
            {
                Name = TaskNames.CaseSiteProvisioning,
                Pin = 12345
            };

            _operationsHandler.Execute(newTask, new Guid());
        }

        [TestMethod]
        public void OperationsHandler_Execute_InvalidOperationTaskType_CompleteUnrecoverableTaskExceptionIsCalled()
        {
            var newTask = new Task
            {
                Name = TaskNames.CaseSiteProvisioning,
                Pin = 12345
            };

            try
            {
                _operationsHandler.Execute(newTask, new Guid());
            }
            catch (Exception)
            {
                // Exception caught to test verification of moving task to the dead task queue
            }

            _mockTaskService.Verify(v => v.CompleteUnrecoverableTaskException(It.IsAny<Task>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void OperationsHandler_CanExecute_PinIsLockedByAnotherServiceInstanceId_ReturnsFalse()
        {
            var newTask = new Task
            {
                Name = TaskNames.AllocateCase,
                Frequency = TaskFrequencyNames.OneTime,
                Pin = _lockedPinOne
            };

            Assert.IsFalse(_operationsHandler.CanExecute(newTask, _serviceInstanceTwo));
        }

        [TestMethod]
        public void OperationsHandler_CanExecute_PinIsLockedBySameServiceInstanceId_ReturnsTrueForLockedTask()
        {
            var newTask = new Task
            {
                Id = 1,
                Name = TaskNames.AllocateCase,
                Frequency = TaskFrequencyNames.OneTime,
                Pin = _lockedPinOne
            };

            Assert.IsTrue(_operationsHandler.CanExecute(newTask, _serviceInstanceOne));
        }

        [TestMethod]
        public void OperationsHandler_CanExecute_PinIsLockedBySameServiceInstanceIdOnDifferentTask_ReturnsFalse()
        {
            var newTask = new Task
            {
                Id = 7,
                Name = TaskNames.AllocateCase,
                Frequency = TaskFrequencyNames.OneTime,
                Pin = _lockedPinOne,
            };

            Assert.IsFalse(_operationsHandler.CanExecute(newTask, _serviceInstanceOne));
        }

        [TestMethod]
        public void OperationsHandler_Execute_ValidOperationTask_ExecutesTask()
        {
            const int pin = 34567;
            var task = new Task
            {
                Id = 7,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.AllocatePin,
                Frequency = TaskFrequencyNames.OneTime,
                Pin = pin
            };

            _operationsHandler.Execute(task, _serviceInstanceTwo);

            _mockTimerLockService.Verify(v => v.AcquireLock(_serviceInstanceTwo, pin, 7), Times.Once);
            _mockTaskService.Verify(v => v.CompleteTask(task, It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void OperationsHandler_Execute_OperationTaskThrowsException_ExceptionIsPropogated()
        {
            const int pin = 34567;
            var task = new Task
            {
                Id = 7,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.AllocatePin,
                Frequency = TaskFrequencyNames.OneTime,
                Pin = pin
            };

            _mockAllocatePinOperationTaskProcessor.Setup(s => s.Execute(It.IsAny<Task>())).Throws(new Exception());

            _operationsHandler.Execute(task, _serviceInstanceTwo);

        }
    }
}
