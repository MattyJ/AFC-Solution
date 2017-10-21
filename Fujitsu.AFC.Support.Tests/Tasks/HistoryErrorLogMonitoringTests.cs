using System;
using System.Collections.Generic;
using System.Linq;
using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Data.Interfaces;
using Fujitsu.AFC.Model;
using Moq;
using Fujitsu.AFC.Services.Interfaces;
using Fujitsu.AFC.Support.Interfaces;
using Fujitsu.AFC.Support.Tasks;
using Fujitsu.AFC.Tasks.Resources;
using Fujitsu.AFC.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.AFC.Support.Tests.Tasks
{
    [TestClass]
    public class HistoryErrorLogMonitoringTests
    {
        private Mock<IService<HistoryLog>> _mockHistoryLogService;
        private Mock<ISupportService> _mockSupportService;
        private Mock<IRepository<HistoryLog>> _mockHistoryLogRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private List<HistoryLog> _historyLogs;

        private ISupportTaskProcessor _supportTaskProcessor;

        private const string UserName = "matthew.jordan@uk.fujitsu.com";
        private readonly DateTime _dateTime = DateTime.Now;

        private const int PIN = 12345;

        [TestInitialize]
        public void TestInitialize()
        {
            _historyLogs = new List<HistoryLog>
            {
                new HistoryLog
                {
                    Id = 1,
                    Name = TaskNames.AllocatePin,
                    Handler = TaskHandlerNames.OperationsHandler,
                    EventDetail =
                        string.Format(TaskResources.OperationsTaskRequest_InvalidRequestNoPin, TaskNames.AllocatePin),
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime,
                    Escalated = false
                },
                new HistoryLog
                {
                    Id = 2,
                    Name = TaskNames.AllocatePin,
                    Handler = TaskHandlerNames.OperationsHandler,
                    EventDetail =
                        string.Format(TaskResources.OperationsTaskRequest_InvalidRequestNoSiteTitle,
                            TaskNames.AllocatePin, PIN),
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime,
                    Escalated = false
                },
                new HistoryLog
                {
                    Id = 3,
                    Name = TaskNames.AllocatePin,
                    Handler = TaskHandlerNames.OperationsHandler,
                    EventDetail =
                        string.Format(TaskResources.OperationsTaskRequest_InvalidRequestNoSiteTitle,
                            TaskNames.AllocatePin, PIN),
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime,
                    Escalated = true

                }
            };

            _mockSupportService = new Mock<ISupportService>();
            _mockHistoryLogService = new Mock<IService<HistoryLog>>();
            _mockHistoryLogRepository = MockRepositoryHelper.Create(_historyLogs, (entity, id) => entity.Id == (int)id);
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _supportTaskProcessor = new HistoryErrorLogMonitoring(_mockSupportService.Object, _mockHistoryLogService.Object);
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HistoryErrorLogMonitoringConstructor_NoSupportService()
        {
            #region Arrange

            #endregion

            #region Act

            new HistoryErrorLogMonitoring(null, _mockHistoryLogService.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HistoryErrorLogMonitoringConstructor_NoHistoryLogService()
        {
            #region Arrange

            #endregion

            #region Act

            new HistoryErrorLogMonitoring(_mockSupportService.Object, null);

            #endregion

            #region Assert

            #endregion
        }

        #endregion

        [TestMethod]
        public void HistoryErrorLogMonitoringExecute_EscalatesCorrectNumberOfDeadTasks()
        {
            #region Arrange

            var escalationTasks = _historyLogs.Where(x => x.Escalated == false).AsQueryable();
            _mockHistoryLogService.Setup(s => s.Query(x => x.Escalated == false)).Returns(escalationTasks);

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.Daily,
                Handler = TaskHandlerNames.SupportHandler,
                Name = TaskNames.HistoryErrorLogMonitoring
            };

            #endregion

            #region Act

            _supportTaskProcessor.Execute(task);

            #endregion

            #region Assert

            _mockSupportService.Verify(v => v.EscalateErrorEvent(It.IsAny<HistoryLog>()), Times.Exactly(escalationTasks.ToList().Count));

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void HistoryErrorLogMonitoringExecute_TaskThrowsException_ExceptionIsPropogated()
        {
            #region Arrange


            var logs = _historyLogs.Where(x => x.Escalated == false).AsQueryable();
            _mockHistoryLogService.Setup(s => s.Query(x => x.Escalated == false)).Returns(logs);
            _mockSupportService.Setup(s => s.EscalateErrorEvent(It.IsAny<HistoryLog>())).Throws(new Exception());

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.Daily,
                Handler = TaskHandlerNames.SupportHandler,
                Name = TaskNames.HistoryErrorLogMonitoring
            };

            #endregion

            #region Act

            _supportTaskProcessor.Execute(task);

            #endregion
        }
    }
}
