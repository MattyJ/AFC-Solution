/****** Object:  StoredProcedure [dbo].[sp_eAspire_UpdateCaseTitle]    Script Date: 04/22/2016 13:34:35 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_eAspire_UpdateCaseTitle]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_eAspire_UpdateCaseTitle]
GO

