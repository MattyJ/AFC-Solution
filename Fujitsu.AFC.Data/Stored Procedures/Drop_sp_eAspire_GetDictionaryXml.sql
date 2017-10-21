/****** Object:  StoredProcedure [dbo].[sp_eAspire_GetDictionaryXml]    Script Date: 04/11/2016 08:34:09 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_eAspire_GetDictionaryXml]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_eAspire_GetDictionaryXml]
GO

