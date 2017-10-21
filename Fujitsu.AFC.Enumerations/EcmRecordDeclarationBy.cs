namespace Fujitsu.AFC.Enumerations
{
    public enum EcmRecordDeclarationBy
    {
        Unknown = 0,
        /// <summary>
        /// All list contributors and administrators
        /// </summary>
        AllListContributors = 1,
        /// <summary>
        /// Only list administrators
        /// </summary>
        OnlyAdmins = 2,
        /// <summary>
        /// Only policy actions
        /// </summary>
        OnlyPolicy = 3
    }
}