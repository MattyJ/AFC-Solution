/****** Object:  StoredProcedure [dbo].[sp_eAspire_UpdatePINWithDictionaryValues]    Script Date: 05/05/2016 17:32:30 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_eAspire_UpdatePINWithDictionaryValues]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_eAspire_UpdatePINWithDictionaryValues]
GO

