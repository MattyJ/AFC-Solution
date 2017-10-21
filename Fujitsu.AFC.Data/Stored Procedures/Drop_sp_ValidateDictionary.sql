/****** Object:  StoredProcedure [dbo].[sp_ValidateDictionary]    Script Date: 03/23/2016 10:20:51 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_ValidateDictionary]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_ValidateDictionary]
GO


