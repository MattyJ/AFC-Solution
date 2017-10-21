using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Fujitsu.AFC.Core.Interfaces;
using Fujitsu.AFC.Data.Interfaces;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Services.Interfaces;
using Fujitsu.AFC.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fujitsu.AFC.Services.Tests
{
    [TestClass]
    public class TimerLockServiceTests
    {
        private Mock<IRepository<TimerLock>> _mockTimerLockRepository;
        private Mock<IUserIdentity> _mockUserIdentity;
        private Mock<IUnitOfWork> _mockUnitOfWork;

        private List<TimerLock> _timerLocks;

        private ITimerLockService _timerLockService;

        private const string UserName = "matthew.jordan@uk.fujitsu.com";
        private readonly DateTime _dateTime = DateTime.Now;
        private readonly Guid _guidOne = new Guid();
        private readonly Guid _guidTwo = new Guid();
        private readonly int _lockedPinOne = 12345;
        private readonly int _lockedPinTwo = 67890;



        [TestInitialize]
        public void Intialize()
        {


            _timerLocks = new List<TimerLock>
            {
                new TimerLock
                {

                    Id = 1,
                    LockedInstance = _guidOne,
                    LockedPin = _lockedPinOne,
                    TaskId = 1,
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime
                },
                new TimerLock
                {

                    Id = 2,
                    LockedInstance = _guidTwo,
                    LockedPin = _lockedPinTwo,
                    TaskId = 2,
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime
                },

        };

            _mockUserIdentity = new Mock<IUserIdentity>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockTimerLockRepository = MockRepositoryHelper.Create(_timerLocks, (entity, id) => entity.Id == (int)id);

            _timerLockService = new TimerLockService(_mockTimerLockRepository.Object, _mockUserIdentity.Object, _mockUnitOfWork.Object);
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TimerLockService_Constructor_NoTaskRepository()
        {
            #region Arrange

            #endregion

            #region Act

            new TimerLockService(
                null,
                _mockUserIdentity.Object,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TimerLockService_Constructor_NoUserIdentity()
        {
            #region Arrange

            #endregion

            #region Act

            new TimerLockService(
                _mockTimerLockRepository.Object,
                null,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TimerLockService_Constructor_NoUnitOfWork()
        {
            #region Arrange

            #endregion

            #region Act

            new TimerLockService(
                _mockTimerLockRepository.Object,
                _mockUserIdentity.Object,
                null);

            #endregion

            #region Assert

            #endregion
        }

        #endregion

        [TestMethod]
        public void TimerLockService_Create_CallsInsertAndUnitOfWorkSave()
        {
            #region Arrange

            var timerLock = new TimerLock
            {

                Id = 1,
                LockedInstance = new Guid(),
                LockedPin = 12345,
                TaskId = 1,
                InsertedBy = UserName,
                InsertedDate = _dateTime,
                UpdatedBy = UserName,
                UpdatedDate = _dateTime
            };

            #endregion

            #region Act

            var response = _timerLockService.Create(timerLock);

            #endregion

            #region Assert

            _mockTimerLockRepository.Verify(x => x.Insert(It.IsAny<TimerLock>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void TimerLockService_Update_CallsUpdateAndUnitOfWorkSave()
        {
            #region Arrange

            var timerLock = new TimerLock
            {
                Id = 1,
                LockedPin = 23456,
            };

            #endregion

            #region Act

            _timerLockService.Update(timerLock);

            #endregion

            #region Assert

            _mockTimerLockRepository.Verify(x => x.Update(It.IsAny<TimerLock>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void TimerLockService_Delete_ByEntity_CallsDeleteAndUnitOfWorkSave()
        {
            #region Arrange

            var timerLock = new TimerLock
            {
                Id = 1
            };

            #endregion

            #region Act

            _timerLockService.Delete(timerLock);

            #endregion

            #region Assert

            _mockTimerLockRepository.Verify(x => x.Delete(It.IsAny<TimerLock>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void TimerLockService_Delete_ById_CallsDeleteAndUnitOfWorkSave()
        {
            #region Arrange

            #endregion

            #region Act

            _timerLockService.Delete(1);

            #endregion

            #region Assert

            _mockTimerLockRepository.Verify(x => x.Delete(It.IsAny<int>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void TimerLockService_GetAll_CallsRepositoryAll()
        {
            #region Arrange

            #endregion

            #region Act

            var timerLocks = _timerLockService.All();

            #endregion

            #region Assert

            _mockTimerLockRepository.Verify(x => x.All(), Times.Once);
            Assert.AreEqual(_timerLocks.Count, timerLocks.Count());

            #endregion
        }

        [TestMethod]
        public void TimerLockService_GetById_CallsRepositoryGetById()
        {
            #region Arrange

            #endregion

            #region Act

            var timerLock = _timerLockService.GetById(1);

            #endregion

            #region Assert

            _mockTimerLockRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(1, timerLock.Id);

            #endregion
        }

        [TestMethod]
        public void TimerLockService_Query_CallsRepositoryQuery()
        {
            #region Arrange

            #endregion

            #region Act

            var query = _timerLockService.Query(x => x.Id == 1).ToList();

            #endregion

            #region Assert


            Assert.IsNotNull(query);
            _mockTimerLockRepository.Verify(x => x.Query(It.IsAny<Expression<Func<TimerLock, bool>>>()), Times.Once());

            #endregion
        }

        [TestMethod]
        public void TimerLockService_AcquireLock_NewLockCreatesNewLock_CallsInsertAndUnitOfWorkSave()
        {
            #region Arrange

            #endregion

            #region Act

            _timerLockService.AcquireLock(new Guid(), 99999, 10);

            #endregion

            #region Assert

            _mockTimerLockRepository.Verify(x => x.Insert(It.IsAny<TimerLock>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void TimerLockService_AcquireLock_ZeroPinIdDoesNotCallInsertOrUnitOfWork()
        {
            #region Arrange

            #endregion

            #region Act

            _timerLockService.AcquireLock(_guidOne, 0, 1);

            #endregion

            #region Assert

            _mockTimerLockRepository.Verify(x => x.Insert(It.IsAny<TimerLock>()), Times.Never);
            _mockUnitOfWork.Verify(x => x.Save(), Times.Never);

            #endregion
        }
    }
}
