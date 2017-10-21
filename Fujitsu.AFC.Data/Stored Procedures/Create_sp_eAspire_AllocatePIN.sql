/* ============================================================================================== */
/* Name:        sp_eAspire_AllocatePIN                                                            */
/* Author:		Ray Banister                                                                      */
/* Create date: 22/03/2016                                                                        */
/*                                                                                                */
/* Description:	Allocate a new PIN if it doesn't already exist, is in use and there are some      */
/*              Provisioned (unallocated) Sites available.                                        */
/*		                                                                                          */
/* Mandatory Input Parameters: @PIN, @SiteTitle,@Dictionary                                       */
/* Optional Input Parameters:                                                                     */
/* Returns:                    Site URL                                                           */
/* Errors Raised: ERR_NOSITES, ERR_PIN_IN_USE, ERR_DICT_INVAL, ERR_PIN_ALREADY_REQUESTED          */
/*   		      ERR_INVALID_PARAMETERS                                                          */
/*                                                                                                */
/*                                                                                                */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 22/03/2016     0.0      Ray Banister	     First implementation                                 */  
/* 23/03/2016     0.1      Ray Banister      Task name corrected                                  */
/* 30/03/2016     0.2      Ray Banister      Replaced 'PININUse with PINExists [Duplicate]        */
/* 12/04/2016     0.3      Matt Jordan       Removed Explicit USE database                        */
/* 03/05/2016     0.4      Ray Banister      Dictionary size changed to NVARCHAR(MAX)             */
/* 05/05/2016     0.5      Ray Banister      Dictionary is now MANDATORY                          */
/*                                                                                                */
/* ============================================================================================== */

CREATE PROCEDURE [dbo].[sp_eAspire_AllocatePIN] 
	-- Add the parameters for the stored procedure here
	@PIN INT =NULL, 
	@SiteTitle NVARCHAR(100) = NULL,
	@Dictionary NVARCHAR (MAX) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @RESULT NVARCHAR(300) = ''
	DECLARE @XML AS XML
	DECLARE @COUNT AS INTEGER = 0
	DECLARE @PINExists AS BIT = 0
	DECLARE @PINRequested AS BIT = 0
	DECLARE @IsValid AS BIT = 0
	
	-- Check all Parameters are there
	IF (@PIN is null) OR (ISNUMERIC(@PIN) = 0) OR 
		(NULLIF(@SiteTitle,'') IS NULL ) OR
		(NULLIF(@Dictionary,'') IS NULL) 
	BEGIN
		RAISERROR(60000,16,1)		-- ERR_INVALID_PARAMETERS
		RETURN
	END
	
	-- Check some Unallocated Site(s) Exist [ProvisionedSite Table]. Error if none available
	EXEC dbo.sp_ValidateAvailableSites @COUNT OUTPUT
	IF @COUNT = 0
	BEGIN
		RAISERROR(60001,16,0)
		RETURN
	END
	
	-- Check if PIN already in use [Site Table] - This is an error
	EXEC dbo.sp_ValidatePINExists @PIN, @PINExists OUTPUT
	IF @PINExists = 1
	BEGIN
		RAISERROR(60002,16,0)
		RETURN
	END
	
	-- Check if PIN already requested - This is an error
	EXEC dbo.sp_ValidatePINRequested @PIN, @PINRequested OUTPUT
	IF @PINRequested = 1
	BEGIN
		RAISERROR(60005,16,0)
		RETURN
	END
	
	-- Check Dictionary XML is valid. 
	EXEC dbo.sp_ValidateDictionary @Dictionary, @IsValid OUTPUT
	IF @IsValid = 0
	BEGIN
		RAISERROR(60007,16,0)
		RETURN
	END


	-- All preliminary checks are OK OK so write the Task to the Task Table
	INSERT INTO dbo.Task (PIN,SiteTitle,Dictionary,
							Handler,Name,Frequency,InsertedDate,InsertedBy,UpdatedDate,UpdatedBy)
	VALUES (@PIN,@SiteTitle,@Dictionary,
				'OperationsHandler','AllocatePin','O',GETDATE(),SYSTEM_USER,GETDATE(),SYSTEM_USER)	
			
END
	

GO


