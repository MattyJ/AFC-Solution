using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Core.Interfaces;
using Fujitsu.AFC.Data.Interfaces;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Services.Interfaces;
using Fujitsu.AFC.Tasks;
using Fujitsu.AFC.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fujitsu.AFC.Services.Tests
{
    [TestClass]
    public class TaskServiceTests
    {
        private Mock<IRepository<Task>> _mockTaskRepository;
        private Mock<IRepository<TimerLock>> _mockTimerLockRepository;
        private Mock<IRepository<HistoryLog>> _mockHistoryLogRepository;
        private Mock<IUserIdentity> _mockUserIdentity;
        private Mock<IUnitOfWork> _mockUnitOfWork;

        private List<Task> _tasks;
        private List<TimerLock> _timerLocks;

        private ITaskService _taskService;

        private const string UserName = "matthew.jordan@uk.fujitsu.com";
        private readonly DateTime _dateTime = DateTime.Now;

        [TestInitialize]
        public void Intialize()
        {
            _timerLocks = new List<TimerLock>
            {
                new TimerLock
                {
                    Id =4,
                    LockedPin = 12345,
                    TaskId = 4,
                    InsertedBy = UserName,
                    InsertedDate = _dateTime.AddMinutes(-1),
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime
                }

            };

            _tasks = new List<Task>
            {
                 new Task
                    {
                        Id = 1,
                        Frequency = TaskFrequencyNames.Daily,
                        Handler = TaskHandlerNames.ProvisioningHandler,
                        Name = TaskNames.CaseSiteProvisioning,
                        InsertedBy = UserName,
                        InsertedDate = _dateTime.AddMinutes(-1),
                        UpdatedBy = UserName,
                        UpdatedDate = _dateTime
                    },
                 new Task
                    {
                        Id = 2,
                        Frequency = TaskFrequencyNames.Daily,
                        Handler = TaskHandlerNames.ProvisioningHandler,
                        Name = "Extra Task",
                        InsertedBy = UserName,
                        InsertedDate = _dateTime,
                        UpdatedBy = UserName,
                        UpdatedDate = _dateTime
                    },
                 new Task
                    {
                        Id = 3,
                        Frequency = TaskFrequencyNames.OneTime,
                        Handler = TaskHandlerNames.OperationsHandler,
                        Name = TaskNames.AllocatePin,
                        Pin = 12345,
                        SiteTitle = "An Example Site",
                        InsertedBy = UserName,
                        InsertedDate = _dateTime.AddDays(-1),
                        UpdatedBy = UserName,
                        UpdatedDate = _dateTime
                    },
                  new Task
                    {
                        Id = 4,
                        Frequency = TaskFrequencyNames.OneTime,
                        Handler = TaskHandlerNames.OperationsHandler,
                        Name = TaskNames.AllocateCase,
                        Pin = 67890,
                        InsertedBy = UserName,
                        InsertedDate = _dateTime,
                        UpdatedBy = UserName,
                        UpdatedDate = _dateTime
                    },
                  new Task
                    {
                        Id = 5,
                        Frequency = TaskFrequencyNames.Daily,
                        Handler = TaskHandlerNames.SupportHandler,
                        Name = TaskNames.HistoryErrorLogMonitoring,
                        InsertedBy = UserName,
                        InsertedDate = _dateTime.AddHours(-1),
                        UpdatedBy = UserName,
                        UpdatedDate = _dateTime
                    },
                  new Task
                    {
                        Id = 6,
                        Frequency = TaskFrequencyNames.Daily,
                        Handler = TaskHandlerNames.SupportHandler,
                        Name = "Extra Task",
                        InsertedBy = UserName,
                        InsertedDate = _dateTime,
                        UpdatedBy = UserName,
                        UpdatedDate = _dateTime
                    },
                  new Task
                    {
                        Id = 7,
                        Frequency = TaskFrequencyNames.OneTime,
                        Handler = TaskHandlerNames.OperationsHandler,
                        Name = TaskNames.MergePin,
                        Pin = 12345,
                        ToPin = 67890,
                        FromPin = 12345,
                        SiteTitle = "An Example Site",
                        InsertedBy = UserName,
                        InsertedDate = _dateTime.AddDays(-1),
                        UpdatedBy = UserName,
                        UpdatedDate = _dateTime
                    },
                    new Task
                    {
                        Id = 7,
                        Frequency = TaskFrequencyNames.OneTime,
                        Handler = TaskHandlerNames.OperationsHandler,
                        Name = TaskNames.MergePin,
                        Pin = 12345,
                        ToPin = 12345,
                        FromPin = 56789,
                        SiteTitle = "An Example Site",
                        InsertedBy = UserName,
                        InsertedDate = _dateTime.AddDays(-1),
                        UpdatedBy = UserName,
                        UpdatedDate = _dateTime
                    },
            };


            _mockUserIdentity = new Mock<IUserIdentity>();
            _mockUserIdentity.Setup(s => s.Name).Returns(UserName);

            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockTaskRepository = MockRepositoryHelper.Create(_tasks, (entity, id) => entity.Id == (int)id);
            _mockHistoryLogRepository = new Mock<IRepository<HistoryLog>>();
            _mockTimerLockRepository = MockRepositoryHelper.Create(_timerLocks, (entity, id) => entity.Id == (int)id);

            _taskService = new TaskService(_mockTaskRepository.Object, _mockTimerLockRepository.Object, _mockHistoryLogRepository.Object, _mockUserIdentity.Object, _mockUnitOfWork.Object);

            Bootstrapper.Initialise();
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TaskService_Constructor_NoTaskRepository()
        {
            #region Arrange

            #endregion

            #region Act

            new TaskService(
                null,
                _mockTimerLockRepository.Object,
                _mockHistoryLogRepository.Object,
                _mockUserIdentity.Object,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TaskService_Constructor_NoTimerLockRepository()
        {
            #region Arrange

            #endregion

            #region Act

            new TaskService(
                _mockTaskRepository.Object,
                null,
                _mockHistoryLogRepository.Object,
                _mockUserIdentity.Object,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TaskService_Constructor_NoUserIdentity()
        {
            #region Arrange

            #endregion

            #region Act

            new TaskService(
                _mockTaskRepository.Object,
                _mockTimerLockRepository.Object,
                _mockHistoryLogRepository.Object,
                null,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TaskService_Constructor_NoUnitOfWork()
        {
            #region Arrange

            #endregion

            #region Act

            new TaskService(
                _mockTaskRepository.Object,
                _mockTimerLockRepository.Object,
                _mockHistoryLogRepository.Object,
                _mockUserIdentity.Object,
                null);

            #endregion

            #region Assert

            #endregion
        }

        #endregion

        [TestMethod]
        public void TaskService_Create_CallsInsertAndUnitOfWorkSave()
        {
            #region Arrange

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.Daily,
                Handler = TaskHandlerNames.ProvisioningHandler,
                Name = TaskNames.CaseSiteProvisioning,
                InsertedBy = UserName,
                InsertedDate = _dateTime,
                UpdatedBy = UserName,
                UpdatedDate = _dateTime
            };

            #endregion

            #region Act

            _taskService.Create(task);

            #endregion

            #region Assert

            _mockTaskRepository.Verify(x => x.Insert(It.IsAny<Task>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void TaskService_Update_CallsUpdateAndUnitOfWorkSave()
        {
            #region Arrange

            var task = new Task
            {
                Id = 1,
                CompletedDate = DateTime.Now,
            };

            #endregion

            #region Act

            _taskService.Update(task);

            #endregion

            #region Assert

            _mockTaskRepository.Verify(x => x.Update(It.IsAny<Task>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void TaskService_Delete_ByEntity_CallDeleteAndUnitOfWorkSaveChanges()
        {
            #region Arrange

            var task = new Task
            {
                Id = 1
            };

            #endregion

            #region Act

            _taskService.Delete(task);

            #endregion

            #region Assert

            _mockTaskRepository.Verify(x => x.Delete(It.IsAny<Task>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void TaskService_Delete_ById_CallsDeleteAndUnitOfWorkSaveChanges()
        {
            #region Act

            _taskService.Delete(1);

            #endregion

            #region Assert

            _mockTaskRepository.Verify(x => x.Delete(It.IsAny<int>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void TaskService_GetAll_CallsRepositoryAll()
        {
            #region Arrange

            #endregion

            #region Act

            var tasks = _taskService.All();

            #endregion

            #region Assert

            _mockTaskRepository.Verify(x => x.All(), Times.Once);
            Assert.AreEqual(_tasks.Count, tasks.Count());

            #endregion
        }

        [TestMethod]
        public void TaskService_GetById_CallsRepositoryGetById()
        {
            #region Arrange

            #endregion

            #region Act

            _taskService.GetById(1);

            #endregion

            #region Assert

            _mockTaskRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void TaskService_Query_CallsRepositoryQuery()
        {
            #region Arrange

            #endregion

            #region Act

            var query = _taskService.Query(x => x.SiteTitle == "Test").ToList();

            #endregion

            #region Assert


            Assert.IsNotNull(query);
            _mockTaskRepository.Verify(x => x.Query(It.IsAny<Expression<Func<Task, bool>>>()), Times.Once());

            #endregion
        }
        [TestMethod]
        public void TaskService_AllProvisioningHandlerTasks_CallsRepositoryQuery()
        {
            #region Arrange

            #endregion

            #region Act

            var outstandingTasks = _taskService.AllProvisioningHandlerTasks().ToList();

            #endregion

            #region Assert


            Assert.IsNotNull(outstandingTasks);
            _mockTaskRepository.Verify(x => x.Query(It.IsAny<Expression<Func<Task, bool>>>()), Times.Once());

            #endregion
        }

        [TestMethod]
        public void TaskService_AllProvisioningHandlerTasks_ReturnsTwoResultsInTheCorrectOrder()
        {
            #region Arrange

            #endregion

            #region Act

            var outstandingTasks = _taskService.AllProvisioningHandlerTasks().ToList();

            #endregion

            #region Assert


            Assert.IsNotNull(outstandingTasks);
            Assert.AreEqual(TaskNames.CaseSiteProvisioning, outstandingTasks[0].Name);
            Assert.AreEqual(2, outstandingTasks.Count);

            #endregion
        }

        [TestMethod]
        public void TaskService_AllOperationsHandlerTasks_CallsRepositoryQuery()
        {
            #region Arrange

            #endregion

            #region Act

            var outstandingTasks = _taskService.AllOperationsHandlerTasks().ToList();

            #endregion

            #region Assert


            Assert.IsNotNull(outstandingTasks);
            _mockTaskRepository.Verify(x => x.Query(It.IsAny<Expression<Func<Task, bool>>>()), Times.Once());

            #endregion
        }

        [TestMethod]
        public void TaskService_AllOperationsHandlerTasks_ReturnsThreeResultsInTheCorrectOrder()
        {
            #region Arrange

            #endregion

            #region Act

            var outstandingTasks = _taskService.AllOperationsHandlerTasks().ToList();

            #endregion

            #region Assert


            Assert.IsNotNull(outstandingTasks);
            Assert.AreEqual(TaskNames.AllocatePin, outstandingTasks[0].Name);
            Assert.AreEqual(4, outstandingTasks.Count);

            #endregion
        }

        [TestMethod]
        public void TaskService_AllSupportHandlerTasks_CallsRepositoryQuery()
        {
            #region Arrange

            #endregion

            #region Act

            var outstandingTasks = _taskService.AllSupportHandlerTasks().ToList();

            #endregion

            #region Assert


            Assert.IsNotNull(outstandingTasks);
            _mockTaskRepository.Verify(x => x.Query(It.IsAny<Expression<Func<Task, bool>>>()), Times.Once());

            #endregion
        }

        [TestMethod]
        public void TaskService_AllSupportHandlerTasks_ReturnsTwoResultInTheCorrectOrder()
        {
            #region Arrange

            #endregion

            #region Act

            var outstandingTasks = _taskService.AllSupportHandlerTasks().ToList();

            #endregion

            #region Assert


            Assert.IsNotNull(outstandingTasks);
            Assert.AreEqual(TaskNames.HistoryErrorLogMonitoring, outstandingTasks[0].Name);
            Assert.AreEqual(2, outstandingTasks.Count);

            #endregion
        }

        [TestMethod]
        public void TaskService_UpdateTask_OneOffTask_CallsDeleteAndSavesChanges()
        {
            #region Arrange

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
            };

            #endregion

            #region Act

            _taskService.UpdateTask(task);

            #endregion

            #region Assert

            _mockTaskRepository.Verify(x => x.Delete(It.IsAny<Task>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion 
        }

        [TestMethod]
        public void TaskService_UpdateTask_ScheduledDailyTask_CallsUpdateAndSavesChanges()
        {
            #region Arrange

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.Daily,
            };

            #endregion

            #region Act

            _taskService.UpdateTask(task);

            #endregion

            #region Assert

            _mockTaskRepository.Verify(x => x.Update(It.IsAny<Task>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion 
        }

        [TestMethod]
        public void TaskService_UpdateTask_ScheduledWeeklyDailyTask_CallsUpdateAndSavesChanges()
        {
            #region Arrange

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.Weekly,
            };

            #endregion

            #region Act

            _taskService.UpdateTask(task);

            #endregion

            #region Assert

            _mockTaskRepository.Verify(x => x.Update(It.IsAny<Task>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion 
        }

        [TestMethod]
        public void TaskService_UpdateTask_ScheduledWeeklyMonthlyTask_CallsUpdateAndSavesChanges()
        {
            #region Arrange

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.Monthly,
            };

            #endregion

            #region Act

            _taskService.UpdateTask(task);

            #endregion

            #region Assert

            _mockTaskRepository.Verify(x => x.Update(It.IsAny<Task>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion 
        }

        [TestMethod]
        public void TaskService_UpdateTask_ScheduledWeeklyYearlyTask_CallsUpdateAndSavesChanges()
        {
            #region Arrange

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.Yearly,
            };

            #endregion

            #region Act

            _taskService.UpdateTask(task);

            #endregion

            #region Assert

            _mockTaskRepository.Verify(x => x.Update(It.IsAny<Task>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion 
        }

        [TestMethod]
        public void TaskService_UpdateDailyTask_UpdatedDateAndByIsSet_DateIsSetToNowByToUsername()
        {
            #region Arrange

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.Daily,
            };

            #endregion

            #region Act

            _taskService.UpdateTask(task);

            #endregion

            var now = DateTime.Now;
            Assert.AreEqual(now.Year, task.UpdatedDate.Year);
            Assert.AreEqual(now.Month, task.UpdatedDate.Month);
            Assert.AreEqual(now.Day, task.UpdatedDate.Day);
            Assert.AreEqual(now.Hour, task.UpdatedDate.Hour);
            Assert.AreEqual(now.Minute, task.UpdatedDate.Minute);
            Assert.AreEqual(UserName, task.UpdatedBy);
        }

        [TestMethod]
        public void TaskService_UpdateWeeklyTask_UpdatedDateAndByIsSet_IsSetToNowByToUsername()
        {
            #region Arrange

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.Weekly,
            };

            #endregion

            #region Act

            _taskService.UpdateTask(task);

            #endregion

            var now = DateTime.Now;
            Assert.AreEqual(now.Year, task.UpdatedDate.Year);
            Assert.AreEqual(now.Month, task.UpdatedDate.Month);
            Assert.AreEqual(now.Day, task.UpdatedDate.Day);
            Assert.AreEqual(now.Hour, task.UpdatedDate.Hour);
            Assert.AreEqual(now.Minute, task.UpdatedDate.Minute);
            Assert.AreEqual(UserName, task.UpdatedBy);
        }

        [TestMethod]
        public void TaskService_UpdateMonthlyTask_UpdatedDateAndByIsSet_DateIsSetToNowByToUsername()
        {
            #region Arrange

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.Monthly,
            };

            #endregion

            #region Act

            _taskService.UpdateTask(task);

            #endregion

            var now = DateTime.Now;
            Assert.AreEqual(now.Year, task.UpdatedDate.Year);
            Assert.AreEqual(now.Month, task.UpdatedDate.Month);
            Assert.AreEqual(now.Day, task.UpdatedDate.Day);
            Assert.AreEqual(now.Hour, task.UpdatedDate.Hour);
            Assert.AreEqual(now.Minute, task.UpdatedDate.Minute);
            Assert.AreEqual(UserName, task.UpdatedBy);
        }

        [TestMethod]
        public void TaskService_UpdateYearlyTask_UpdatedDateAndByIsSet_DateIsSetToNowByToUsername()
        {
            #region Arrange

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.Yearly,
            };

            #endregion

            #region Act

            _taskService.UpdateTask(task);

            #endregion

            var now = DateTime.Now;
            Assert.AreEqual(now.Year, task.UpdatedDate.Year);
            Assert.AreEqual(now.Month, task.UpdatedDate.Month);
            Assert.AreEqual(now.Day, task.UpdatedDate.Day);
            Assert.AreEqual(now.Hour, task.UpdatedDate.Hour);
            Assert.AreEqual(now.Minute, task.UpdatedDate.Minute);
            Assert.AreEqual(UserName, task.UpdatedBy);
        }

        [TestMethod]
        public void TaskService_PendingAllocatePinOperation_ReturnsTrueForDateAfterPendingAllocatePinOperation()
        {
            var result = _taskService.PendingAllocatePinOperation(12345, DateTime.Now);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TaskService_PendingAllocatePinOperation_ReturnsFalseForDateBeforePendingAllocatePinOperation()
        {
            var result = _taskService.PendingAllocatePinOperation(12345, DateTime.Now.AddDays(-4));

            Assert.IsFalse(result);
        }


        [TestMethod]
        public void TaskService_PendingAllocatePinOperation_ReturnsFalseForNoPendingAllocatePinOperation()
        {
            var result = _taskService.PendingAllocatePinOperation(67890, DateTime.Now);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TaskService_CompleteUnrecoverableTaskException_WithTimerLockCallsTimerLockDeleteAndPerformCorrectCallsIncludingUnitOfWorkSave()
        {
            const string errorMessage = "An Error Occurred";
            _taskService.CompleteUnrecoverableTaskException(_tasks[3], errorMessage);

            _mockTimerLockRepository.Verify(v => v.Delete(It.IsAny<TimerLock>()), Times.Once);
            _mockTaskRepository.Verify(v => v.Delete(It.IsAny<Task>()), Times.Once);
            _mockHistoryLogRepository.Verify(v => v.Insert(It.IsAny<HistoryLog>()), Times.Once);
            _mockUnitOfWork.Verify(v => v.Save(), Times.Once);
        }

        [TestMethod]
        public void TaskService_CompleteUnrecoverableTaskException_NoTimerLockDoesNotCallTimerLockDeleteAndThenPerformsRemaimingCallsUnitOfWorkSave()
        {
            const string errorMessage = "An Error Occurred";
            _taskService.CompleteUnrecoverableTaskException(_tasks[4], errorMessage);

            _mockTimerLockRepository.Verify(v => v.Delete(It.IsAny<TimerLock>()), Times.Never);
            _mockTaskRepository.Verify(v => v.Delete(It.IsAny<Task>()), Times.Once);
            _mockHistoryLogRepository.Verify(v => v.Insert(It.IsAny<HistoryLog>()), Times.Once);
            _mockUnitOfWork.Verify(v => v.Save(), Times.Once);
        }

        [TestMethod]
        public void TaskService_PendingMergeFromPinOperation_ReturnsTrueForDateAfterPendingMergePinOperation()
        {
            var result = _taskService.PendingMergeFromPinOperation(12345, DateTime.Now);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TaskService_PendingMergeFromPinOperation_ReturnsFalseForDateBeforePendingMergePinOperation()
        {
            var result = _taskService.PendingMergeFromPinOperation(12345, DateTime.Now.AddDays(-4));

            Assert.IsFalse(result);
        }


        [TestMethod]
        public void TaskService_PendingMergeFromPinOperation_ReturnsFalseForNoPendingMergePinOperation()
        {
            var result = _taskService.PendingMergeFromPinOperation(67890, DateTime.Now);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TaskService_PendingMergeToPinOperation_ReturnsTrueForDateAfterPendingMergePinOperation()
        {
            var result = _taskService.PendingMergeToPinOperation(12345, DateTime.Now);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TaskService_PendingMergeToPinOperation_ReturnsFalseForDateBeforePendingMergePinOperation()
        {
            var result = _taskService.PendingMergeToPinOperation(12345, DateTime.Now.AddDays(-4));

            Assert.IsFalse(result);
        }


        [TestMethod]
        public void TaskService_PendingMergeToPinOperation_ReturnsFalseForNoPendingMergePinOperation()
        {
            var result = _taskService.PendingMergeToPinOperation(56789, DateTime.Now);

            Assert.IsFalse(result);
        }


        [TestMethod]
        public void TaskService_CompleteTask_WithTimerLockRemovesTimerLock()
        {
            _taskService.CompleteTask(_tasks[3], true);
            _mockTimerLockRepository.Verify(v => v.Delete(It.IsAny<TimerLock>()), Times.Once);
        }

        [TestMethod]
        public void TaskService_CompleteTask_WithoutTimerLockNeverRemovesTimerLock()
        {
            _taskService.CompleteTask(_tasks[2], true);
            _mockTimerLockRepository.Verify(v => v.Delete(It.IsAny<TimerLock>()), Times.Never);
        }

        [TestMethod]
        public void TaskService_CompleteTask_OneTimeTaskDeletesTaskOnCompletion()
        {
            _taskService.CompleteTask(_tasks[2], true);
            _mockTaskRepository.Verify(v => v.Delete(It.IsAny<Task>()), Times.Once);
        }

        [TestMethod]
        public void TaskService_CompleteTask_DailyTaskUpdatesTaskOnCompletion()
        {
            _taskService.CompleteTask(_tasks[1], true);
            _mockTaskRepository.Verify(v => v.Update(It.IsAny<Task>()), Times.Once);
        }

        [TestMethod]
        public void TaskService_CompleteTask_InsertsIntoHistoryLog()
        {
            _taskService.CompleteTask(_tasks[3], true);
            _mockHistoryLogRepository.Verify(v => v.Insert(It.IsAny<HistoryLog>()), Times.Once);
        }

        [TestMethod]
        public void TaskService_CompleteTask_SavesUnitOfWork()
        {
            _taskService.CompleteTask(_tasks[3], true);
            _mockHistoryLogRepository.Verify(v => v.Insert(It.IsAny<HistoryLog>()), Times.Once);
        }
    }
}
