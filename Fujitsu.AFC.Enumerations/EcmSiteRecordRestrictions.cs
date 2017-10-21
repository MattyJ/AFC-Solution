namespace Fujitsu.AFC.Enumerations
{
    public enum EcmSiteRecordRestrictions
    {
        Unknown = 0,
        /// <summary>
        /// Records are no more restricted than non-records
        /// </summary>
        None = 1,
        /// <summary>
        /// Records can be edited but not deleted
        /// </summary>
        BlockDelete = 16,
        /// <summary>
        /// Records cannot be edited or deleted. Any change will require the record declaration to be revoked
        /// </summary>
        BlockEdit = 256
    }
}