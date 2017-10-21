/****** Object:  StoredProcedure [dbo].[sp_eAspire_RemoveRestrictedUser]    Script Date: 05/05/2016 15:44:04 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_eAspire_RemoveRestrictedUser]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_eAspire_RemoveRestrictedUser]
GO

