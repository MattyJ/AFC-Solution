/****** Object:  StoredProcedure [dbo].[sp_ValidateAvailableSites]    Script Date: 03/23/2016 10:19:54 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_ValidateAvailableSites]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_ValidateAvailableSites]
GO


