USE [AFC.DCF.Database]
GO

/****** Object:  StoredProcedure [dbo].[sp_ValidatePIN_ProjectId_Exists]    Script Date: 05/12/2016 16:56:40 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_ValidatePIN_ProjectId_Exists]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_ValidatePIN_ProjectId_Exists]
GO

