using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Extensions;
using Fujitsu.AFC.Services.Interfaces;
using Microsoft.SharePoint.Client;
using System;
using System.Security;

namespace Fujitsu.AFC.Services.Common
{
    public abstract class SharePointOnline : ISharePointOnline
    {
        private readonly IParameterService _parameterService;

        protected SharePointOnline(IParameterService parameterService)
        {
            if (parameterService == null)
            {
                throw new ArgumentNullException(nameof(parameterService));
            }

            _parameterService = parameterService;
        }

        public string Username => _parameterService.GetParameterByNameAndCache<string>(ParameterNames.TenantAccountUserName);
        public string Password => _parameterService.GetParameterByNameAndCache<string>(ParameterNames.TenantAccountPassword);
        public SecureString PasswordSecureString => Password.PasswordToSecureString();
        public string TenantAdminWebSiteDomainPath => _parameterService.GetParameterByNameAndCache<string>(ParameterNames.TenantAdminWebSiteDomainPath);
        public string TenantWebSiteDomainPath => _parameterService.GetParameterByNameAndCache<string>(ParameterNames.TenantWebSiteDomainPath);
        public string TenantWebSiteUrlPath => _parameterService.GetParameterByNameAndCache<string>(ParameterNames.TenantWebSiteUrlPath);
        public long SiteCollectionStorageMaximumLevel => _parameterService.GetParameterByNameAndCache<long>(ParameterNames.SiteCollectionStorageMaximumLevel);
        public long SiteCollectionStorageWarningLevel => _parameterService.GetParameterByNameAndCache<long>(ParameterNames.SiteCollectionStorageWarningLevel);
        public double SiteCollectionUserCodeMaximumLevel => _parameterService.GetParameterByNameAndCache<double>(ParameterNames.SiteCollectionUserCodeMaximumLevel);
        public string SiteTemplate => _parameterService.GetParameterByNameAndCache<string>(ParameterNames.SiteTemplate);
        public string EnvironmentSiteCollectionPrefix => _parameterService.GetParameterByNameAndCache<string>(ParameterNames.EnvironmentSiteCollectionPrefix);
        public string SupportSiteCollectionSuffix => _parameterService.GetParameterByNameAndCache<string>(ParameterNames.SupportSiteCollectionSuffix);
        public string SupportEscalationListName => _parameterService.GetParameterByNameAndCache<string>(ParameterNames.SupportEscalationListName);
        public SharePointOnlineCredentials Type { get; set; }
        protected SharePointOnlineCredentials Credentials => new SharePointOnlineCredentials(Username, PasswordSecureString);

        public string ServerRelativeUrl(string siteCollectionName)
        {
            // e.g. "/sites/DCF-L1"
            return $"/{TenantWebSiteUrlPath}/{siteCollectionName}";
        }

        public string ServerRelativeSiteUrl(string siteCollectionName, string pin)
        {
            // e.g. "/sites/DCF-L1/12345"
            return $"/{TenantWebSiteUrlPath}/{siteCollectionName}/{pin}";
        }

        public string SiteCollectionUrl(int siteCollectionId)
        {
            // e.g. "http://viewmycase.sharepoint.com/sites/DCF-L1"
            return $"{TenantWebSiteDomainPath}/{TenantWebSiteUrlPath}/{EnvironmentSiteCollectionPrefix}{siteCollectionId}";
        }

        public string SiteCollectionUrl(string siteCollectionName)
        {
            // e.g. "http://viewmycase.sharepoint.com/sites/DCF-L1"
            return $"{TenantWebSiteDomainPath}/{TenantWebSiteUrlPath}/{siteCollectionName}";
        }

        public string SiteUrl(string siteCollectionName, string siteName)
        {
            // e.g. "http://viewmycase.sharepoint.com/sites/DCF-L1/1234"
            return $"{SiteCollectionUrl(siteCollectionName)}/{siteName}";
        }

        public string SupportSiteCollectionUrl()
        {
            // e.g. "http://viewmycase.sharepoint.com/sites/DCF-L-Support"
            return $"{TenantWebSiteDomainPath}/{TenantWebSiteUrlPath}/{EnvironmentSiteCollectionPrefix}-{SupportSiteCollectionSuffix}";
        }

        public string CaseDocumentLibraryUrl(string siteCollectionName, string siteName, string caseName)
        {
            return $"{SiteUrl(siteCollectionName, siteName)}/{caseName}";
        }

        public string GetListNameFromServerRelativeUrl(string url)
        {
            return url.Substring(url.LastIndexOf('/') + 1);
        }
    }
}
