/****** Object:  StoredProcedure [dbo].[sp_eAspire_UpdateCaseWithDictionaryValues]    Script Date: 05/05/2016 18:14:19 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_eAspire_UpdateCaseWithDictionaryValues]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_eAspire_UpdateCaseWithDictionaryValues]
GO

