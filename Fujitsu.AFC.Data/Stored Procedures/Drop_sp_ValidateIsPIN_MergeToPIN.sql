/****** Object:  StoredProcedure [dbo].[sp_ValidateIsPIN_MergeToPIN]    Script Date: 03/30/2016 07:52:51 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_ValidateIsPIN_MergeToPIN]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_ValidateIsPIN_MergeToPIN]
GO

