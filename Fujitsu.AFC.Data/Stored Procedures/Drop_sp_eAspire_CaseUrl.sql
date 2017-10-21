/****** Object:  StoredProcedure [dbo].[sp_eAspire_CaseUrl]    Script Date: 05/05/2016 17:17:17 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_eAspire_CaseUrl]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_eAspire_CaseUrl]
GO

