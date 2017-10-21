using System;
using System.Collections.Generic;
using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Data.Handlers;
using Fujitsu.AFC.Data.Interfaces;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fujitsu.AFC.Data.Tests
{
    [TestClass]
    public class TestHandlerTests
    {
        private Mock<IRepository<TimerLock>> _mockTimerLockRepository;

        private TaskHandler _taskHandler;

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
                    LockedInstance = _serviceInstanceOne,
                    LockedPin = _lockedPinOne
                }
            };

            _mockTimerLockRepository = MockRepositoryHelper.Create(_timerLocks, (entity, id) => entity.Id == (int)id);

            _taskHandler = new TestTaskHandler();
        }

        [TestMethod]
        public void TaskHandler_CanExecute_TaskIsOneTimeThereforeNoNextScheduledDate_ReturnsTrue()
        {
            var newTask = new Task
            {
                Name = TaskNames.AllocatePin,
                Frequency = TaskFrequencyNames.OneTime,
                Pin = 12125,
            };

            Assert.IsTrue(_taskHandler.CanExecute(newTask, _serviceInstanceTwo));
        }

        [TestMethod]
        public void TaskHandler_CanExecute_TaskIsDailyNextScheduledDateInPast_ReturnsTrue()
        {
            var newTask = new Task
            {
                Name = TaskNames.AllocatePin,
                Frequency = TaskFrequencyNames.Daily,
                Pin = 12125,
                NextScheduledDate = DateTime.Now.AddDays(-1)
            };

            Assert.IsTrue(_taskHandler.CanExecute(newTask, null));
        }

        [TestMethod]
        public void TaskHandler_CanExecute_TaskIsDailyNextScheduledDateInFuture_ReturnsFalse()
        {
            var newTask = new Task
            {
                Name = TaskNames.AllocatePin,
                Frequency = TaskFrequencyNames.Daily,
                Pin = 12125,
                NextScheduledDate = DateTime.Now.AddDays(+1)
            };

            Assert.IsFalse(_taskHandler.CanExecute(newTask, null));
        }

        [TestMethod]
        public void TaskHandler_CanExecute_TaskIsWeeklyNextScheduledDateInPast_ReturnsTrue()
        {
            var newTask = new Task
            {
                Name = TaskNames.AllocatePin,
                Frequency = TaskFrequencyNames.Weekly,
                Pin = 12125,
                NextScheduledDate = DateTime.Now.AddDays(-1)
            };

            Assert.IsTrue(_taskHandler.CanExecute(newTask, null));
        }

        [TestMethod]
        public void TaskHandler_CanExecute_TaskIsWeeklyNextScheduledDateInFuture_ReturnsFalse()
        {
            var newTask = new Task
            {
                Name = TaskNames.AllocatePin,
                Frequency = TaskFrequencyNames.Weekly,
                Pin = 12125,
                NextScheduledDate = DateTime.Now.AddDays(+1)
            };

            Assert.IsFalse(_taskHandler.CanExecute(newTask, null));
        }

        [TestMethod]
        public void TaskHandler_CanExecute_TaskIsMonthlyNextScheduledDateInPast_ReturnsTrue()
        {
            var newTask = new Task
            {
                Name = TaskNames.AllocatePin,
                Frequency = TaskFrequencyNames.Monthly,
                Pin = 12125,
                NextScheduledDate = DateTime.Now.AddDays(-1)
            };

            Assert.IsTrue(_taskHandler.CanExecute(newTask, null));
        }

        [TestMethod]
        public void TaskHandler_CanExecute_TaskMonthlyNextScheduledDateInFuture_ReturnsFalse()
        {
            var newTask = new Task
            {
                Name = TaskNames.AllocatePin,
                Frequency = TaskFrequencyNames.Monthly,
                Pin = 12125,
                NextScheduledDate = DateTime.Now.AddDays(+1)
            };

            Assert.IsFalse(_taskHandler.CanExecute(newTask, null));
        }

        [TestMethod]
        public void TaskHandler_CanExecute_TaskYearlyNextScheduledDateInPast_ReturnsTrue()
        {
            var newTask = new Task
            {
                Name = TaskNames.AllocatePin,
                Frequency = TaskFrequencyNames.Yearly,
                Pin = 12125,
                NextScheduledDate = DateTime.Now.AddDays(-1)
            };

            Assert.IsTrue(_taskHandler.CanExecute(newTask, null));
        }

        [TestMethod]
        public void TaskHandler_CanExecute_TaskYearlyNextScheduledDateInFuture_ReturnsFalse()
        {
            var newTask = new Task
            {
                Name = TaskNames.AllocatePin,
                Frequency = TaskFrequencyNames.Yearly,
                Pin = 12125,
                NextScheduledDate = DateTime.Now.AddDays(+1)
            };

            Assert.IsFalse(_taskHandler.CanExecute(newTask, null));
        }
    }

    internal class TestTaskHandler : TaskHandler
    {

        public override void Execute(Task task, Guid? serviceInstanceId)
        {
        }

        public override void Dispose()
        {

        }
    }
}
