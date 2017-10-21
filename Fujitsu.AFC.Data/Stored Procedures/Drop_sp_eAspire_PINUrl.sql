/****** Object:  StoredProcedure [dbo].[sp_eAspire_PINUrl]    Script Date: 03/29/2016 13:51:40 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_eAspire_PINUrl]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_eAspire_PINUrl]
GO

