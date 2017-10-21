/****** Object:  StoredProcedure [dbo].[sp_eAspire_RestrictUser]    Script Date: 05/05/2016 15:44:31 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_eAspire_RestrictUser]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_eAspire_RestrictUser]
GO

