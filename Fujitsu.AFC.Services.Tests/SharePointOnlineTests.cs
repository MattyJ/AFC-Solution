using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Services.Common;
using Fujitsu.AFC.Services.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace Fujitsu.AFC.Services.Tests
{
    [TestClass]
    public class SharePointOnlineTests
    {
        private SharePointOnline _sharePointOnline;
        private Mock<IParameterService> _mockParameterService;


        [TestInitialize]
        public void TestInitialize()
        {
            _mockParameterService = new Mock<IParameterService>();

            _mockParameterService.Setup(s => s.GetParameterByNameAndCache<string>(ParameterNames.TenantWebSiteDomainPath))
                .Returns("https://mattjordan.sharepoint.com");
            _mockParameterService.Setup(s => s.GetParameterByNameAndCache<string>(ParameterNames.TenantWebSiteUrlPath))
                .Returns("sites");
            _mockParameterService.Setup(s => s.GetParameterByNameAndCache<string>(ParameterNames.EnvironmentSiteCollectionPrefix))
                .Returns("DCF-T");
            _mockParameterService.Setup(s => s.GetParameterByNameAndCache<string>(ParameterNames.SupportSiteCollectionSuffix))
                .Returns("Support");

            _sharePointOnline = new TestSharePointOnline(_mockParameterService.Object);
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SharePointOnline_Constructor_NoParameterService_ThrowsArgumentNullException()
        {
            new TestSharePointOnline(null);
        }

        #endregion

        [TestMethod]
        public void SharePointOnline_Username_CallsParameterService_GetParameterByNameAndCache()
        {
            var username = _sharePointOnline.Username;

            _mockParameterService.Verify(v => v.GetParameterByNameAndCache<string>(ParameterNames.TenantAccountUserName), Times.Once);
        }

        [TestMethod]
        public void SharePointOnline_Password_CallsParameterService_GetParameterByNameAndCache()
        {
            var password = _sharePointOnline.Password;

            _mockParameterService.Verify(v => v.GetParameterByNameAndCache<string>(ParameterNames.TenantAccountPassword), Times.Once);
        }

        [TestMethod]
        public void SharePointOnline_TenantAdminWebSiteDomainPath_CallsParameterService_GetParameterByNameAndCache()
        {
            var tenantAdminWebSiteDomainPath = _sharePointOnline.TenantAdminWebSiteDomainPath;

            _mockParameterService.Verify(v => v.GetParameterByNameAndCache<string>(ParameterNames.TenantAdminWebSiteDomainPath), Times.Once);
        }

        [TestMethod]
        public void SharePointOnline_TenantWebSiteDomainPath_CallsParameterService_GetParameterByNameAndCache()
        {
            var tenantWebSiteDomainPath = _sharePointOnline.TenantWebSiteDomainPath;

            _mockParameterService.Verify(v => v.GetParameterByNameAndCache<string>(ParameterNames.TenantWebSiteDomainPath), Times.Once);
        }

        [TestMethod]
        public void SharePointOnline_TenantWebSiteUrlPath_CallsParameterService_GetParameterByNameAndCache()
        {
            var tenantWebSiteUrlPath = _sharePointOnline.TenantWebSiteUrlPath;

            _mockParameterService.Verify(v => v.GetParameterByNameAndCache<string>(ParameterNames.TenantWebSiteUrlPath), Times.Once);
        }

        [TestMethod]
        public void SharePointOnline_SiteCollectionStorageMaximumLevel_CallsParameterService_GetParameterByNameAndCache()
        {
            var siteCollectionStorageMaximumLevel = _sharePointOnline.SiteCollectionStorageMaximumLevel;

            _mockParameterService.Verify(v => v.GetParameterByNameAndCache<long>(ParameterNames.SiteCollectionStorageMaximumLevel), Times.Once);
        }

        [TestMethod]
        public void SharePointOnline_SiteCollectionStorageWarningLevel_CallsParameterService_GetParameterByNameAndCache()
        {
            var siteCollectionStorageWarningLevel = _sharePointOnline.SiteCollectionStorageWarningLevel;

            _mockParameterService.Verify(v => v.GetParameterByNameAndCache<long>(ParameterNames.SiteCollectionStorageWarningLevel), Times.Once);
        }

        [TestMethod]
        public void SharePointOnline_SiteCollectionUserCodeMaximumLevel_CallsParameterService_GetParameterByNameAndCache()
        {
            var siteCollectionUserCodeMaximumLevel = _sharePointOnline.SiteCollectionUserCodeMaximumLevel;

            _mockParameterService.Verify(v => v.GetParameterByNameAndCache<double>(ParameterNames.SiteCollectionUserCodeMaximumLevel), Times.Once);
        }

        [TestMethod]
        public void SharePointOnline_SiteTemplate_CallsParameterService_GetParameterByNameAndCache()
        {
            var siteTemplate = _sharePointOnline.SiteTemplate;

            _mockParameterService.Verify(v => v.GetParameterByNameAndCache<string>(ParameterNames.SiteTemplate), Times.Once);
        }

        [TestMethod]
        public void SharePointOnline_EnvironmentSiteCollectionPrefix_CallsParameterService_GetParameterByNameAndCache()
        {
            var environmentSiteCollectionPrefix = _sharePointOnline.EnvironmentSiteCollectionPrefix;

            _mockParameterService.Verify(v => v.GetParameterByNameAndCache<string>(ParameterNames.EnvironmentSiteCollectionPrefix), Times.Once);
        }

        [TestMethod]
        public void SharePointOnline_ServerRelativeUrl_ReturnsCorrectUrl()
        {
            const string expectedUrl = "/sites/DCF-T99";

            Assert.AreEqual(expectedUrl, _sharePointOnline.ServerRelativeUrl("DCF-T99"));
        }

        [TestMethod]
        public void SharePointOnline_ServerRelativeSiteUrl_ReturnsCorrectUrl()
        {
            const string expectedUrl = "/sites/DCF-T99/1234567";

            Assert.AreEqual(expectedUrl, _sharePointOnline.ServerRelativeSiteUrl("DCF-T99", "1234567"));
        }

        [TestMethod]
        public void SharePointOnline_SiteCollectionUrl_ReturnsCorrectUrlFromSiteCollectionId()
        {
            const string expectedUrl = "https://mattjordan.sharepoint.com/sites/DCF-T99";

            Assert.AreEqual(expectedUrl, _sharePointOnline.SiteCollectionUrl(99));
        }

        [TestMethod]
        public void SharePointOnline_SiteCollectionUrl_ReturnsCorrectUrlFromSiteCollectionName()
        {
            const string expectedUrl = "https://mattjordan.sharepoint.com/sites/DCF-T99";

            Assert.AreEqual(expectedUrl, _sharePointOnline.SiteCollectionUrl("DCF-T99"));
        }

        [TestMethod]
        public void SharePointOnline_SiteUrl_ReturnsCorrectUrl()
        {
            const string expectedUrl = "https://mattjordan.sharepoint.com/sites/DCF-T99/12345";

            Assert.AreEqual(expectedUrl, _sharePointOnline.SiteUrl("DCF-T99", "12345"));
        }

        [TestMethod]
        public void SharePointOnline_SupportUrl_ReturnsCorrectUrl()
        {
            const string expectedUrl = "https://mattjordan.sharepoint.com/sites/DCF-T-Support";

            Assert.AreEqual(expectedUrl, _sharePointOnline.SupportSiteCollectionUrl());
        }

        [TestMethod]
        public void SharePointOnline_CaseDocumentLibraryUrl_ReturnsCorrectUrl()
        {
            const string expectedUrl = "https://mattjordan.sharepoint.com/sites/DCF-T99/12345/11111";

            Assert.AreEqual(expectedUrl, _sharePointOnline.CaseDocumentLibraryUrl("DCF-T99", "12345", "11111"));
        }
    }

    internal class TestSharePointOnline : SharePointOnline
    {
        public TestSharePointOnline(IParameterService parameterService) : base(parameterService)
        {
        }

    }
}
