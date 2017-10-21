/****** Object:  StoredProcedure [dbo].[sp_ValidatePINRequested]    Script Date: 03/30/2016 09:26:38 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_ValidatePINRequested]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_ValidatePINRequested]
GO

