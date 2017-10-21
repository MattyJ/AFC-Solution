/* ============================================================================================== */
/* Name:        sp_eAspire_MoveCase                                                               */
/* Author:		Ray Banister                                                                      */
/* Create date: 05/05/2016                                                                        */
/*                                                                                                */
/* Description:	Adds a MoveCase task to the list providing the PIN exists, the CaseId is          */
/*              unique and the Dictionary, if submitted is valid XML.                             */
/*                                                                                                */
/*		                                                                                          */
/* Mandatory Input Parameters: @PIN,  @CaseId, @CurrentProjectId, @NewProjectId,@IsPrimary,       */
/*                             @Dictionary                                                        */
/* Optional Input Parameters:                                                                     */
/* Returns:                                                                                       */
/* Errors Raised: ERR_NO_PIN_DCF, ERR_DICT_INVAL, ERR_NO_CASE_DCF,ERR_PIN_BEING_MERGED            */
/*                ERR_INVALID_PARAMETERS, ERR_NEW_PROJECT_EQUALS_CURRENT_PROJECT                  */
/*                                                                                                */
/*                                                                                                */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 05/05/2016     0.0      Ray Banister      First Implementation                                 */
/* 25/05/2016     0.2      Ray Banister      Now check for 'pending' PIN and Case                 */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */
        
CREATE PROCEDURE [dbo].[sp_eAspire_MoveCase] 
    @PIN INT = NULL, 
    @CaseId INT = NULL,
    @CurrentProjectId INT = NULL,
    @NewProjectId INT = NULL,
    @IsPrimary BIT = NULL,
    @Dictionary NVARCHAR (MAX) = NULL
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;
    DECLARE @RESULT NVARCHAR(300) = ''
    DECLARE @PINExists AS BIT = 'False'
    DECLARE @IsValid AS BIT = 'False'
    DECLARE @CaseIdInUse AS BIT = 'False'
    DECLARE @IsMergeFromPIN AS BIT = 'False'
	DECLARE @PINRequested AS BIT = 'False'
	DECLARE @CaseIdRequested AS BIT = 'False'
        	
    -- Check all Parameters are there
    IF (@PIN is null) OR (ISNUMERIC(@PIN) = 0) OR 
        (@CaseId IS NULL) OR (ISNUMERIC(@CaseId) = 0) OR
        (@CurrentProjectId IS NULL) OR (ISNUMERIC(@CurrentProjectId) = 0) OR
        (@NewProjectId IS NULL) OR (ISNUMERIC(@NewProjectId) = 0) OR
        (@IsPrimary IS NULL) OR 
		(NULLIF(@Dictionary,'') IS NULL) 
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

         	
    -- Check if PIN is in use for an earlier 'merge' as the FromPin - This is an error if it does not exist
    EXEC dbo.sp_ValidateIsPIN_MergeFromPIN @PIN, @IsMergeFromPIN OUTPUT
    IF @IsMergeFromPIN = 1
    BEGIN
        RAISERROR(60012,16,0)		-- ERR_PIN_BEING_MERGED
        RETURN
    END
      	
    -- Check Dictionary XML is valid. 
    EXEC dbo.sp_ValidateDictionary @Dictionary, @IsValid OUTPUT
    IF @IsValid = 0
    BEGIN
        RAISERROR(60007,16,0)		-- ERR_DICT_INVAL
        RETURN
    END

	-- Check NewProjectId <> CurrentProjectId as that would be a bit daft
    IF @NewProjectId = @CurrentProjectId
    BEGIN
        RAISERROR(60016,16,0)		-- ERR_NEW_PROJECT_EQUALS_CURRENT_PROJECT
        RETURN
    END
    
      
    -- All preliminary checks are OK so write the Task to the Task Table
    INSERT INTO dbo.Task (PIN,CaseId,CurrentProjectId,NewProjectId,IsPrimary,Dictionary,
        					Handler,Name,Frequency,InsertedDate,InsertedBy,UpdatedDate,UpdatedBy)
    VALUES (@PIN,@CaseId,@CurrentProjectId,@NewProjectId,@IsPrimary,@Dictionary,
        		'OperationsHandler','MoveCase','O',GETDATE(),SYSTEM_USER,GETDATE(),SYSTEM_USER)	
        			
END
        
    
GO

