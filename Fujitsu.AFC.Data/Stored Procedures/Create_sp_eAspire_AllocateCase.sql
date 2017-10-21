/* ============================================================================================== */
/* Name:        sp_eAspire_AllocateCase                                                           */
/* Author:		Ray Banister                                                                      */
/* Create date: 23/03/2016                                                                        */
/*                                                                                                */
/* Description:	Adds an AllocateCase task to the list providing the PIN exists, the CaseId is     */
/*              unique and the Dictionary, if submitted is valid XML.                             */
/*                                                                                                */
/*		                                                                                          */
/* Mandatory Input Parameters: @PIN, @ProjectId, @CaseId,@CaseTitle,@IsPrimary,@Dictionary        */
/* Optional Input Parameters:                                                                     */
/* Returns:                                                                                       */
/* Errors Raised: ERR_NO_PIN_DCF, ERR_DICT_INVAL, ERR_CASE_IN_USE,ERR_PIN_BEING_MERGED            */
/*                ERR_CASEID_ALREADY_REQUESTED,ERR_INVALID_PARAMETERS                             */
/*                                                                                                */
/*                                                                                                */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 23/03/2016     0.0      Ray Banister	     First implementation                                 */
/* 12/04/2016     0.1      Matt Jordan       Removed Explicit USE database                        */  
/* 21/04/2016     0.2      Ray Banister      Added check for Pending PIN Allocation and pending   */
/*                                           Allocate Case with same CaseId                       */  
/* 03/05/2016     0.3      Ray Banister      Dictionary size changed to NVARCHAR(MAX)             */
/* 05/05/2016     0.4      Ray Banister      Dictionary is now MANDATORY                          */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */
        
CREATE PROCEDURE [dbo].[sp_eAspire_AllocateCase] 
    -- Add the parameters for the stored procedure here
    @PIN INT = NULL, 
    @ProjectId INT = NULL,
    @CaseId INT = NULL,
    @CaseTitle NVARCHAR(100) = NULL,
    @Dictionary NVARCHAR (MAX) = NULL,
    @IsPrimary BIT = NULL
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;
    DECLARE @RESULT NVARCHAR(300) = ''
    DECLARE @PINExists AS BIT = 0
    DECLARE @IsValid AS BIT = 0
    DECLARE @CaseIdInUse AS BIT = 0
    DECLARE @IsMergeFromPIN AS BIT = 0
    DECLARE @PINRequested AS BIT = 0
    DECLARE @CaseIdRequested AS BIT = 0
        	
    -- Check all Parameters are there
    IF (@PIN is null) OR (ISNUMERIC(@PIN) = 0) OR 
        (@ProjectId IS NULL) OR (ISNUMERIC(@ProjectId) = 0) OR
        (@CaseId IS NULL) OR (ISNUMERIC(@CaseId) = 0) OR
        (@IsPrimary IS NULL) OR (NULLIF(@CaseTitle,'') IS NULL) OR
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
         	
    -- Check if PIN is in use for an earlier 'merge' as the FromPin - This is an error if it does not exist
    EXEC dbo.sp_ValidateIsPIN_MergeFromPIN @PIN, @IsMergeFromPIN OUTPUT
    IF @IsMergeFromPIN = 1
    BEGIN
        RAISERROR(60012,16,0)		-- ERR_PIN_BEING_MERGED
        RETURN
    END
        	
    -- Check if the CaseId has already been used - This is an error if it has
    EXEC dbo.sp_ValidateCaseIdInUse @CaseId, @CaseIdInUse OUTPUT
    IF @CaseIdInUse = 1
    BEGIN
        RAISERROR(60003,16,0)		-- ERR_CASE_IN_USE
        RETURN
    END
        	
    -- Check if someone has already requested a Case with the same CaseId
    EXEC dbo.sp_ValidateCaseIdRequested @CaseId, @CaseIdRequested OUTPUT
    IF @CaseIdRequested = 1
    BEGIN
        RAISERROR(60015,16,0)		-- ERR_CASEID_ALREADY_REQUESTED
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
    INSERT INTO dbo.Task (PIN,ProjectId,CaseId,CaseTitle,Dictionary,IsPrimary,
        					Handler,Name,Frequency,InsertedDate,InsertedBy,UpdatedDate,UpdatedBy)
    VALUES (@PIN,@ProjectId,@CaseId,@CaseTitle,@Dictionary,@IsPrimary,
        		'OperationsHandler','AllocateCase','O',GETDATE(),SYSTEM_USER,GETDATE(),SYSTEM_USER)	
        			
END
        
    
GO

