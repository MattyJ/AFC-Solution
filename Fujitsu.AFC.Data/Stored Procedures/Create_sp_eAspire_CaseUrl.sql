/* ============================================================================================== */
/* Name:        sp_eAspire_CaseUrl                                                                */
/* Author:		Ray Banister                                                                      */
/* Create date: 05/05/2016                                                                        */
/*                                                                                                */
/* Description:	Retrive the URL for a specified Case                                              */
/*                                                                                                */
/*		                                                                                          */
/* Mandatory Input Parameters: @CaseId                                                            */
/* Optional Input Parameters:  NONE                                                               */
/* Returns: @Url [OUT], @StatusId [OUT]                                                           */
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
/* 05/05/2016     0.0      Ray Banister	     First implementation                                 */  
/* 16/05/2016     0.1      Ray Banister	     The SP was incorrectly raising ERR_NO_CASE_DCF.      */
/*                                           The spec does not require this, you just set StausId */
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
	DECLARE @CaseIdInUse BIT = 'False'
	DECLARE @CaseIdRequested BIT = 'False'
	
	-- Check all Parameters are there
	IF (@CaseId is null) OR (ISNUMERIC(@CaseId) = 0)
	BEGIN
		RAISERROR(60000,16,1)		-- ERR_INVALID_PARAMETERS
		RETURN
	END
	
	-- Check if Case already allocated
	EXEC dbo.sp_ValidateCaseIdInUse @CaseId, @CaseIdInUse OUTPUT
	IF @CaseIdInUse = 'True'
	BEGIN
		-- Return "Initialised" and the Url
		SET @Url = (SELECT Url FROM dbo.Library WHERE CaseId = @CaseId)
		SET @StatusId = 2
		RETURN
	END
		
	-- Check if Case awaiting allocation
	EXEC dbo.sp_ValidateCaseIdRequested @CaseId, @CaseIdRequested OUTPUT
	IF @CaseIdRequested = 'True'
	BEGIN
		-- Return "Initialising" and a NULL Url
		SET @Url = NULL
		SET @StatusId = 1
		RETURN
	END
	
	-- PIN not found and not awaiting allocation. Set "Undeclared" 
	BEGIN
		SET @Url = NULL
		SET @StatusId = 0
		RETURN
	END	
	
			
END

GO

