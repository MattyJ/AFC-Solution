/****** Object:  StoredProcedure [dbo].[sp_eAspire_MoveCase]    Script Date: 05/05/2016 15:28:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_eAspire_MoveCase]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_eAspire_MoveCase]
GO

