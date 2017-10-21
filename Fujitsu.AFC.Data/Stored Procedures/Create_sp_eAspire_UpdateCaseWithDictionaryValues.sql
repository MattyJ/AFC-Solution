/* ============================================================================================== */
/* Name:        sp_eAspire_UpdateCaseWithDictionaryValues                                         */
/* Author:		Ray Banister                                                                      */
/* Create date: 05/05/2016                                                                        */
/*                                                                                                */
/* Description:	Update the Case’s Dictionary data with new values                                 */
/*		                                                                                          */
/* Mandatory Input Parameters: @CaseId, @Dictionary, @PIN                                         */
/* Optional Input Parameters:                                                                     */
/* Returns:                                                                                       */
/* Errors Raised: ERR_INVALID_PARAMETERS, ERR_NO_CASE_DCF, ERR_DICT_INVAL. ERR_NO_PIN_DCF         */
/*                                                                                                */
/*                                                                                                */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 05/05/2016     0.0      Ray Banister	     First implementation                                 */  
/* 16/05/2016     0.1      Ray Banister	     Method name corrected                                */  
/* 25/05/2016     0.2      Ray Banister      Now check for 'pending' PIN and Case                 */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */

CREATE PROCEDURE [dbo].[sp_eAspire_UpdateCaseWithDictionaryValues] 
	-- Add the parameters for the stored procedure here
	@PIN INT = NULL,
	@CaseId INT = NULL,
	@Dictionary NVARCHAR (MAX) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @RESULT NVARCHAR(300) = ''
	DECLARE @CaseIdInUse AS BIT = 'False'
	DECLARE @PINExists AS BIT = 'False'
	DECLARE @IsValid AS BIT = 'False'
	DECLARE @PINRequested AS BIT = 'False'
	DECLARE @CaseIdRequested AS BIT = 'False'

	
	-- Check all Parameters are there
	IF (@PIN is NULL) OR (ISNUMERIC(@PIN) = 0) OR 
		(@CaseId is NULL) OR (ISNUMERIC(@CaseId) = 0) OR 
		(NULLIF(@Dictionary,'')IS NULL ) 
	BEGIN
		RAISERROR(60000,16,1)		-- ERR_INVALID_PARAMETERS
		RETURN
	END
	
    -- Check if PIN already in use [Site Table] - This is an error if it does not exist
    -- or if it is not pending allocation
    EXEC dbo.sp_ValidatePINExists @PIN, @PINExists OUTPUT
    IF @PINExists = 'False'
    BEGIN
        EXEC dbo.sp_ValidatePINRequested @PIN, @PINRequested OUTPUT
        IF @PINRequested = 'False'
        BEGIN
        	RAISERROR(60004,16,0)	-- PIN not found nor awaiting allocation
        	RETURN
        END
    END
	
    -- Check if Case already in use [Library Table] - This is an error if it does not exist
    -- or if it is not pending allocation
    EXEC dbo.sp_ValidateCaseIdInUse @CaseId, @CaseIdInUse OUTPUT
    IF @CaseIdInUse = 'False'
    BEGIN
		EXEC dbo.sp_ValidateCaseIdRequested @CaseId, @CaseIdRequested OUTPUT
    	IF @CaseIdRequested = 'False'
		BEGIN
			RAISERROR(60006,16,0)	-- Case not found nor awaiting allocation [ERR_NO_CASE_DCF]
            RETURN
		END
    END	

	
	-- Check Dictionary XML is valid. 
	EXEC dbo.sp_ValidateDictionary @Dictionary, @IsValid OUTPUT
	IF @IsValid = 0
	BEGIN
		RAISERROR(60007,16,0)		-- ERR_DICT_INVAL
		RETURN
	END

	-- All preliminary checks are OK so write the Task to the Task Table
	INSERT INTO dbo.Task (PIN,CaseId,Dictionary,
							Handler,Name,Frequency,InsertedDate,InsertedBy,UpdatedDate,UpdatedBy)
	VALUES (@PIN,@CaseId,@Dictionary,
				'OperationsHandler','UpdateCaseWithDictionaryValues','O',GETDATE(),SYSTEM_USER,GETDATE(),SYSTEM_USER)	
			
END
	

GO

