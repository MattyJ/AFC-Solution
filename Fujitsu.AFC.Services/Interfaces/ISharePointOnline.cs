namespace Fujitsu.AFC.Services.Interfaces
{
    public interface ISharePointOnline
    {
        string ServerRelativeUrl(string siteCollectionName);

        string SiteCollectionUrl(int siteCollectionId);

        string SiteCollectionUrl(string siteCollectionName);

        string SiteUrl(string siteCollectionName, string siteName);

        string SupportSiteCollectionUrl();

        string CaseDocumentLibraryUrl(string siteCollectionName, string siteName, string caseId);
    }
}
