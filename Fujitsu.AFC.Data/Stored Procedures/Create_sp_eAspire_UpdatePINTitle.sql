/* ============================================================================================== */
/* Name:        sp_eAspire_UpdatePINTitle                                                         */
/* Author:		Ray Banister                                                                      */
/* Create date: 23/03/2016                                                                        */
/*                                                                                                */
/* Description:	Update the PIN’s Title with a new value                                           */
/*              [Also known as Update Service User Title]                                         */
/*		                                                                                          */
/* Mandatory Input Parameters: @PIN, @SiteTitle                                                   */
/* Optional Input Parameters:                                                                     */
/* Returns:                                                                                       */
/* Errors Raised: ERR_INVALID_PARAMETERS, ERR_NO_PIN_DCF, ERR_PIN_BEING_MERGED                    */
/*                                                                                                */
/* ERR_PIN_BEING_MERGED will be raised IF there is an outstanding (i.e. earlier) MergePIN task    */
/* and the PIN = FromPIN in that MergePIN request.                                                */
/*                                                                                                */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 23/03/2016     0.0      Ray Banister	     First implementation                                 */  
/* 23/03/2016	  0.1	   Ray Banister      Merge FromPin check now a validation stored          */
/*                                           procedure                                            */
/* 24/03/2016     0.2      Ray Banister      Added check to see if PIN is waiting allocation      */
/* 30/03/2016     0.3      Ray Banister      Call PINRequested instead of PINwaiting (duplicate)  */
/* 12/04/2016     0.4      Matt Jordan       Removed Explicit USE database                        */
/* ============================================================================================== */

CREATE PROCEDURE [dbo].[sp_eAspire_UpdatePINTitle] 
	-- Add the parameters for the stored procedure here
	@PIN INT = NULL,
	@SiteTitle NVARCHAR(100) = NULL 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @RESULT NVARCHAR(300) = ''
	DECLARE @PINExists AS BIT = 'False'
	DECLARE @IsMergeFromPin AS BIT = 'False'
	DECLARE @PINRequested AS BIT = 'False'
	
	-- Check all Parameters are there
	IF (@PIN is null) OR (ISNUMERIC(@PIN) = 0) OR (NULLIF(@SiteTitle,'')IS NULL ) 
	BEGIN
		RAISERROR(60000,16,1)		-- Error in Parameters
		RETURN
	END
	
	-- Check if PIN exists in use [Site Table] - If not, this is an error
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
	
-- Check if there is an earlier 'merge' task with the same PIN
	EXEC dbo.sp_ValidateIsPIN_MergeFromPIN @PIN, @IsMergeFromPin OUTPUT
	IF @IsMergeFromPin = 'True'
		BEGIN
			RAISERROR(60012,16,0)	-- PIN being Merged
			RETURN
		END 
	
	-- All preliminary checks are OK so write the Task to the Task Table
	INSERT INTO dbo.Task (PIN,SiteTitle,
							Handler,Name,Frequency,InsertedDate,InsertedBy,UpdatedDate,UpdatedBy)
	VALUES (@PIN,@SiteTitle,
				'OperationsHandler','UpdateServiceUserTitle','O',GETDATE(),SYSTEM_USER,GETDATE(),SYSTEM_USER)	
			
END
	

GO

