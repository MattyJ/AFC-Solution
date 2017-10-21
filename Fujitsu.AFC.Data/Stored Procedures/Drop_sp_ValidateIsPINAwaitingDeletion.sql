/****** Object:  StoredProcedure [dbo].[sp_ValidateIsPIN_AwaitingDeletion]    Script Date: 08/23/2016 15:18:26 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_ValidateIsPIN_AwaitingDeletion]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_ValidateIsPIN_AwaitingDeletion]
GO

