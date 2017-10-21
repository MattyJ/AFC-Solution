/* ============================================================================================== */
/* Name:        sp_eAspire_ChangePrimaryProject                                                   */
/* Author:		Ray Banister                                                                      */
/* Create date: 05/05/2016                                                                        */
/*                                                                                                */
/* Description:	Adds a ChangePrimaryProject task to the list providing the PIN exists,            */ 
/*				the PIN is not a FromPIN in a pending merge and the Dictionary is valid XML.      */
/*                                                                                                */
/*		                                                                                          */
/* Mandatory Input Parameters: @PIN, @CurrentProjectId, @NewProjectId, @Dictionary                */
/* Optional Input Parameters:                                                                     */
/* Returns:                                                                                       */
/* Errors Raised: ERR_NO_PIN_DCF, ERR_DICT_INVAL, ERR_CASE_IN_USE,ERR_PIN_BEING_MERGED            */
/*                ERR_INVALID_PARAMETERS                                                          */
/*                                                                                                */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 05/05/2016     0.0      Ray Banister	     First implementation                                 */
/* 18/05/2016     0.1      Ray Banister	     Check for the two ProjectIds being the same added    */
/* 25/05/2016     0.2      Ray Banister      Now check for 'pending' PIN                          */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */
        
CREATE PROCEDURE [dbo].[sp_eAspire_ChangePrimaryProject] 
    @PIN INT = NULL, 
    @CurrentProjectId INT = NULL,
    @NewProjectId INT = NULL,
    @Dictionary NVARCHAR (MAX) = NULL
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;
    DECLARE @RESULT NVARCHAR(300) = ''
    DECLARE @PINExists AS BIT = 0
    DECLARE @IsValid AS BIT = 0
    DECLARE @IsMergeFromPIN AS BIT = 0
	DECLARE @PINRequested AS BIT = 'False'
        	
    -- Check all Parameters are there
    IF (@PIN is null) OR (ISNUMERIC(@PIN) = 0) OR 
        (@CurrentProjectId IS NULL) OR (ISNUMERIC(@CurrentProjectId) = 0) OR
        (@NewProjectId IS NULL) OR (ISNUMERIC(@NewProjectId) = 0) OR
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

		-- Check NewProjectId <> CurrentProjectId as that would be a bit daft
    IF @NewProjectId = @CurrentProjectId
    BEGIN
        RAISERROR(60016,16,0)		-- ERR_NEW_PROJECT_EQUALS_CURRENT_PROJECT
        RETURN
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
        
        
    -- All preliminary checks are OK so write the Task to the Task Table
    INSERT INTO dbo.Task (PIN,Dictionary, CurrentProjectId,NewProjectId,
        					Handler,Name,Frequency,InsertedDate,InsertedBy,UpdatedDate,UpdatedBy)
    VALUES (@PIN,@Dictionary,@CurrentProjectId,@NewProjectId,
        		'OperationsHandler','ChangePrimaryProject','O',GETDATE(),SYSTEM_USER,GETDATE(),SYSTEM_USER)	
        			
END
        
    

GO

