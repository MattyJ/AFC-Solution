/****** Object:  StoredProcedure [dbo].[sp_eAspire_ArchiveCase]    Script Date: 05/03/2016 08:13:11 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_eAspire_ArchiveCase]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_eAspire_ArchiveCase]
GO

