/****** Object:  StoredProcedure [dbo].[sp_eAspire_UpdateCaseTitleByProject]    Script Date: 04/12/2016 07:54:59 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_eAspire_UpdateCaseTitleByProject]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_eAspire_UpdateCaseTitleByProject]
GO

