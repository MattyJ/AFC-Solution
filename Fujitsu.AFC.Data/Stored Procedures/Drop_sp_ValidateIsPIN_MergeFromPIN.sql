/****** Object:  StoredProcedure [dbo].[sp_ValidateIsPIN_MergeFromPIN]    Script Date: 03/23/2016 16:40:24 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_ValidateIsPIN_MergeFromPIN]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_ValidateIsPIN_MergeFromPIN]
GO

