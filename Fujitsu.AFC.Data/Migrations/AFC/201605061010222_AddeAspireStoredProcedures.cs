namespace Fujitsu.AFC.Data.Migrations.AFC
{
    using System.Data.Entity.Migrations;

    public partial class AddeAspireStoredProcedures : DbMigration
    {
        public override void Up()
        {
            Sql(DatabaseResources.Create_sp_ValidateAvailableSites);
            Sql(DatabaseResources.Create_sp_ValidateCaseIdRequested);
            Sql(DatabaseResources.Create_sp_ValidateCaseIdInUse);
            Sql(DatabaseResources.Create_sp_ValidateDictionary);
            Sql(DatabaseResources.Create_sp_ValidateIsPIN_MergeFromPIN);
            Sql(DatabaseResources.Create_sp_ValidateIsPIN_MergeToPIN);
            Sql(DatabaseResources.Create_sp_ValidatePINExists);
            Sql(DatabaseResources.Create_sp_ValidatePINRequested);
            Sql(DatabaseResources.Create_sp_eAspire_AllocateCase);
            Sql(DatabaseResources.Create_sp_eAspire_AllocatePIN);
            Sql(DatabaseResources.Create_sp_eAspire_DeletePIN);
            Sql(DatabaseResources.Create_sp_eAspire_GetDictionaryXml);
            Sql(DatabaseResources.Create_sp_eAspire_MergePIN);
            Sql(DatabaseResources.Create_sp_eAspire_PINUrl);
            Sql(DatabaseResources.Create_sp_eAspire_UpdatePINTitle);
            Sql(DatabaseResources.Create_sp_eAspire_UpdateCaseTitle);
            Sql(DatabaseResources.Create_sp_eAspire_UpdateCaseTitleByProject);
            Sql(DatabaseResources.Create_sp_eAspire_CloseCase);
            Sql(DatabaseResources.Create_sp_eAspire_ArchiveCase);
            Sql(DatabaseResources.Create_sp_eAspire_MoveCase);
            Sql(DatabaseResources.Create_sp_eAspire_ChangePrimaryProject);
            Sql(DatabaseResources.Create_sp_eAspire_RestrictUser);
            Sql(DatabaseResources.Create_sp_eAspire_RemoveRestrictedUser);
            Sql(DatabaseResources.Create_sp_eAspire_UpdatePINWithDictionaryValues);
            Sql(DatabaseResources.Create_sp_eAspire_UpdateCaseWithDictionaryValues);
            Sql(DatabaseResources.Create_sp_eAspire_CaseUrl);
            Sql(DatabaseResources.Create_sp_ValidateIsPINAwaitingDeletion);
        }

        public override void Down()
        {
            Sql(DatabaseResources.Drop_sp_ValidateAvailableSites);
            Sql(DatabaseResources.Drop_sp_ValidateCaseIdRequested);
            Sql(DatabaseResources.Drop_sp_ValidateCaseIdInUse);
            Sql(DatabaseResources.Drop_sp_ValidateDictionary);
            Sql(DatabaseResources.Drop_sp_ValidateIsPIN_MergeFromPIN);
            Sql(DatabaseResources.Drop_sp_ValidateIsPIN_MergeToPIN);
            Sql(DatabaseResources.Drop_sp_ValidatePINExists);
            Sql(DatabaseResources.Drop_sp_ValidatePINRequested);
            Sql(DatabaseResources.Drop_sp_eAspire_AllocateCase);
            Sql(DatabaseResources.Drop_sp_eAspire_AllocatePIN);
            Sql(DatabaseResources.Drop_sp_eAspire_DeletePIN);
            Sql(DatabaseResources.Drop_sp_eAspire_GetDictionaryXml);
            Sql(DatabaseResources.Drop_sp_eAspire_MergePIN);
            Sql(DatabaseResources.Drop_sp_eAspire_PINUrl);
            Sql(DatabaseResources.Drop_sp_eAspire_UpdatePINTitle);
            Sql(DatabaseResources.Drop_sp_eAspire_UpdateCaseTitle);
            Sql(DatabaseResources.Drop_sp_eAspire_UpdateCaseTitleByProject);
            Sql(DatabaseResources.Drop_sp_eAspire_CloseCase);
            Sql(DatabaseResources.Drop_sp_eAspire_ArchiveCase);
            Sql(DatabaseResources.Drop_sp_eAspire_MoveCase);
            Sql(DatabaseResources.Drop_sp_eAspire_ChangePrimaryProject);
            Sql(DatabaseResources.Drop_sp_eAspire_RestrictUser);
            Sql(DatabaseResources.Drop_sp_eAspire_RemoveRestrictedUser);
            Sql(DatabaseResources.Drop_sp_eAspire_UpdatePINWithDictionaryValues);
            Sql(DatabaseResources.Drop_sp_eAspire_UpdateCaseWithDictionaryValues);
            Sql(DatabaseResources.Drop_sp_eAspire_CaseUrl);
            Sql(DatabaseResources.Drop_sp_ValidateIsPINAwaitingDeletion);
        }
    }
}
