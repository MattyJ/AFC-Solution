/* ============================================================================================== */
/* Name:        sp_eAspire_CloseCase                                                              */
/* Author:		Ray Banister                                                                      */
/* Create date: 03/05/2016                                                                        */
/*                                                                                                */
/* Description:	Close the Case                                                                    */
/*                                                                                                */
/*		                                                                                          */
/* Mandatory Input Parameters: @PIN, @CaseId, @ProjectId, @IsPrimary, @Dictionary                 */
/* Optional Input Parameters:                                                                     */
/* Returns:                                                                                       */
/* Errors Raised: ERR_INVALID_PARAMETERS, ERR_NO_PIN_DCF, ERR_NO_CASE_DCF ERR_PIN_BEING_MERGED    */
/*                ERR_DICT_INVAL                                                                  */
/* ERR_PIN_BEING_MERGED will be raised IF there is an outstanding (i.e. earlier) MergePIN task    */
/* and the PIN = FromPIN in that MergePIN request.                                                */
/*                                                                                                */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 03/05/2016     0.0      Ray Banister	     First implementation                                 */  
/* 05/05/2016     0.1      Matt Jordan       IsPrimary superfluous as per email Andy East         */
/* 17/05/2016     0.2      Ray Banister      Dictionary was not being checked for NULL            */
/* 25/05/2016     0.3      Ray Banister      Now check for 'pending' PIN and Case                 */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */

CREATE PROCEDURE [dbo].[sp_eAspire_CloseCase] 
	-- Add the parameters for the stored procedure here
	@PIN INT = NULL,
	@ProjectId INT = NULL,
	@CaseId INT = NULL,
	@Dictionary NVARCHAR(MAX) = NULL
	 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @RESULT NVARCHAR(300) = ''
	DECLARE @PINExists AS BIT = 'False'
	DECLARE @CaseIdInUse AS BIT = 'False'
	DECLARE @IsMergeFromPin AS BIT = 'False'
	DECLARE @IsValid AS BIT = 'False'
	DECLARE @PINRequested AS BIT = 'False'
	DECLARE @CaseIdRequested AS BIT = 'False'
	
	-- Check all Parameters are there
	IF (@PIN is NULL) OR (ISNUMERIC(@PIN) = 0) OR
	   (@ProjectId is NULL) OR (ISNUMERIC(@ProjectId) = 0) OR
	   (@CaseId is NULL) OR (ISNUMERIC(@CaseId) = 0) OR
	   (NULLIF(@Dictionary,'') IS NULL) 

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

	-- Check Dictionary XML is valid. 
	EXEC dbo.sp_ValidateDictionary @Dictionary, @IsValid OUTPUT
	IF @IsValid = 0
	BEGIN
		RAISERROR(60007,16,0)
		RETURN
	END
	
    -- Check if there is an earlier 'merge' task with the same PIN as the FromPIN
	EXEC dbo.sp_ValidateIsPIN_MergeFromPIN @PIN, @IsMergeFromPin OUTPUT
	IF @IsMergeFromPin = 'True'
		BEGIN
			RAISERROR(60012,16,0)	-- PIN being Merged
			RETURN
		END 

	
	-- All preliminary checks are OK so write the Task to the Task Table
	INSERT INTO dbo.Task (PIN,ProjectId,CaseId,Dictionary,Handler,Name,Frequency,InsertedDate,InsertedBy,UpdatedDate,UpdatedBy)
	VALUES (@PIN,@Projectid,@CaseId,@Dictionary,'OperationsHandler','CloseCase','O',GETDATE(),SYSTEM_USER,GETDATE(),SYSTEM_USER)	
			
END

GO

