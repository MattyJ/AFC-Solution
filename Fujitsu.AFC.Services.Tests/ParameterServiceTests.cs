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
    public class ParameterServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IUserIdentity> _mockUserIdentity;
        private Mock<ICacheManager> _mockCacheManager;
        private Mock<IRepository<Parameter>> _mockParameterRepository;
        private const string UserName = "matthew.jordan@uk.fujitsu.com";
        private List<Parameter> _parameters;
        private IParameterService _parameterService;
        private const int ExistingId = 10;
        private const string ExistingParameterName = "HanSolo";
        private const string ExistingParameterValue = "10.867";

        [TestInitialize]
        public void Initialize()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockCacheManager = new Mock<ICacheManager>();

            _mockUserIdentity = new Mock<IUserIdentity>();
            _mockUserIdentity.Setup(s => s.Name).Returns(UserName);

            _parameters = new List<Parameter>
            {
                UnitTestHelper.GenerateRandomData<Parameter>(),
                UnitTestHelper.GenerateRandomData<Parameter>(),
                UnitTestHelper.GenerateRandomData<Parameter>(x => x.Id = ExistingId),
                UnitTestHelper.GenerateRandomData<Parameter>(x =>
                {
                    x.Name = ExistingParameterName;
                    x.Value = ExistingParameterValue;
                }),
                UnitTestHelper.GenerateRandomData<Parameter>()
            };

            _mockParameterRepository = MockRepositoryHelper.Create(_parameters,
                (entity, id) => entity.Id == (int)id,
                (p1, p2) => p1.Id == p2.Id);

            _parameterService = new ParameterService(_mockParameterRepository.Object,
                _mockUnitOfWork.Object,
                _mockUserIdentity.Object,
                _mockCacheManager.Object);
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParameterService_Constructor_ParameterRepositoryIsNull_ThrowsException()
        {
            new ParameterService(null,
                _mockUnitOfWork.Object,
                _mockUserIdentity.Object,
                _mockCacheManager.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParameterService_Constructor_UnitOfWorkIsNull_ThrowsException()
        {
            new ParameterService(_mockParameterRepository.Object,
                null,
                _mockUserIdentity.Object,
                _mockCacheManager.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParameterService_Constructor_UserIdentityIsNull_ThrowsException()
        {
            new ParameterService(_mockParameterRepository.Object,
                _mockUnitOfWork.Object,
                null,
                _mockCacheManager.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParameterService_Constructor_CacheManagerIsNull_ThrowsException()
        {
            new ParameterService(_mockParameterRepository.Object,
                _mockUnitOfWork.Object,
                _mockUserIdentity.Object,
                null);
        }

        #endregion

        [TestMethod]
        public void ParameterService_All_ReturnsAllParameters()
        {
            var result = _parameterService.All();
            Assert.AreEqual(5, result.Count());
        }

        [TestMethod]
        public void ParameterService_Create_NewParameterEntityPassed_IsAddedToCollection()
        {
            var p = UnitTestHelper.GenerateRandomData<Parameter>(x => x.Name = "XXX");
            _parameterService.Create(p);
            Assert.IsTrue(_parameters.Any(x => x.Name == "XXX"));
        }

        [TestMethod]
        public void ParameterService_Create_NewParameterEntityPassed_UnitOfWorkCalled()
        {
            var p = UnitTestHelper.GenerateRandomData<Parameter>(x => x.Name = "XXX");
            _parameterService.Create(p);
            _mockUnitOfWork.Verify(v => v.Save(), Times.Once);
        }

        [TestMethod]
        public void ParameterService_Update_ExistingParameterEntityPassed_CacheIsInvalidated()
        {
            var p = UnitTestHelper.GenerateRandomData<Parameter>();
            _parameterService.Update(p);
            _mockCacheManager.Verify(v => v.Remove(p.Name));
        }

        [TestMethod]
        public void ParameterService_Update_ExistingParameterEntityPassed_ParameterIsUpdated()
        {
            var existing = _parameters.First(x => x.Id == ExistingId);
            var p = UnitTestHelper.GenerateRandomData<Parameter>(x =>
            {
                x.Id = existing.Id;
                x.Name = existing.Name;
                x.Value = "XXX666";
            });
            _parameterService.Update(p);
            existing = existing = _parameters.First(x => x.Id == ExistingId);
            Assert.AreEqual("XXX666", existing.Value);
        }

        [TestMethod]
        public void ParameterService_Update_ExistingParameterEntityPassed_UnitOfWorkCalled()
        {
            var p = UnitTestHelper.GenerateRandomData<Parameter>();
            _parameterService.Update(p);
            _mockUnitOfWork.Verify(v => v.Save(), Times.Once);
        }

        [TestMethod]
        public void ParameterService_Delete_ExistingParameterEntityPassed_IsRemovedFromCollection()
        {
            var p = _parameters.First(x => x.Id == ExistingId);
            _parameterService.Delete(p);
            Assert.AreEqual(4, _parameters.Count);
        }

        [TestMethod]
        public void ParameterService_Delete_ParameterEntityDoesNotExist_NothingIsRemovedFromCollection()
        {
            var p = UnitTestHelper.GenerateRandomData<Parameter>();
            _parameterService.Delete(p);
            Assert.AreEqual(5, _parameters.Count);
        }

        [TestMethod]
        public void ParameterService_Delete_ExistingParameterEntityPassed_UnitOfWorkCalled()
        {
            var p = _parameters.First(x => x.Id == ExistingId);
            _parameterService.Delete(p);
            _mockUnitOfWork.Verify(v => v.Save(), Times.Once);
        }

        [TestMethod]
        public void ParameterService_Delete_ExistingParameterIdPassed_IsRemovedFromCollection()
        {
            _parameterService.Delete(ExistingId);
            Assert.AreEqual(4, _parameters.Count);
        }

        [TestMethod]
        public void ParameterService_Delete_ParameterIdDoesNotExist_NothingIsRemovedFromCollection()
        {
            _parameterService.Delete(999);
            Assert.AreEqual(5, _parameters.Count);
        }

        [TestMethod]
        public void ParameterService_Delete_ExistingParameterIdPassed_GetByIdAndUnitOfWorkCalled()
        {
            _parameterService.Delete(ExistingId);
            _mockParameterRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
            _mockUnitOfWork.Verify(v => v.Save(), Times.Once);
        }



        [TestMethod]
        public void ParameterService_Find_ParameterNameExists_ParameterEntityIsReturned()
        {
            var expected = _parameters.Single(x => x.Name == ExistingParameterName);
            var result = _parameterService.Find(ExistingParameterName);
            Assert.AreEqual(expected.Id, result.Id);
        }

        [TestMethod]
        public void ParameterService_Find_ParameterNameDoesNotExists_NullIsReturned()
        {
            var result = _parameterService.Find("XXX666");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ParameterService_GetParameterByName_ParameterNameExists_ReturnedTypeIsDecimal()
        {
            var result = _parameterService.GetParameterByName<decimal>(ExistingParameterName);
            Assert.IsInstanceOfType(result, typeof(decimal));
        }

        [TestMethod]
        public void ParameterService_GetParameterByName_ParameterNameExists_ReturnedTypeIsExpectedValue()
        {
            var expected = Decimal.Parse(ExistingParameterValue);
            var result = _parameterService.GetParameterByName<decimal>(ExistingParameterName);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ParameterService_GetParameterByNameOrCreate_ParameterNameExists_ReturnedTypeIsExpectedValue()
        {
            var expected = Decimal.Parse(ExistingParameterValue);
            var result = _parameterService.GetParameterByNameOrCreate<decimal>(ExistingParameterName, 6);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ParameterService_GetParameterByNameOrCreate_ParameterNameDoesNotExist_NewParameterEntryIsCreated()
        {
            const string parameterName = "XXX666";
            _parameterService.GetParameterByNameOrCreate<decimal>("XXX666", 6);
            Assert.IsTrue(_parameters.Any(x => x.Name == parameterName));
        }

        [TestMethod]
        public void ParameterService_GetParameterByNameOrCreate_ParameterNameDoesNotExist_NewParameterEntryHasCorrectParameterValue()
        {
            const string parameterName = "XXX666";
            _parameterService.GetParameterByNameOrCreate<decimal>("XXX666", 6);
            var result = _parameters.Single(x => x.Name == parameterName);
            Assert.AreEqual("6", result.Value);
        }

        [TestMethod]
        public void ParameterService_GetParameterByNameOrCreate_ParameterNameDoesNotExist_NewParameterEntryHasInsertedBy()
        {
            const string parameterName = "XXX666";
            _parameterService.GetParameterByNameOrCreate<decimal>("XXX666", 6);
            var result = _parameters.Single(x => x.Name == parameterName);
            Assert.AreEqual(UserName, result.InsertedBy);
        }

        [TestMethod]
        public void ParameterService_GetParameterByNameOrCreate_ParameterNameDoesNotExist_NewParameterEntryHasInsertedDate()
        {
            var now = DateTime.Now;
            const string parameterName = "XXX666";
            _parameterService.GetParameterByNameOrCreate<decimal>("XXX666", 6);
            var result = _parameters.Single(x => x.Name == parameterName);
            Assert.AreEqual(now.Year, result.InsertedDate.Year);
            Assert.AreEqual(now.Month, result.InsertedDate.Month);
            Assert.AreEqual(now.Day, result.InsertedDate.Day);
            Assert.AreEqual(now.Hour, result.InsertedDate.Hour);
            Assert.AreEqual(now.Minute, result.InsertedDate.Minute);
        }

        [TestMethod]
        public void ParameterService_GetParameterByNameOrCreate_ParameterNameDoesNotExist_NewParameterEntryHasUpdatedBy()
        {
            const string parameterName = "XXX666";
            _parameterService.GetParameterByNameOrCreate<decimal>("XXX666", 6);
            var result = _parameters.Single(x => x.Name == parameterName);
            Assert.AreEqual(UserName, result.UpdatedBy);
        }

        [TestMethod]
        public void ParameterService_GetParameterByNameOrCreate_ParameterNameDoesNotExist_NewParameterEntryHasUpdatedDate()
        {
            var now = DateTime.Now;
            const string parameterName = "XXX666";
            _parameterService.GetParameterByNameOrCreate<decimal>("XXX666", 6);
            var result = _parameters.Single(x => x.Name == parameterName);
            Assert.AreEqual(now.Year, result.UpdatedDate.Year);
            Assert.AreEqual(now.Month, result.UpdatedDate.Month);
            Assert.AreEqual(now.Day, result.UpdatedDate.Day);
            Assert.AreEqual(now.Hour, result.UpdatedDate.Hour);
            Assert.AreEqual(now.Minute, result.UpdatedDate.Minute);
        }

        [TestMethod]
        public void ParameterService_SaveParameter_ParameterNameExist_CacheIsInvalidated()
        {
            const string parameterName = "XXX666";
            _parameterService.SaveParameter(parameterName, 6);
            _mockCacheManager.Verify(v => v.Remove(parameterName));
        }

        [TestMethod]
        public void ParameterService_SaveParameter_ParameterNameDoesNotExist_NewParameterIsCreated()
        {
            const string parameterName = "XXX666";
            _parameterService.SaveParameter(parameterName, 6);
            Assert.IsTrue(_parameters.Any(x => x.Name == parameterName));
        }

        [TestMethod]
        public void ParameterService_SaveParameter_ParameterNameDoesNotExist_NewParameterHasCorrectValue()
        {
            const string parameterName = "XXX666";
            _parameterService.SaveParameter(parameterName, 6);
            var result = _parameters.Single(x => x.Name == parameterName);
            Assert.AreEqual("6", result.Value);
        }

        [TestMethod]
        public void ParameterService_SaveParameter_ParameterNameDoesNotExist_NewParameterEntryHasInsertedBy()
        {
            const string parameterName = "XXX666";
            _parameterService.SaveParameter("XXX666", 6);
            var result = _parameters.Single(x => x.Name == parameterName);
            Assert.AreEqual(UserName, result.InsertedBy);
        }

        [TestMethod]
        public void ParameterService_SaveParameter_ParameterNameDoesNotExist_NewParameterEntryHasInsertedDate()
        {
            var now = DateTime.Now;
            const string parameterName = "XXX666";
            _parameterService.SaveParameter("XXX666", 6);
            var result = _parameters.Single(x => x.Name == parameterName);
            Assert.AreEqual(now.Year, result.InsertedDate.Year);
            Assert.AreEqual(now.Month, result.InsertedDate.Month);
            Assert.AreEqual(now.Day, result.InsertedDate.Day);
            Assert.AreEqual(now.Hour, result.InsertedDate.Hour);
            Assert.AreEqual(now.Minute, result.InsertedDate.Minute);
        }

        [TestMethod]
        public void ParameterService_SaveParameter_ParameterNameDoesNotExist_NewParameterEntryHasUpdatedBy()
        {
            const string parameterName = "XXX666";
            _parameterService.SaveParameter("XXX666", 6);
            var result = _parameters.Single(x => x.Name == parameterName);
            Assert.AreEqual(UserName, result.UpdatedBy);
        }

        [TestMethod]
        public void ParameterService_SaveParameter_ParameterNameDoesNotExist_NewParameterEntryHasUpdatedDate()
        {
            var now = DateTime.Now;
            const string parameterName = "XXX666";
            _parameterService.SaveParameter("XXX666", 6);
            var result = _parameters.Single(x => x.Name == parameterName);
            Assert.AreEqual(now.Year, result.UpdatedDate.Year);
            Assert.AreEqual(now.Month, result.UpdatedDate.Month);
            Assert.AreEqual(now.Day, result.UpdatedDate.Day);
            Assert.AreEqual(now.Hour, result.UpdatedDate.Hour);
            Assert.AreEqual(now.Minute, result.UpdatedDate.Minute);
        }

        [TestMethod]
        public void ParameterService_SaveParameter_ParameterNameDoesNotExist_UnitOfWorkIsCalled()
        {
            _parameterService.SaveParameter("XXX666", 6);
            _mockUnitOfWork.Verify(v => v.Save(), Times.Once);
        }

        [TestMethod]
        public void ParameterService_Query_CallsRepositoryQuery()
        {
            #region Arrange

            #endregion

            #region Act

            var query = _parameterService.Query(x => x.Id == 1).ToList();

            #endregion

            #region Assert


            Assert.IsNotNull(query);
            _mockParameterRepository.Verify(x => x.Query(It.IsAny<Expression<Func<Parameter, bool>>>()), Times.Once());

            #endregion
        }

        [TestMethod]
        public void ParameterService_GetById_CallsRepositoryGetById()
        {
            #region Arrange

            #endregion

            #region Act

            var parameter = _parameterService.GetById(10);

            #endregion

            #region Assert

            _mockParameterRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(10, parameter.Id);

            #endregion
        }
    }
}
