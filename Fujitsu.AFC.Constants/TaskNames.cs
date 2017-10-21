namespace Fujitsu.AFC.Constants
{
    public struct TaskNames
    {
        // Provisioning Handler Tasks

        public const string CaseSiteProvisioning = "CaseSiteProvisioning";

        // Operations Handler Tasks

        public const string AllocatePin = "AllocatePin";
        public const string AllocateCase = "AllocateCase";
        public const string UpdateServiceUserTitle = "UpdateServiceUserTitle";
        public const string UpdateCaseTitle = "UpdateCaseTitle";
        public const string UpdateCaseTitleByProject = "UpdateCaseTitleByProject";
        public const string MoveCase = "MoveCase";
        public const string CloseCase = "CloseCase";
        public const string ArchiveCase = "ArchiveCase";
        public const string DeletePin = "DeletePin";
        public const string DeleteCase = "DeleteCase";
        public const string MergePin = "MergePin";
        public const string ChangePrimaryProject = "ChangePrimaryProject";
        public const string RestrictUser = "RestrictUser";
        public const string RemoveRestrictedUser = "RemoveRestrictedUser";
        public const string UpdatePinWithDictionaryValues = "UpdatePinWithDictionaryValues";
        public const string UpdateCaseWithDictionaryValues = "UpdateCaseWithDictionaryValues";

        public const string RetrieveDictionaryXml = "RetrieveDictionaryXml";
        public const string RetrieveCaseUrl = "RetrieveCaseUrl";
        public const string RetrievePinUrl = "RetrievePinUrl";

        // Support Handler Tasks

        public const string HistoryErrorLogMonitoring = "HistoryErrorLogMonitoring";

        //TODO:
        // 1. SharePoint Online Connectivity
        // 2. Database Connectivity
        // 3. Verify pool of unallocated sites against thresholds
        // 4. Recording the current storage consumption
        // 5. Verify spare storage capacity within the site collections is within tolerances
        // 6. Verify spare storage capacity within tenant is within tolerances
        // 7. Verify timer Locks within lock tolerances, remove lock if lock tolerance exceeded
        // 8. Purge/Archive history log as per configured parameter
        // 9. Verify Windows Services are "healthy"?
        // 10. Ascertain Site Collection Capacity Setting, this on a daily basis in size of each site collection and store this in the data model
        // 11. Report/Monitor/Delete any provisioned sites that no longer exist?
    }
}
