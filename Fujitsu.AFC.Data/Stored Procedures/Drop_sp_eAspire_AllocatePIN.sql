/****** Object:  StoredProcedure [dbo].[sp_eAspire_AllocatePIN]    Script Date: 03/23/2016 10:19:19 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_eAspire_AllocatePIN]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_eAspire_AllocatePIN]
GO


