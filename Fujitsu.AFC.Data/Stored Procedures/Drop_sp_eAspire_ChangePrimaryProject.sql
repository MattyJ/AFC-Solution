/****** Object:  StoredProcedure [dbo].[sp_eAspire_ChangePrimaryProject]    Script Date: 05/05/2016 14:23:37 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_eAspire_ChangePrimaryProject]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_eAspire_ChangePrimaryProject]
GO

