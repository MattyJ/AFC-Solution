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
    public class SiteServiceTests
    {
        private Mock<IRepository<Site>> _mockSiteRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;

        private List<Site> _sites;

        private IService<Site> _siteService;

        private const string UserName = "matthew.jordan@uk.fujitsu.com";
        private readonly DateTime _dateTime = DateTime.Now;

        [TestInitialize]
        public void Intialize()
        {
            _sites = new List<Site>
            {
                new Site
                {
                    Id = 1,
                    Url = "https://fujitsuuki.sharepoint.com/sites/DCF-T1/12345",
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime
                },
                new Site
                {
                    Id = 2,
                    Url = "https://fujitsuuki.sharepoint.com/sites/DCF-T1/23456",
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime
                },
                new Site
                {
                    Id = 3,
                    Url = "https://fujitsuuki.sharepoint.com/sites/DCF-T1/34567",
                    InsertedBy = UserName,
                    InsertedDate = _dateTime,
                    UpdatedBy = UserName,
                    UpdatedDate = _dateTime
                }

            };

            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockSiteRepository = MockRepositoryHelper.Create(_sites, (entity, id) => entity.Id == (int)id);

            _siteService = new SiteService(_mockSiteRepository.Object, _mockUnitOfWork.Object);

            Bootstrapper.Initialise();
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SiteService_Constructor_NoSiteRepository()
        {
            #region Arrange

            #endregion

            #region Act

            new SiteService(
                null,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SiteService_Constructor_NoUnitOfWork()
        {
            #region Arrange

            #endregion

            #region Act

            new SiteService(
                _mockSiteRepository.Object,
                null);

            #endregion

            #region Assert

            #endregion
        }

        #endregion

        [TestMethod]
        public void SiteService_Create_CallsInsertAndSaveChanges()
        {
            #region Arrange

            var site = new Site
            {
                Id = 4,
                Url = "https://fujitsuuki.sharepoint.com/sites/DCF-T1/45678",
                InsertedBy = UserName,
                InsertedDate = _dateTime,
                UpdatedBy = UserName,
                UpdatedDate = _dateTime
            };


            #endregion

            #region Act

            _siteService.Create(site);

            #endregion

            #region Assert

            _mockSiteRepository.Verify(x => x.Insert(It.IsAny<Site>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void SiteService_Update_CallUpdateAndUnitOfWorkSave()
        {
            #region Arrange

            var site = new Site
            {
                Id = 1,
            };

            #endregion

            #region Act

            _siteService.Update(site);

            #endregion

            #region Assert

            _mockSiteRepository.Verify(x => x.Update(It.IsAny<Site>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void SiteService_Delete_ByEntity_CallsDeleteAndUnitOfWorkSave()
        {
            #region Arrange

            var site = new Site
            {
                Id = 1
            };

            #endregion

            #region Act

            _siteService.Delete(site);

            #endregion

            #region Assert

            _mockSiteRepository.Verify(x => x.Delete(It.IsAny<Site>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void SiteService_Delete_ById_CallsDeleteAndUnitOfWorkSave()
        {
            #region Arrange

            #endregion

            #region Act

            _siteService.Delete(1);

            #endregion

            #region Assert

            _mockSiteRepository.Verify(x => x.Delete(It.IsAny<int>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void SiteService_GetAll_CallsRepositoryAll()
        {
            #region Arrange

            #endregion

            #region Act

            var sites = _siteService.All();

            #endregion

            #region Assert

            _mockSiteRepository.Verify(x => x.All(), Times.Once);
            Assert.AreEqual(_sites.Count, sites.Count());

            #endregion
        }

        [TestMethod]
        public void SiteService_GetById_CallsRepositoryGetById()
        {
            #region Arrange

            #endregion

            #region Act

            var site = _siteService.GetById(1);

            #endregion

            #region Assert

            _mockSiteRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(1, site.Id);

            #endregion
        }

        [TestMethod]
        public void SiteService_Query_CallsRepositoryQuery()
        {
            #region Arrange

            #endregion

            #region Act

            var query = _siteService.Query(x => x.Id == 1).ToList();

            #endregion

            #region Assert


            Assert.IsNotNull(query);
            _mockSiteRepository.Verify(x => x.Query(It.IsAny<Expression<Func<Site, bool>>>()), Times.Once());

            #endregion
        }
    }
}
