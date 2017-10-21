/****** Object:  StoredProcedure [dbo].[sp_eAspire_AllocateCase]    Script Date: 03/23/2016 16:57:33 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_eAspire_AllocateCase]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_eAspire_AllocateCase]
GO

