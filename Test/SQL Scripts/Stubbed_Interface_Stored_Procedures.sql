USE [AFC.DCF.Database]
GO
/****** Object:  StoredProcedure [dbo].[sp_eAspire_UpdatePINWithDictionaryValues]    Script Date: 03/23/2016 08:48:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* ============================================================================================== */
/* Name:        sp_eAspire_UpdatePINWithDictionaryValues                                          */
/* Author:		Ray Banister                                                                      */
/* Create date: 14/03/2016                                                                        */
/*                                                                                                */
/* Description:	Update selected PIN with Dictionary values                                        */
/*                                                                                                */
/*		                                                                                          */
/* Mandatory Input Parameters: @PIN, @Dictionary                                                  */
/* Optional Input Parameters:  @Dictionary                                                        */
/* Returns:                                                                                       */
/* Errors Raised: ERR_NO_PIN_DCF, ERR_DICT_INVALID                                                */
/*                                                                                                */
/*                       TEST FUNCTION ONLY                                                       */
/*                                                                                                */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 17/03/2016     0.0      Ray Banister	     First implementation                                 */  
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */

CREATE PROCEDURE [dbo].[sp_eAspire_UpdatePINWithDictionaryValues] 
	-- Add the parameters for the stored procedure here
	@PIN INT, 
	@Dictionary NVARCHAR (4000) = NULL
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @RESULT NVARCHAR(300) = ''
	
	-- Dummy function STARTS
	IF @PIN <100
		SET @RESULT = 'Success'
	ELSE IF @PIN < 400
		RAISERROR(60007,16,1)
	ELSE IF @PIN < 700
		RAISERROR(60004,16,1)

	-- Dummy function ENDS...
			
END
	
SELECT @RESULT
GO
/****** Object:  StoredProcedure [dbo].[sp_eAspire_UpdatePINTitle]    Script Date: 03/23/2016 08:48:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* ============================================================================================== */
/* Name:        sp_eAspire_UpdatePINTitle                                                         */
/* Author:		Ray Banister                                                                      */
/* Create date: 14/03/2016                                                                        */
/*                                                                                                */
/* Description:	Updates the Site Title based on the PIN.                                          */
/*                                                                                                */
/*		                                                                                          */
/* Mandatory Input Parameters: @PIN, @SiteTitle                                                   */
/* Optional Input Parameters:  NONE                                                               */
/* Returns:                                                                                       */
/* Errors Raised: ERR_NO_PIN_DCF                                                                  */
/*                                                                                                */
/*                       TEST FUNCTION ONLY                                                       */
/*                                                                                                */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 15/03/2016     0.0      Ray Banister	     First implementation                                 */  
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */

CREATE PROCEDURE [dbo].[sp_eAspire_UpdatePINTitle] 
	@PIN INT, 
	@SiteTitle NVARCHAR(100)
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @RESULT NVARCHAR(300) = ''
	
	-- Dummy function STARTS
	IF @PIN <100
		SET @RESULT = 'Site Title: ' + 
		' - PIN: ' + CAST(@PIN AS NVARCHAR(50))
	ELSE IF (@PIN >= 600 AND @PIN <=699)
		RAISERROR(60004,16,1)
	-- Dummy function ENDS...
			
END
	
SELECT @RESULT
GO
/****** Object:  StoredProcedure [dbo].[sp_eAspire_UpdateCaseWithDictionaryValues]    Script Date: 03/23/2016 08:48:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* ============================================================================================== */
/* Name:        sp_eAspire_UpdateCaseWithDictionaryValues                                         */
/* Author:		Ray Banister                                                                      */
/* Create date: 14/03/2016                                                                        */
/*                                                                                                */
/* Description:	Update a case with dictonary data                                                 */
/*                                                                                                */
/*		                                                                                          */
/* Mandatory Input Parameters: @CaseId                                                            */
/* Optional Input Parameters:                                                                     */
/* Returns:                                                                                       */
/* Errors Raised: ERR_NO_CASE_DCF, ERR_DICT_INVAL                                                 */
/*                                                                                                */
/*                       TEST FUNCTION ONLY                                                       */
/*                                                                                                */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 17/03/2016     0.0      Ray Banister	     First implementation                                 */  
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */

CREATE PROCEDURE [dbo].[sp_eAspire_UpdateCaseWithDictionaryValues] 
	-- Add the parameters for the stored procedure here
	@CaseId INT, 
	@Dictionary NVARCHAR (4000) = NULL
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @RESULT NVARCHAR(300) = ''
	
	-- Dummy function STARTS
	IF @CaseId <100
		SET @RESULT = 'Success'
	ELSE IF @CaseId < 400
		RAISERROR(60007,16,1)
	ELSE IF @CaseId < 800
		RAISERROR(60006,16,1)

	-- Dummy function ENDS...
			
END
	
SELECT @RESULT
GO
/****** Object:  StoredProcedure [dbo].[sp_eAspire_UpdateCaseTitleByProject]    Script Date: 03/23/2016 08:48:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* ============================================================================================== */
/* Name:        sp_eAspire_UpdateCaseTitleByProject                                               */
/* Author:		Ray Banister                                                                      */
/* Create date: 14/03/2016                                                                        */
/*                                                                                                */
/* Description:	Replace all the Project Names in the title of cases for the given ProjectId       */
/*                                                                                                */
/*		                                                                                          */
/* Mandatory Input Parameters: @PIN, @ProjectName @ProjectId                                      */
/* Optional Input Parameters:  NONE                                                               */
/* Returns:                                                                                       */
/* Errors Raised: ERR_NO_PIN_DCF                                                                  */
/*                                                                                                */
/*                       TEST FUNCTION ONLY                                                       */
/*                                                                                                */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 15/03/2016     0.0      Ray Banister	     First implementation                                 */  
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */

CREATE PROCEDURE [dbo].[sp_eAspire_UpdateCaseTitleByProject] 
	@PIN INT, 
	@ProjectName NVARCHAR(100) ,
	@ProjectId INT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @RESULT NVARCHAR(300) = ''
	
	-- Dummy function STARTS
	IF @PIN <100
		SET @RESULT = 'Project Name: ' + @ProjectName + 
		' - PIN: ' + CAST(@PIN AS NVARCHAR(50)) + 
		' - ProjectId ' + CAST(@ProjectId AS NVARCHAR(50))
	ELSE IF (@PIN >= 600 AND @PIN <=699)
		RAISERROR(60004,16,1)
	-- Dummy function ENDS...
			
END
	
SELECT @RESULT
GO
/****** Object:  StoredProcedure [dbo].[sp_eAspire_UpdateCaseTitle]    Script Date: 03/23/2016 08:48:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* ============================================================================================== */
/* Name:        sp_eAspire_UpdateCaseTitle                                                        */
/* Author:		Ray Banister                                                                      */
/* Create date: 14/03/2016                                                                        */
/*                                                                                                */
/* Description:	Updates the Case Title based on the CaseId.                                       */
/*                                                                                                */
/*		                                                                                          */
/* Mandatory Input Parameters: @PIN, @CaseTitle @CaseId                                           */
/* Optional Input Parameters:  NONE                                                               */
/* Returns:                                                                                       */
/* Errors Raised: ERR_NO_PIN_DCF, ERR_NO_CASE_DCF                                                 */
/*                                                                                                */
/*                       TEST FUNCTION ONLY                                                       */
/*                                                                                                */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 15/03/2016     0.0      Ray Banister	     First implementation                                 */  
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */

CREATE PROCEDURE [dbo].[sp_eAspire_UpdateCaseTitle] 
	@PIN INT, 
	@CaseTitle NVARCHAR(100) ,
	@CaseId INT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @RESULT NVARCHAR(300) = ''
	
	-- Dummy function STARTS
	IF @PIN <100
		SET @RESULT = 'Case Title: ' + @CaseTitle + 
		' - PIN: ' + CAST(@PIN AS NVARCHAR(50)) + 
		' - CaseId ' + CAST(@CaseId AS NVARCHAR(50))
	ELSE IF (@PIN >= 600 AND @PIN <=699)
		RAISERROR(60004,16,1)
	ELSE IF (@PIN >= 700 AND @PIN <=799)
		RAISERROR(60006,16,1)
	-- Dummy function ENDS...
			
END
	
SELECT @RESULT
GO
/****** Object:  StoredProcedure [dbo].[sp_eAspire_RestrictUser]    Script Date: 03/23/2016 08:48:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* ============================================================================================== */
/* Name:        sp_eAspire_RestrictUser                                                           */
/* Author:		Ray Banister                                                                      */
/* Create date: 14/03/2016                                                                        */
/*                                                                                                */
/* Description:	Deltet Digital case File.                                                         */
/*                                                                                                */
/*		                                                                                          */
/* Mandatory Input Parameters: @PIN                                                               */
/* Optional Input Parameters:  NONE                                                               */
/* Returns:                                                                                       */
/* Errors Raised: ERR_NO_PIN_DCF                                                                  */
/*                                                                                                */
/*                       TEST FUNCTION ONLY                                                       */
/*                                                                                                */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 15/03/2016     0.0      Ray Banister	     First implementation                                 */  
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */

create PROCEDURE [dbo].[sp_eAspire_RestrictUser] 
	@PIN INT 

	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @RESULT NVARCHAR(300) = ''
	
	-- Dummy function STARTS
	IF @PIN <100
		SET @RESULT = 'PIN: ' + CAST(@PIN AS NVARCHAR(50)) + 
		' - User Restricted' 
	ELSE IF (@PIN >= 600 AND @PIN <=699)
		RAISERROR(60004,16,1)
	
	-- Dummy function ENDS...
			
END
	
SELECT @RESULT
GO
/****** Object:  StoredProcedure [dbo].[sp_eAspire_RemoveRestrictedUser]    Script Date: 03/23/2016 08:48:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* ============================================================================================== */
/* Name:        sp_eAspire_RemoveRestrictedUser                                                   */
/* Author:		Ray Banister                                                                      */
/* Create date: 14/03/2016                                                                        */
/*                                                                                                */
/* Description:	Deltet Digital case File.                                                         */
/*                                                                                                */
/*		                                                                                          */
/* Mandatory Input Parameters: @PIN                                                               */
/* Optional Input Parameters:  NONE                                                               */
/* Returns:                                                                                       */
/* Errors Raised: ERR_NO_PIN_DCF                                                                  */
/*                                                                                                */
/*                       TEST FUNCTION ONLY                                                       */
/*                                                                                                */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 15/03/2016     0.0      Ray Banister	     First implementation                                 */  
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */

create PROCEDURE [dbo].[sp_eAspire_RemoveRestrictedUser] 
	@PIN INT 

	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @RESULT NVARCHAR(300) = ''
	
	-- Dummy function STARTS
	IF @PIN <100
		SET @RESULT = 'PIN: ' + CAST(@PIN AS NVARCHAR(50)) + 
		' - Restricted user Removed' 
	ELSE IF (@PIN >= 600 AND @PIN <=699)
		RAISERROR(60004,16,1)
	
	-- Dummy function ENDS...
			
END
	
SELECT @RESULT
GO
/****** Object:  StoredProcedure [dbo].[sp_eAspire_PINUrl]    Script Date: 03/23/2016 08:48:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* ============================================================================================== */
/* Name:        sp_eAspire_PINUrl                                                                 */
/* Author:		Ray Banister                                                                      */
/* Create date: 14/03/2016                                                                        */
/*                                                                                                */
/* Description:	Retrive the URL for a specified PIN                                               */
/*                                                                                                */
/*		                                                                                          */
/* Mandatory Input Parameters: @PIN, @Url [OUT], @StatusId [OUT]                                  */
/* Optional Input Parameters:  NONE                                                               */
/* Returns:                                                                                       */
/* Errors Raised: ERR_NO_PIN_DCF                                                                  */
/*                                                                                                */
/*                       TEST FUNCTION ONLY                                                       */
/*                                                                                                */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 15/03/2016     0.0      Ray Banister	     First implementation                                 */  
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */

CREATE PROCEDURE [dbo].[sp_eAspire_PINUrl] 
	@PIN INT, 
	@Url NVARCHAR(2000) OUT,
	@StatusId INT OUT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @RESULT NVARCHAR(300) = ''
	DECLARE @BASE NVARCHAR(50) = 'https://afc.onsharepoint.com/sites/DCF-1/'
	
	IF (@PIN <100)
	BEGIN
		SET @Url = @BASE + '1234' + '/' + CAST(@PIN AS NVARCHAR(50))
		SET @StatusId = 2
		SET @RESULT = 'Initialised'
	END	
	
	ELSE IF (@PIN >= 100 AND @PIN	<=199)
	BEGIN
		SET @Url = ''
		SET @StatusId = 0
		SET @RESULT = 'Undeclared'
	END		
		
	ELSE IF (@PIN >= 200 AND @PIN	<=299)
	BEGIN
		SET @Url =  ''
		SET @StatusId = 1
		SET @RESULT = 'Initialising'
	END	
	
	ELSE IF (@PIN >= 300 AND @PIN	<=399)
	BEGIN
		SET @Url = ''
		SET @StatusId = -1
		SET @RESULT = 'Invalid'
	END	
	
	ELSE IF (@PIN >= 400 AND @PIN	<=599)
	BEGIN
		SET @Url = ''
		SET @StatusId = -2
		SET @RESULT = 'Invalid'
	END		
		
	ELSE IF (@PIN >= 600 AND @PIN	<=699)
	BEGIN
		SET @Url = ''
		SET @StatusId = -2
		SET @RESULT = 'Invalid'
		RAISERROR(60004,16,1)
	END		
		
	-- Dummy function ENDS...
			
END
	
SELECT @RESULT
GO
/****** Object:  StoredProcedure [dbo].[sp_eAspire_MoveCase]    Script Date: 03/23/2016 08:48:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* ============================================================================================== */
/* Name:        sp_eAspire_MoveCase                                                               */
/* Author:		Ray Banister                                                                      */
/* Create date: 14/03/2016                                                                        */
/*                                                                                                */
/* Description:	Replace all the Project Names in the title of cases for the given ProjectId       */
/*                                                                                                */
/*		                                                                                          */
/* Mandatory Input Parameters: @PIN, @CaseId, @CurrentProjectId, @NewProjectId, @IsPrimary        */
/* Optional Input Parameters:  NONE                                                               */
/* Returns:                                                                                       */
/* Errors Raised: ERR_NO_PIN_DCF, ERR_NO_CASE_DCF                                                 */
/*                                                                                                */
/*                       TEST FUNCTION ONLY                                                       */
/*                                                                                                */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 15/03/2016     0.0      Ray Banister	     First implementation                                 */  
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */

CREATE PROCEDURE [dbo].[sp_eAspire_MoveCase] 
	@PIN INT, 
	@CaseId INT,
	@CurrentProjectId INT,
	@NewProjectId INT,
	@IsPrimary BIT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @RESULT NVARCHAR(300) = ''
	
	-- Dummy function STARTS
	IF @PIN <100
		SET @RESULT = 'CaseId: ' + CAST(@CaseId AS NVARCHAR(50)) + 
		' - PIN: ' + CAST(@PIN AS NVARCHAR(50)) + 
		' - CurrentProjectId ' + CAST(@CurrentProjectId AS NVARCHAR(50)) + 
		' - NewProjectId ' + CAST(@NewProjectId AS NVARCHAR(50))
	ELSE IF (@PIN >= 600 AND @PIN <=699)
		RAISERROR(60004,16,1)
	ELSE IF (@PIN >= 700 AND @PIN <=799)
		RAISERROR(60006,16,1)
	-- Dummy function ENDS...
			
END
	
SELECT @RESULT
GO
/****** Object:  StoredProcedure [dbo].[sp_eAspire_MergePIN]    Script Date: 03/23/2016 08:48:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* ============================================================================================== */
/* Name:        sp_eAspire_MergePIN                                                               */
/* Author:		Ray Banister                                                                      */
/* Create date: 14/03/2016                                                                        */
/*                                                                                                */
/* Description:	Merge DCF for two Service users                                                   */
/*                                                                                                */
/*		                                                                                          */
/* Mandatory Input Parameters: @ToPIN, @FromPIN                                                   */
/* Optional Input Parameters:  NONE                                                               */
/* Returns:                                                                                       */
/* Errors Raised: ERR_NO_PIN_DCF, ERR_NO_SPACE                                                    */
/*                                                                                                */
/*                       TEST FUNCTION ONLY                                                       */
/*                                                                                                */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 15/03/2016     0.0      Ray Banister	     First implementation                                 */  
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */

CREATE PROCEDURE [dbo].[sp_eAspire_MergePIN] 
	@ToPIN INT, 
	@FromPIN INT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @RESULT NVARCHAR(300) = ''
	
	-- Dummy function STARTS
	IF (@ToPIN <100 AND @FromPIN <700)
		SET @RESULT = 'ToPIN: ' + CAST(@ToPIN AS NVARCHAR(50)) + 
		' - FromPIN: ' + CAST(@FromPIN AS NVARCHAR(50)) + ' MERGED' 
	ELSE IF (@ToPIN >= 600 AND @ToPIN <=699)
		RAISERROR(60004,16,1)
	ELSE IF (@ToPIN >= 800 AND @ToPIN <=899)
		RAISERROR(60011,16,1)
	ELSE IF (@FromPIN >= 700 AND @FromPIN <=799)
		RAISERROR(60004,16,1)
	-- Dummy function ENDS...
			
END
	
SELECT @RESULT
GO
/****** Object:  StoredProcedure [dbo].[sp_eAspire_GetPINDictionaryXml]    Script Date: 03/23/2016 08:48:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* ============================================================================================== */
/* Name:        sp_eAspire_GetPINDictionaryXml                                                    */
/* Author:		Ray Banister                                                                      */
/* Create date: 17/03/2016                                                                        */
/*                                                                                                */
/* Description:	Retrieves the Dictionary XML for the associated PIN                               */
/*                                                                                                */
/*		                                                                                          */
/* Mandatory Input Parameters: @PIN, @Dictionary                                                  */
/* Optional Input Parameters:  NONE                                                               */
/* Returns:                                                                                       */
/* Errors Raised: ERR_NO_PIN_DCF                                                                  */
/*                                                                                                */
/*                       TEST FUNCTION ONLY                                                       */
/*                                                                                                */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 17/03/2016     0.0      Ray Banister	     First implementation                                 */  
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */

CREATE PROCEDURE [dbo].[sp_eAspire_GetPINDictionaryXml] 
	@Dictionary NVARCHAR(4000) OUT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @RESULT NVARCHAR(300) = ''
	
	BEGIN
		SET @Dictionary = '<items><item><key>Service Type</key><value/></item><item><key>Service User Pin</key><value/></item></items>'
		SET @RESULT = 'Success'
	END	
	

	
	-- Dummy function ENDS...
			
END
	
SELECT @RESULT
GO
/****** Object:  StoredProcedure [dbo].[sp_eAspire_DeletePIN]    Script Date: 03/23/2016 08:48:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* ============================================================================================== */
/* Name:        sp_eAspire_DeletePIN                                                              */
/* Author:		Ray Banister                                                                      */
/* Create date: 14/03/2016                                                                        */
/*                                                                                                */
/* Description:	Delete PIN.                                                                       */
/*                                                                                                */
/*		                                                                                          */
/* Mandatory Input Parameters: @PIN,                                                              */
/* Optional Input Parameters:  NONE                                                               */
/* Returns:                                                                                       */
/* Errors Raised: ERR_NO_PIN_DCF, ERR_NO_CASE_DCF                                                 */
/*                                                                                                */
/*                       TEST FUNCTION ONLY                                                       */
/*                                                                                                */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 15/03/2016     0.0      Ray Banister	     First implementation                                 */  
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */

CREATE PROCEDURE [dbo].[sp_eAspire_DeletePIN] 
	@PIN INT 

	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @RESULT NVARCHAR(300) = ''
	
	-- Dummy function STARTS
	IF @PIN <100
		SET @RESULT = 'PIN: ' + CAST(@PIN AS NVARCHAR(50)) + ' DELETED' 
	ELSE IF (@PIN >= 600 AND @PIN <=699)
		RAISERROR(60004,16,1)
	-- Dummy function ENDS...
			
END
	
SELECT @RESULT
GO
/****** Object:  StoredProcedure [dbo].[sp_eAspire_DeleteCase]    Script Date: 03/23/2016 08:48:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* ============================================================================================== */
/* Name:        sp_eAspire_DeleteCase                                                            */
/* Author:		Ray Banister                                                                      */
/* Create date: 14/03/2016                                                                        */
/*                                                                                                */
/* Description:	Deltet Digital case File.                                                        */
/*                                                                                                */
/*		                                                                                          */
/* Mandatory Input Parameters: @PIN, @CaseId,                                                     */
/* Optional Input Parameters:  NONE                                                               */
/* Returns:                                                                                       */
/* Errors Raised: ERR_NO_PIN_DCF, ERR_NO_CASE_DCF                                                 */
/*                                                                                                */
/*                       TEST FUNCTION ONLY                                                       */
/*                                                                                                */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 15/03/2016     0.0      Ray Banister	     First implementation                                 */  
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */

create PROCEDURE [dbo].[sp_eAspire_DeleteCase] 
	@PIN INT, 
	@CaseId INT

	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @RESULT NVARCHAR(300) = ''
	
	-- Dummy function STARTS
	IF @PIN <100
		SET @RESULT = 'CaseId: ' + CAST(@CaseId AS NVARCHAR(50)) + 
		' - PIN: ' + CAST(@PIN AS NVARCHAR(50)) 
	ELSE IF (@PIN >= 600 AND @PIN <=699)
		RAISERROR(60004,16,1)
	ELSE IF (@PIN >= 700 AND @PIN <=799)
		RAISERROR(60006,16,1)
	-- Dummy function ENDS...
			
END
	
SELECT @RESULT
GO
/****** Object:  StoredProcedure [dbo].[sp_eAspire_CloseCase]    Script Date: 03/23/2016 08:48:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* ============================================================================================== */
/* Name:        sp_eAspire_CloseCase                                                              */
/* Author:		Ray Banister                                                                      */
/* Create date: 14/03/2016                                                                        */
/*                                                                                                */
/* Description:	Close Digital case File.                                                          */
/*                                                                                                */
/*		                                                                                          */
/* Mandatory Input Parameters: @PIN, @CaseId, @ProjectId, @IsPrimary                              */
/* Optional Input Parameters:  NONE                                                               */
/* Returns:                                                                                       */
/* Errors Raised: ERR_NO_PIN_DCF, ERR_NO_CASE_DCF                                                 */
/*                                                                                                */
/*                       TEST FUNCTION ONLY                                                       */
/*                                                                                                */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 15/03/2016     0.0      Ray Banister	     First implementation                                 */  
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */

Create PROCEDURE [dbo].[sp_eAspire_CloseCase] 
	@PIN INT, 
	@CaseId INT,
	@ProjectId INT,
	@IsPrimary BIT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @RESULT NVARCHAR(300) = ''
	
	-- Dummy function STARTS
	IF @PIN <100
		SET @RESULT = 'CaseId: ' + CAST(@CaseId AS NVARCHAR(50)) + 
		' - PIN: ' + CAST(@PIN AS NVARCHAR(50)) + 
		' - ProjectId ' + CAST(@ProjectId AS NVARCHAR(50)) 
	ELSE IF (@PIN >= 600 AND @PIN <=699)
		RAISERROR(60004,16,1)
	ELSE IF (@PIN >= 700 AND @PIN <=799)
		RAISERROR(60006,16,1)
	-- Dummy function ENDS...
			
END
	
SELECT @RESULT
GO
/****** Object:  StoredProcedure [dbo].[sp_eAspire_ChangePrimaryProject]    Script Date: 03/23/2016 08:48:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* ============================================================================================== */
/* Name:        sp_eAspire_ChangePrimaryProject                                                   */
/* Author:		Ray Banister                                                                      */
/* Create date: 14/03/2016                                                                        */
/*                                                                                                */
/* Description:	Merge DCF for two Service users                                                   */
/*                                                                                                */
/*		                                                                                          */
/* Mandatory Input Parameters: @PIN, @CurrentProjectId, @NewProjectId                             */
/* Optional Input Parameters:  NONE                                                               */
/* Returns:                                                                                       */
/* Errors Raised: ERR_NO_PIN_DCF,                                                                 */
/*                                                                                                */
/*                       TEST FUNCTION ONLY                                                       */
/*                                                                                                */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 15/03/2016     0.0      Ray Banister	     First implementation                                 */  
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */

create PROCEDURE [dbo].[sp_eAspire_ChangePrimaryProject] 
	@PIN INT, 
	@CurrentProjectId INT,
	@NewProjectId INT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @RESULT NVARCHAR(300) = ''
	
	-- Dummy function STARTS
	IF (@PIN <100 )
		SET @RESULT = 'PIN: ' + CAST(@PIN AS NVARCHAR(50)) + 
		' - Current Project Id: ' + CAST(@CurrentProjectId AS NVARCHAR(50))  + 
		' - New Project Id: ' + CAST(@NewProjectId AS NVARCHAR(50))+ ' Primary Project Changed' 
	ELSE IF (@PIN >= 600 AND @PIN <=699)
		RAISERROR(60004,16,1)
	-- Dummy function ENDS...
			
END
	
SELECT @RESULT
GO
/****** Object:  StoredProcedure [dbo].[sp_eAspire_CaseUrl]    Script Date: 03/23/2016 08:48:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* ============================================================================================== */
/* Name:        sp_eAspire_CaseUrl                                                                */
/* Author:		Ray Banister                                                                      */
/* Create date: 14/03/2016                                                                        */
/*                                                                                                */
/* Description:	Retrive the URL for a specified Case Id                                           */
/*                                                                                                */
/*		                                                                                          */
/* Mandatory Input Parameters: @CaseId, @Url [OUT], @StatusId [OUT]                               */
/* Optional Input Parameters:  NONE                                                               */
/* Returns:                                                                                       */
/* Errors Raised: ERR_NO_CASE_DCF                                                                 */
/*                                                                                                */
/*                       TEST FUNCTION ONLY                                                       */
/*                                                                                                */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 15/03/2016     0.0      Ray Banister	     First implementation                                 */  
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */

CREATE PROCEDURE [dbo].[sp_eAspire_CaseUrl] 
	@CaseId INT, 
	@Url NVARCHAR(2000) OUT,
	@StatusId INT OUT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @RESULT NVARCHAR(300) = ''
	DECLARE @BASE NVARCHAR(50) = 'https://afc.onsharepoint.com/sites/DCF-1/'
	
	IF (@CaseId <100)
	BEGIN
		SET @Url = @BASE + '1234' + '/' + CAST(@CaseId AS NVARCHAR(50))
		SET @StatusId = 2
		SET @RESULT = 'Initialised'
	END	
	
	ELSE IF (@CaseId >= 100 AND @CaseId	<=199)
	BEGIN
		SET @Url = ''
		SET @StatusId = 0
		SET @RESULT = 'Undeclared'
	END		
		
	ELSE IF (@CaseId >= 200 AND @CaseId	<=299)
	BEGIN
		SET @Url =  ''
		SET @StatusId = 1
		SET @RESULT = 'Initialising'
	END	
	
	ELSE IF (@CaseId >= 300 AND @CaseId	<=399)
	BEGIN
		SET @Url = ''
		SET @StatusId = -1
		SET @RESULT = 'Invalid'
	END	
	
	ELSE IF (@CaseId >= 400 AND @CaseId	<=599)
	BEGIN
		SET @Url = ''
		SET @StatusId = -2
		SET @RESULT = 'Invalid'
	END		
		
	ELSE IF (@CaseId >= 700 AND @CaseId	<=799)
	BEGIN
		SET @Url = ''
		SET @StatusId = -2
		SET @RESULT = 'Invalid'
		RAISERROR(60006,16,1)
	END		
		
	-- Dummy function ENDS...
			
END
	
SELECT @RESULT
GO
/****** Object:  StoredProcedure [dbo].[sp_eAspire_ArchiveCase]    Script Date: 03/23/2016 08:48:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* ============================================================================================== */
/* Name:        sp_eAspire_ArchiveCase                                                            */
/* Author:		Ray Banister                                                                      */
/* Create date: 14/03/2016                                                                        */
/*                                                                                                */
/* Description:	Archive Digital case File.                                                        */
/*                                                                                                */
/*		                                                                                          */
/* Mandatory Input Parameters: @PIN, @CaseId,                                                     */
/* Optional Input Parameters:  NONE                                                               */
/* Returns:                                                                                       */
/* Errors Raised: ERR_NO_PIN_DCF, ERR_NO_CASE_DCF                                                 */
/*                                                                                                */
/*                       TEST FUNCTION ONLY                                                       */
/*                                                                                                */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 15/03/2016     0.0      Ray Banister	     First implementation                                 */  
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */

create PROCEDURE [dbo].[sp_eAspire_ArchiveCase] 
	@PIN INT, 
	@CaseId INT

	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @RESULT NVARCHAR(300) = ''
	
	-- Dummy function STARTS
	IF @PIN <100
		SET @RESULT = 'CaseId: ' + CAST(@CaseId AS NVARCHAR(50)) + 
		' - PIN: ' + CAST(@PIN AS NVARCHAR(50)) 
	ELSE IF (@PIN >= 600 AND @PIN <=699)
		RAISERROR(60004,16,1)
	ELSE IF (@PIN >= 700 AND @PIN <=799)
		RAISERROR(60006,16,1)
	-- Dummy function ENDS...
			
END
	
SELECT @RESULT
GO
/****** Object:  StoredProcedure [dbo].[sp_eAspire_AllocatePIN]    Script Date: 03/23/2016 08:48:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* ============================================================================================== */
/* Name:        sp_eAspire_AllocatePIN                                                            */
/* Author:		Ray Banister                                                                      */
/* Create date: 14/03/2016                                                                        */
/*                                                                                                */
/* Description:	Allocate a new PIN if it doesn't already exist.                                   */
/*                                                                                                */
/*		                                                                                          */
/* Mandatory Input Parameters: @PIN, @SiteTitle                                                   */
/* Optional Input Parameters:  @Dictionary                                                        */
/* Returns:                    Site URL                                                           */
/* Errors Raised: ERR_NOSITES, ERR_PIN_IN_USE, ERR_DICT_INVAL, ERR_PIN_ALREADY_REQUESTED          */
/*                                                                                                */
/*                       TEST FUNCTION ONLY                                                       */
/*                                                                                                */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 14/03/2016     0.0      Ray Banister	     First implementation                                 */  
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */

CREATE PROCEDURE [dbo].[sp_eAspire_AllocatePIN] 
	-- Add the parameters for the stored procedure here
	@PIN INT, 
	@SiteTitle NVARCHAR(100) ,
	@Dictionary NVARCHAR (4000) = NULL
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @RESULT NVARCHAR(300) = ''
	
	-- Dummy function STARTS
	DECLARE @BASE NVARCHAR(50) = 'http://www.eAspireTest/'
	IF @PIN <100
		SET @RESULT = @BASE + @SiteTitle + '/' + CAST(@PIN AS NVARCHAR(50))
	ELSE IF @PIN < 200
		RAISERROR(60001,16,1)
	ELSE IF @PIN < 300
		RAISERROR(60002,16,1)	
	ELSE IF @PIN < 500
		RAISERROR(60007,16,1)
		ELSE IF @PIN < 400
	RAISERROR(60005,16,1)
	-- Dummy function ENDS...
			
END
	
SELECT @RESULT
GO
/****** Object:  StoredProcedure [dbo].[sp_eAspire_AllocateCase]    Script Date: 03/23/2016 08:48:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* ============================================================================================== */
/* Name:        sp_eAspire_AllocateCase                                                           */
/* Author:		Ray Banister                                                                      */
/* Create date: 14/03/2016                                                                        */
/*                                                                                                */
/* Description:	Allocate a Case.                                                                  */
/*                                                                                                */
/*		                                                                                          */
/* Mandatory Input Parameters: @PIN, @ProjectId,@CaseId,@CaseTitle,@IsPrimary                     */
/* Optional Input Parameters:  @Dictionary                                                        */
/* Returns:                                                                                       */
/* Errors Raised: ERR_CASEINUSE, ERR_NO_PIN_DCF, ERR_DICT_INVAL                                   */
/*                                                                                                */
/*                       TEST FUNCTION ONLY                                                       */
/*                                                                                                */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 14/03/2016     0.0      Ray Banister	     First implementation                                 */  
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */

CREATE PROCEDURE [dbo].[sp_eAspire_AllocateCase] 
	@PIN INT, 
	@ProjectId INT,
	@CaseId INT,
	@CaseTitle NVARCHAR(100),
	@Dictionary NVARCHAR(4000) = NULL,
	@IsPrimary BIT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @RESULT NVARCHAR(300) = ''
	
	-- Dummy function STARTS
	IF @PIN <100
		SET @RESULT = 'Case Allocated: ' + CAST(@CaseId AS NVARCHAR(50)) +
		' - ProjectId: ' + CAST(@ProjectId AS NVARCHAR(50)) + 
		' - PIN: ' + CAST(@PIN AS NVARCHAR(50))
	ELSE IF (@PIN >= 300 AND @PIN <=399)
		RAISERROR(60007,16,1)
	ELSE IF (@PIN >= 500 AND @PIN <=599)
		RAISERROR(60009,16,1)	
	ELSE IF (@PIN >= 600 AND @PIN <=699)
		RAISERROR(60004,16,1)
	-- Dummy function ENDS...
			
END
	
SELECT @RESULT
GO
/****** Object:  StoredProcedure [dbo].[usr_sp_ValidatePINRequested]    Script Date: 03/23/2016 08:48:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* ============================================================================================== */
/* Name:        usr_sp_ValidatePINRequested                                                       */
/* Author:		Ray Banister                                                                      */
/* Create date: 23/03/2016                                                                        */
/*                                                                                                */
/* Description:	Checks if the selected PIN has already been requested, via a previous AllocatePIN */
/*              request but has not yet been processed.                                           */
/*		                                                                                          */
/* Mandatory Input Parameters: @PIN                                                               */
/* Optional Input Parameters:  None                                                               */
/* Returns: @PINRequested as an OUT parameter                                                     */
/* Errors Raised: None                                                                            */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 22/03/2016     0.0      Ray Banister	     First implementation                                 */  
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */

CREATE PROCEDURE [dbo].[usr_sp_ValidatePINRequested] 
	@PIN INT,
	@PinRequested BIT OUT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	IF EXISTS(SELECT  1 FROM dbo.Task
		WHERE (Handler = 'OperationsHandler' AND Name = 'AllocatePIN' AND Pin = @PIN AND InsertedDate < GETDATE()))  
		SET @PinRequested = 1
	ELSE SET @PinRequested = 0
END
GO
