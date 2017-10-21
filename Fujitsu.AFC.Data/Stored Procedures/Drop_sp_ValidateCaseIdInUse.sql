/****** Object:  StoredProcedure [dbo].[sp_ValidateCaseIdInUse]    Script Date: 03/23/2016 16:53:40 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_ValidateCaseIdInUse]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_ValidateCaseIdInUse]
GO

