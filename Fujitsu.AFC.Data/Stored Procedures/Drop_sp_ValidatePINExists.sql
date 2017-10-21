/****** Object:  StoredProcedure [dbo].[sp_ValidatePINExists]    Script Date: 03/23/2016 10:22:08 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_ValidatePINExists]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_ValidatePINExists]
GO

