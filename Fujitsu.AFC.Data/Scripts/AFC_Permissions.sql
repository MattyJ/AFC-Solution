-- Grant permissions to the various stored procedures and views.
-- Note that access to these is from eAspire so xxx\xxxxx is the eAspire Service account

USE [master]
GO
CREATE LOGIN [xxx\xxxxx] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO


USE [SLM]			-- Change [SLM] to [database name] 
GO
CREATE USER [xxx\xxxxx] FOR LOGIN [xxx\xxxxx] WITH DEFAULT_SCHEMA=[dbo]
EXEC sp_addrolemember N'db_datareader', N'xxx\xxxxx'
EXEC sp_addrolemember N'db_datawriter', N'xxx\xxxxx'
GO

-- Main Stored procedures
GRANT EXECUTE ON [sp_eAspire_AllocateCase] TO "xxx\xxxxx"
GRANT EXECUTE ON [sp_eAspire_AllocatePIN] TO "xxx\xxxxx"
GRANT EXECUTE ON [sp_eAspire_ArchiveCase] TO "xxx\xxxxx"
GRANT EXECUTE ON [sp_eAspire_CaseUrl] TO "xxx\xxxxx"
GRANT EXECUTE ON [sp_eAspire_ChangePrimaryProject] TO "xxx\xxxxx"

GRANT EXECUTE ON [sp_eAspire_CloseCase] TO "xxx\xxxxx"
GRANT EXECUTE ON [sp_eAspire_DeletePIN] TO "xxx\xxxxx"
GRANT EXECUTE ON [sp_eAspire_GetDictionaryXml] TO "xxx\xxxxx"
GRANT EXECUTE ON [sp_eAspire_MergePIN] TO "xxx\xxxxx"
GRANT EXECUTE ON [sp_eAspire_MoveCase] TO "xxx\xxxxx"

GRANT EXECUTE ON [sp_eAspire_PINUrl] TO "xxx\xxxxx"
GRANT EXECUTE ON [sp_eAspire_RemoveRestrictedUser] TO "xxx\xxxxx"
GRANT EXECUTE ON [sp_eAspire_RestrictUser] TO "xxx\xxxxx"
GRANT EXECUTE ON [sp_eAspire_UpdateCaseTitle] TO "xxx\xxxxx"
GRANT EXECUTE ON [sp_eAspire_UpdateCaseTitleByProject] TO "xxx\xxxxx"

GRANT EXECUTE ON [sp_eAspire_UpdateCaseWithDictionaryValues] TO "xxx\xxxxx"
GRANT EXECUTE ON [sp_eAspire_UpdatePINTitle] TO "xxx\xxxxx"
GRANT EXECUTE ON [sp_eAspire_UpdatePINWithDictionaryValues] TO "xxx\xxxxx"

-- Validation Stored Procedures
GRANT EXECUTE ON [sp_ValidateAvailableSites] TO "xxx\xxxxx"
GRANT EXECUTE ON [sp_ValidateCaseIdInUse] TO "xxx\xxxxx"
GRANT EXECUTE ON [sp_ValidateCaseIdRequested] TO "xxx\xxxxx"
GRANT EXECUTE ON [sp_ValidateDictionary] TO "xxx\xxxxx"
GRANT EXECUTE ON [sp_ValidateIsPIN_MergeFromPIN] TO "xxx\xxxxx"
GRANT EXECUTE ON [sp_ValidateIsPIN_AwaitingDeletion] TO "xxx\xxxxx"


GRANT EXECUTE ON [sp_ValidateIsPIN_MergeToPIN] TO "xxx\xxxxx"
GRANT EXECUTE ON [sp_ValidatePINExists] TO "xxx\xxxxx"
GRANT EXECUTE ON [sp_ValidatePINRequested] TO "xxx\xxxxx"
--GRANT EXECUTE ON [sp_ValidatePIN_ProjectId_Exists] TO "xxx\xxxxx"


-- Views (read only)
GRANT SELECT ON [vw_eAspireCase] TO "xxx\xxxxx"
GRANT SELECT ON [vw_eAspirePIN] TO "xxx\xxxxx"

GO
