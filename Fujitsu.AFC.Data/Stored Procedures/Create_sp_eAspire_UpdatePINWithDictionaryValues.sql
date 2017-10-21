/* ============================================================================================== */
/* Name:        sp_eAspire_UpdatePINWithDictionaryValues                                          */
/* Author:		Ray Banister                                                                      */
/* Create date: 05/05/2016                                                                        */
/*                                                                                                */
/* Description:	Update the PIN’s Dictionary data with new values                                  */
/*		                                                                                          */
/* Mandatory Input Parameters: @PIN, @Dictionary                                                  */
/* Optional Input Parameters:                                                                     */
/* Returns:                                                                                       */
/* Errors Raised: ERR_INVALID_PARAMETERS, ERR_NO_PIN_DCF, ERR_DICT_INVAL                          */
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

CREATE PROCEDURE [dbo].[sp_eAspire_UpdatePINWithDictionaryValues] 
	-- Add the parameters for the stored procedure here
	@PIN INT = NULL,
	@Dictionary NVARCHAR (MAX) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @RESULT NVARCHAR(300) = ''
	DECLARE @PINExists AS BIT = 'False'
	DECLARE @IsValid AS BIT = 'False'
	DECLARE @PINRequested AS BIT = 'False'

	
	-- Check all Parameters are there
	IF (@PIN is null) OR (ISNUMERIC(@PIN) = 0) OR 
		(NULLIF(@Dictionary,'')IS NULL ) 
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
	
	-- Check Dictionary XML is valid. 
	EXEC dbo.sp_ValidateDictionary @Dictionary, @IsValid OUTPUT
	IF @IsValid = 0
	BEGIN
		RAISERROR(60007,16,0)	-- ERR_DICT_INVAL
		RETURN
	END

	-- All preliminary checks are OK so write the Task to the Task Table
	INSERT INTO dbo.Task (PIN,Dictionary,
							Handler,Name,Frequency,InsertedDate,InsertedBy,UpdatedDate,UpdatedBy)
	VALUES (@PIN,@Dictionary,
				'OperationsHandler','UpdatePinWithDictionaryValues','O',GETDATE(),SYSTEM_USER,GETDATE(),SYSTEM_USER)	
			
END
	

GO

