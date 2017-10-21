using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Fujitsu.AFC.Constants;
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
    public class HistoryLogServiceTests
    {
        private Mock<IRepository<HistoryLog>> _mockHistoryLogRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;

        private List<HistoryLog> _historyLogs;

        private IService<HistoryLog> _historyLogService;

        private const string UserName = "matthew.jordan@uk.fujitsu.com";
        private readonly DateTime _dateTime = DateTime.Now;

        [TestInitialize]
        public void Intialize()
        {


            _historyLogs = new List<HistoryLog>
            {
                new HistoryLog
                {
                    Id = 1,
                    Frequency = TaskFrequencyNames.Daily,
                    Handler = TaskHandlerNames.ProvisioningHandler,
                    Name = TaskNames.CaseSiteProvisioning,
                    CompletedDate = DateTime.Now.AddMinutes(-10),
                    InsertedBy = UserName,
                    InsertedDate = _dateTime.AddDays(-1),
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime.AddDays(-1),
                },
                new HistoryLog
                {
                    Frequency = TaskFrequencyNames.OneTime,
                    Handler = TaskHandlerNames.OperationsHandler,
                    Name = TaskNames.AllocatePin,
                    CompletedDate = DateTime.Now.AddMinutes(-10),
                    InsertedBy = UserName,
                    InsertedDate = _dateTime.AddDays(-1),
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime.AddDays(-1),
                },
                new HistoryLog
                {
                    Frequency = TaskFrequencyNames.OneTime,
                    Handler = TaskHandlerNames.OperationsHandler,
                    Name = TaskNames.AllocateCase,
                    CompletedDate = DateTime.Now.AddMinutes(-10),
                    InsertedBy = UserName,
                    InsertedDate = _dateTime.AddDays(-1),
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime.AddDays(-1),
                },
                new HistoryLog
                {
                    Frequency = TaskFrequencyNames.Daily,
                    Handler = TaskHandlerNames.SupportHandler,
                    Name = TaskNames.HistoryErrorLogMonitoring,
                    CompletedDate = DateTime.Now.AddMinutes(-10),
                    InsertedBy = UserName,
                    InsertedDate = _dateTime.AddDays(-1),
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime.AddDays(-1),
                },

            };



            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockHistoryLogRepository = MockRepositoryHelper.Create(_historyLogs, (entity, id) => entity.Id == (int)id);

            _historyLogService = new HistoryLogService(
                _mockHistoryLogRepository.Object, _mockUnitOfWork.Object);

            Bootstrapper.Initialise();
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HistoryLogService_Constructor_NoHistoryLogRepository()
        {
            #region Arrange

            #endregion

            #region Act

            new HistoryLogService(
                null,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HistoryLogService_Constructor_NoUnitOfWork()
        {
            #region Arrange

            #endregion

            #region Act

            new HistoryLogService(
                _mockHistoryLogRepository.Object,
                null);

            #endregion

            #region Assert

            #endregion
        }

        #endregion

        [TestMethod]
        public void HistoryLogService_Create_CallsInsertAndSaveChanges()
        {
            #region Arrange

            var historyLog = new HistoryLog
            {
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.AllocateCase,
                CompletedDate = DateTime.Now.AddMinutes(-10),
                InsertedBy = UserName,
                InsertedDate = _dateTime.AddDays(-1),
                UpdatedBy = UserName,
                UpdatedDate = _dateTime.AddDays(-1),
            };

            #endregion

            #region Act

            _historyLogService.Create(historyLog);

            #endregion

            #region Assert

            _mockHistoryLogRepository.Verify(x => x.Insert(It.IsAny<HistoryLog>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void HistoryLogService_Update_CallUpdateAndUnitOfWorkSave()
        {
            #region Arrange

            var historyLog = new HistoryLog
            {
                Id = 1,
            };

            #endregion

            #region Act

            _historyLogService.Update(historyLog);

            #endregion

            #region Assert

            _mockHistoryLogRepository.Verify(x => x.Update(It.IsAny<HistoryLog>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void HistoryLogService_Delete_ByEntity_CallsDeleteAndUnitOfWorkSave()
        {
            #region Arrange

            var historyLog = new HistoryLog
            {
                Id = 1
            };

            #endregion

            #region Act

            _historyLogService.Delete(historyLog);

            #endregion

            #region Assert

            _mockHistoryLogRepository.Verify(x => x.Delete(It.IsAny<HistoryLog>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void HistoryLogService_Delete_ById_CallsDeleteAndUnitOfWorkSave()
        {
            #region Arrange

            #endregion

            #region Act

            _historyLogService.Delete(1);

            #endregion

            #region Assert

            _mockHistoryLogRepository.Verify(x => x.Delete(It.IsAny<int>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void HistoryLogService_GetAll_CallsRepositoryAll()
        {
            #region Arrange

            #endregion

            #region Act

            var historyLogs = _historyLogService.All();

            #endregion

            #region Assert

            _mockHistoryLogRepository.Verify(x => x.All(), Times.Once);
            Assert.AreEqual(_historyLogs.Count, historyLogs.Count());

            #endregion
        }

        [TestMethod]
        public void HistoryLogService_GetById_CallsRepositoryGetById()
        {
            #region Arrange

            #endregion

            #region Act

            var historyLog = _historyLogService.GetById(1);

            #endregion

            #region Assert

            _mockHistoryLogRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(1, historyLog.Id);

            #endregion
        }

        [TestMethod]
        public void HistoryLogService_Query_CallsRepositoryQuery()
        {
            #region Arrange

            #endregion

            #region Act

            var query = _historyLogService.Query(x => x.Id == 1).ToList();

            #endregion

            #region Assert


            Assert.IsNotNull(query);
            _mockHistoryLogRepository.Verify(x => x.Query(It.IsAny<Expression<Func<HistoryLog, bool>>>()), Times.Once());

            #endregion
        }
    }
}
