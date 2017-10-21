/* ============================================================================================== */
/* Name:        sp_eAspire_PINUrl                                                                 */
/* Author:		Ray Banister                                                                      */
/* Create date: 29/03/2016                                                                        */
/*                                                                                                */
/* Description:	Retrive the URL for a specified PIN                                               */
/*                                                                                                */
/*		                                                                                          */
/* Mandatory Input Parameters: @PIN                                                               */
/* Optional Input Parameters:  NONE                                                               */
/* Returns: @Url [OUT], @StatusId [OUT]                                                           */
/* Errors Raised:  None                                                                           */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 29/03/2016     0.0      Ray Banister	     First implementation                                 */  
/* 30/03/2016	  0.1      Ray Banister      Now call sp_ValidatePINRequested                     */
/* 12/04/2016     0.2      Matt Jordan       Removed Explicit USE database                        */
/* 16/05/2016     0.3      Ray Banister	     The SP was incorrectly raising ERR_NO_PIN_DCF.       */
/*                                           The spec does not require this, you just set StausId */
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
	DECLARE @PINExists BIT = 'False'
	DECLARE @PINRequested BIT = 'False'
	
	-- Check all Parameters are there
	IF (@PIN is null) OR (ISNUMERIC(@PIN) = 0)
	BEGIN
		RAISERROR(60000,16,1)		-- ERR_INVALID_PARAMETERS
		RETURN
	END
	
	-- Check if PIN already allocated
	EXEC dbo.sp_ValidatePINExists @PIN, @PINExists OUTPUT
	IF @PINExists = 'True'
	BEGIN
		-- Return "Initialised" and the Url
		SET @Url = (SELECT Url FROM dbo.Site WHERE Pin = @PIN)
		SET @StatusId = 2
		RETURN
	END
		
	-- Check if PIN awaiting allocation
	EXEC dbo.sp_ValidatePINRequested @PIN, @PINRequested OUTPUT
	IF @PINRequested = 'True'
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

