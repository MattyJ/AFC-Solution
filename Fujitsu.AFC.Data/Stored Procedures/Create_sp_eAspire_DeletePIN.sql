/* ============================================================================================== */
/* Name:        sp_eAspire_DeletePIN                                                              */
/* Author:		Ray Banister                                                                      */
/* Create date: 30/03/2016                                                                        */
/*                                                                                                */
/* Description:	Delete the PIN Site                                                               */
/*                                                                                                */
/*		                                                                                          */
/* Mandatory Input Parameters: @PIN                                                               */
/* Optional Input Parameters:                                                                     */
/* Returns:                                                                                       */
/* Errors Raised: ERR_INVALID_PARAMETERS, ERR_NO_PIN_DCF, ERR_PIN_BEING_MERGED                    */
/*                                                                                                */
/* ERR_PIN_BEING_MERGED will be raised IF there is an outstanding (i.e. earlier) MergePIN task    */
/* and the PIN = (FromPIN OR ToPIN) in that MergePIN request.                                     */
/*                                                                                                */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 23/03/2016     0.0      Ray Banister	     First implementation                                 */  
/* 12/04/2016     0.1      Matt Jordan       Removed Explicit USE database                        */
/* 16/05/2016     0.2      Ray Banister	     Removed error on "ToPIN" check                       */  
/* 25/05/2016     0.3      Ray Banister      Now check for 'pending' PIN                          */
/* 19/08/2016     0.4      Ray Banister	     Restored error on "ToPIN" check                      */  
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */

Create PROCEDURE [dbo].[sp_eAspire_DeletePIN] 
	-- Add the parameters for the stored procedure here
	@PIN INT = NULL 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @RESULT NVARCHAR(300) = ''
	DECLARE @PINExists AS BIT = 0
	DECLARE @IsMergeFromPin AS BIT = 'False'
	DECLARE @IsMergeToPin AS BIT = 'False'
	DECLARE @PINRequested AS BIT = 'False'
	
	-- Check all Parameters are there
	IF (@PIN is null) OR (ISNUMERIC(@PIN) = 0) 
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
	
-- Check if there is an earlier 'merge' task with the same PIN as the FromPIN
	EXEC dbo.sp_ValidateIsPIN_MergeFromPIN @PIN, @IsMergeFromPin OUTPUT
	IF @IsMergeFromPin = 'True'
		BEGIN
			RAISERROR(60012,16,0)	-- PIN being Merged
			RETURN
		END 
		
-- Check if there is an earlier 'merge' task with the same PIN as the ToPIN
	EXEC dbo.sp_ValidateIsPIN_MergeToPIN @PIN, @IsMergeToPin OUTPUT
	IF @IsMergeToPin = 'True'
		BEGIN
			RAISERROR(60012,16,0)	-- PIN being Merged
			RETURN
		END 
	
	-- All preliminary checks are OK so write the Task to the Task Table
	INSERT INTO dbo.Task (PIN,Handler,Name,Frequency,InsertedDate,InsertedBy,UpdatedDate,UpdatedBy)
	VALUES (@PIN,'OperationsHandler','DeletePin','O',GETDATE(),SYSTEM_USER,GETDATE(),SYSTEM_USER)	
			
END
	

GO

