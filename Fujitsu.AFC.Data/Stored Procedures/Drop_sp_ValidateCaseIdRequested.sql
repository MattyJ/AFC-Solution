/****** Object:  StoredProcedure [dbo].[sp_ValidateCaseIdRequested]    Script Date: 05/06/2016 13:39:00 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_ValidateCaseIdRequested]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_ValidateCaseIdRequested]
GO

