/* ============================================================================================== */
/* Name:        sp_eAspire_UpdateCaseTitle                                                        */
/* Author:		Ray Banister                                                                      */
/* Create date: 22/04/2016                                                                        */
/*                                                                                                */
/* Description:	Update the Case Title with a new value                                            */
/*                                                                                                */
/*		                                                                                          */
/* Mandatory Input Parameters: @PIN, @CaseTitle , @CaseId                                         */
/* Optional Input Parameters:                                                                     */
/* Returns:                                                                                       */
/* Errors Raised: ERR_INVALID_PARAMETERS, ERR_NO_PIN_DCF, ERR_NO_CASE_DCF, ERR_PIN_BEING_MERGED   */
/*                                                                                                */
/*                                                                                                */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 22/04/2016     0.0      Ray Banister	     First implementation                                 */ 
/* 11/05/2016	  0.1      Ray Banister      Fixed bug - @IsMergeFromPIN was not set to OUTPUT    */
/* 25/05/2016     0.2      Ray Banister      Now check for 'pending' PIN and Case                 */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */

CREATE PROCEDURE [dbo].[sp_eAspire_UpdateCaseTitle] 
	-- Add the parameters for the stored procedure here
	@PIN INT = NULL,
	@CaseId INT = NULL,
	@CaseTitle NVARCHAR(100) = NULL 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @RESULT NVARCHAR(300) = ''
	DECLARE @PINExists AS BIT = 'False'
	DECLARE @CaseIdInUse AS BIT = 'False'
	DECLARE @IsMergeFromPIN AS BIT = 'False'
	DECLARE @PINRequested AS BIT = 'False'
	DECLARE @CaseIdRequested AS BIT = 'False'
	
	-- Check all Parameters are there
	IF (@PIN is null) OR (ISNUMERIC(@PIN) = 0) OR 
	   (@CaseId is null) OR (ISNUMERIC(@CaseId) = 0) OR
	   (NULLIF(@CaseTitle,'')IS NULL ) 
	BEGIN
		RAISERROR(60000,16,1)		-- Error in Parameters
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
		
	-- Check if the PIN is a "from" PIN in a pending merge
	exec dbo.sp_ValidateIsPIN_MergeFromPIN @PIN,@IsMergeFromPIN OUTPUT
	IF @IsMergeFromPIN = 'True'
		BEGIN
			RAISERROR(60012,16,0)	-- ERR_PIN_BEING_MERGED
			RETURN
		END 

	-- All preliminary checks are OK so write the Task to the Task Table
	INSERT INTO dbo.Task (PIN,CaseId,CaseTitle,
							Handler,Name,Frequency,InsertedDate,InsertedBy,UpdatedDate,UpdatedBy)
	VALUES (@PIN,@CaseId,@CaseTitle,
				'OperationsHandler','UpdateCaseTitle','O',GETDATE(),SYSTEM_USER,GETDATE(),SYSTEM_USER)	
			
END


GO

