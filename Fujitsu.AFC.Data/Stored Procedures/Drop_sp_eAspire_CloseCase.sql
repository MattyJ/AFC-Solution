/****** Object:  StoredProcedure [dbo].[sp_eAspire_CloseCase]    Script Date: 05/03/2016 11:48:15 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_eAspire_CloseCase]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_eAspire_CloseCase]
GO

