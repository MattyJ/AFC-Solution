        /* ============================================================================================== */
        /* Name:        sp_eAspire_MergePIN                                                               */
        /* Author:		Ray Banister                                                                      */
        /* Create date: 11/04/2016                                                                        */
        /*                                                                                                */
        /* Description:	Merge DCF for two Service users                                                   */
        /*                                                                                                */
        /*		                                                                                          */
        /* Mandatory Input Parameters: @ToPIN, @FromPIN                                                   */
        /* Optional Input Parameters:  NONE                                                               */
        /* Returns:                                                                                       */
        /* Errors Raised: ERR_NO_PIN_DCF, ERR_PIN_BEING_MERGED, ERR_INVALID_PARAMETERS                    */
        /*                ERR_PIN_PENDING_DELETION                                                        */
        /*                                                                                                */
        /*                                                                                                */
        /* Copyright 2016 Fujitsu Services Ltd                                                            */
        /* ============================================================================================== */
        /*                                        Change Record                                           */
        /* ============================================================================================== */
        /*    Date      Version     Changed By                     Comments                               */
        /* ============================================================================================== */
        /* 11/04/2016     0.0      Ray Banister	     First implementation                                 */  
        /* 12/04/2016     0.1      Matt Jordan       Removed Explicit USE database                        */
        /* 06/05/2016     0.2      Matt Jordan       SELECT @RESULT removed                               */
        /* 25/05/2016     0.3      Ray Banister      Now check for 'pending' PINs                         */
        /* 18/08/2016     0.4      Ray Banister      Added check for PIN pending deletion                 */
        /*                                                                                                */
        /*                                                                                                */
        /* ============================================================================================== */
        
        CREATE PROCEDURE [dbo].[sp_eAspire_MergePIN] 
        	@ToPIN INT, 
        	@FromPIN INT
        	
        AS
        BEGIN
        	-- SET NOCOUNT ON added to prevent extra result sets from
        	-- interfering with SELECT statements.
        	SET NOCOUNT ON;
        	DECLARE @RESULT NVARCHAR(300) = ''
        	DECLARE @PINExists BIT = 'False'
        	DECLARE @IsMergeToPIN BIT = 'False'
        	DECLARE @IsMergeFromPIN BIT = 'False'
        	DECLARE @PINRequested AS BIT = 'False'
        	DECLARE @PINAwaitingDeletion AS BIT = 'False'
        	
        	-- Check all Parameters are there
        	IF (@ToPIN is NULL) OR (ISNUMERIC(@ToPIN) = 0) OR
        		(@FromPIN is NULL) or (ISNUMERIC(@FromPIN) = 0)
        	BEGIN
        		RAISERROR(60000,16,1)		-- ERR_INVALID_PARAMETERS
        		RETURN
        	END
        	
        	-- Check that ToPIN <> FromPIN as that's silly
        	IF @ToPIN = @FromPIN
        	BEGIN
        		RAISERROR(60014,16,1)		-- ERR_TOPIN_EQUALS_FROMPIN
        		RETURN
        	END
        
        	-- Check that both PINS exist
        
        	-- Check if ToPIN already in use [Site Table] - This is an error if it does not exist
            -- or if it is not pending allocation
            EXEC dbo.sp_ValidatePINExists @ToPIN, @PINExists OUTPUT
            IF @PINExists = 'False'
            BEGIN
                EXEC dbo.sp_ValidatePINRequested @ToPIN, @PINRequested OUTPUT
                IF @PINRequested = 'False'
                BEGIN
                	RAISERROR(60004,16,0)	-- PIN not found nor awaiting allocation
                	RETURN
                END
            END
        
        	-- Check if FromPIN already in use [Site Table] - This is an error if it does not exist
            -- or if it is not pending allocation
        	EXEC dbo.sp_ValidatePINExists @FromPIN, @PINExists OUTPUT
            IF @PINExists = 'False'
            BEGIN
                EXEC dbo.sp_ValidatePINRequested @FromPIN, @PINRequested OUTPUT
                IF @PINRequested = 'False'
                BEGIN
                	RAISERROR(60004,16,0)	-- PIN not found nor awaiting allocation
                	RETURN
                END
            END
        
        	-- Check if the ToPIN is the FromPIN in a pending merge
        	EXEC dbo.sp_ValidateIsPIN_MergeFromPIN @ToPIN, @IsMergeFromPIN OUTPUT
        	IF @IsMergeFromPIN = 'True'
        	BEGIN
        		RAISERROR(60012,16,1)		-- ERR_PIN_BEING_MERGED
        		RETURN
        	END
        	
        	-- Check if the FromPIN is the FromPIN in a pending merge
        	EXEC dbo.sp_ValidateIsPIN_MergeFromPIN @FromPIN, @IsMergeFromPIN OUTPUT
        	IF @IsMergeFromPIN = 'True'
        	BEGIN
        		RAISERROR(60012,16,1)		-- ERR_PIN_BEING_MERGED
        		RETURN
        	END
        	
        	-- Check if the FromPIN is in a pending DeletePIN Request
        	EXEC dbo.sp_ValidateIsPIN_AwaitingDeletion @FromPIN, @PINAwaitingDeletion OUTPUT
        	IF @PINAwaitingDeletion = 'True'
        	BEGIN
        		RAISERROR(60018,16,1)		-- ERR_PIN_PENDING_DELETION
        		RETURN
        	END
        	
        	-- Check if the ToPIN is in a pending DeletePIN Request
        	EXEC dbo.sp_ValidateIsPIN_AwaitingDeletion @ToPIN, @PINAwaitingDeletion OUTPUT
        	IF @PINAwaitingDeletion = 'True'
        	BEGIN
        		RAISERROR(60018,16,1)		-- ERR_PIN_PENDING_DELETION
        		RETURN
        	END
        	
        	-- All preliminary checks are OK so write the Task to the Task Table
        	INSERT INTO dbo.Task (ToPIN,FromPIN,Handler,Name,Frequency,InsertedDate,InsertedBy,UpdatedDate,UpdatedBy)
        	VALUES (@ToPIN,@FromPIN,'OperationsHandler','MergePin','O',GETDATE(),SYSTEM_USER,GETDATE(),SYSTEM_USER)	
        
        		
        END
        
    
GO

