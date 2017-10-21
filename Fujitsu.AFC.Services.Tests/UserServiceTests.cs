using System;
using System.Collections.Generic;
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
    public class UserServiceTests
    {
        private Mock<IRepository<Site>> _mockSiteRepository;
        private Mock<IRepository<Library>> _mockLibraryRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IUserIdentity> _mockUserIdentity;
        private List<Site> _sites;

        private Mock<IParameterService> _mockParameterService;
        private IUserService _userService;
        private Mock<IContextService> _mockContextService;

        [TestInitialize]
        public void Intialize()
        {
            _sites = new List<Site>();



            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserIdentity = new Mock<IUserIdentity>();
            _mockParameterService = new Mock<IParameterService>();
            _mockContextService = new Mock<IContextService>();

            _mockLibraryRepository = new Mock<IRepository<Library>>();

            _mockSiteRepository = MockRepositoryHelper.Create(_sites, (entity, id) => entity.Id == (int)id);
            _mockSiteRepository = new Mock<IRepository<Site>>();
            _userService = new UserService(_mockParameterService.Object, _mockContextService.Object, _mockSiteRepository.Object, _mockLibraryRepository.Object, _mockUserIdentity.Object, _mockUnitOfWork.Object);
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UserService_Constructor_NoParameterService()
        {
            #region Arrange

            #endregion

            #region Act

            new UserService(
                null,
                _mockContextService.Object,
                _mockSiteRepository.Object,
                _mockLibraryRepository.Object,
                _mockUserIdentity.Object,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UserService_Constructor_NoContextService()
        {
            #region Arrange

            #endregion

            #region Act

            new UserService(
                _mockParameterService.Object,
                null,
                _mockSiteRepository.Object,
                _mockLibraryRepository.Object,
                _mockUserIdentity.Object,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UserService_Constructor_NoSiteRepository()
        {
            #region Arrange

            #endregion

            #region Act

            new UserService(
                _mockParameterService.Object,
                _mockContextService.Object,
                null,
                _mockLibraryRepository.Object,
                _mockUserIdentity.Object,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UserService_Constructor_NoLibaryRepository()
        {
            #region Arrange

            #endregion

            #region Act

            new UserService(
                _mockParameterService.Object,
                _mockContextService.Object,
                _mockSiteRepository.Object,
                null,
                _mockUserIdentity.Object,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UserService_Constructor_NoUserIdentity()
        {
            #region Arrange

            #endregion

            #region Act

            new UserService(
                _mockParameterService.Object,
                _mockContextService.Object,
                _mockSiteRepository.Object,
                _mockLibraryRepository.Object,
                null,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UserService_Constructor_NoUnitOfWork()
        {
            #region Arrange

            #endregion

            #region Act

            new UserService(
                _mockParameterService.Object,
                _mockContextService.Object,
                _mockSiteRepository.Object,
                _mockLibraryRepository.Object,
                _mockUserIdentity.Object,
                null);

            #endregion

            #region Assert

            #endregion
        }

        #endregion
    }
}
