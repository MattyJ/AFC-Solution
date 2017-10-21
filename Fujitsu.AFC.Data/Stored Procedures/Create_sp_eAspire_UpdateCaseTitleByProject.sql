/* ============================================================================================== */
/* Name:        sp_eAspire_UpdateCaseTitleByProject                                               */
/* Author:		Ray Banister                                                                      */
/* Create date: 14/03/2016                                                                        */
/*                                                                                                */
/* Description:	Replace all the Project Names in the title of cases for the given ProjectId       */
/*                                                                                                */
/*		                                                                                          */
/* Mandatory Input Parameters: @ProjectName @ProjectId, @Dictionary                               */
/* Optional Input Parameters:                                                                     */
/* Returns:                                                                                       */
/* Errors Raised: ERR_DICT_INVAL                                                                  */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 11/04/2016     0.0      Ray Banister	     First implementation                                 */  
/* 12/04/2016     0.1      Ray Banister	     Removed the "USE" clause.                            */  
/* 26/04/2016     0.2      Ray Banister      Now added the code to insert the task                */
/* 05/05/2016     0.3      Ray Banister      Dictionary is now MANDATORY                          */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */

CREATE PROCEDURE [dbo].[sp_eAspire_UpdateCaseTitleByProject] 
	@ProjectId INT = NULL,
	@ProjectName NVARCHAR(100) = NULL,
	@Dictionary NVARCHAR(MAX) = NULL
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @IsValid AS BIT = 0

	DECLARE @RESULT NVARCHAR(300) = ''
	
	-- Check all Parameters are there
	IF (@ProjectId is null) OR 
		(ISNUMERIC(@ProjectId) = 0) OR 
		(NULLIF(@ProjectName,'')IS NULL ) OR
		(NULLIF(@Dictionary,'')IS NULL ) 
	BEGIN
		RAISERROR(60000,16,1)
		RETURN
	END

	-- Check Dictionary XML is valid. 
	EXEC dbo.sp_ValidateDictionary @Dictionary, @IsValid OUTPUT
	IF @IsValid = 0
	BEGIN
		RAISERROR(60007,16,0)
		RETURN
	END


	-- All preliminary checks are OK OK so write the Task to the Task Table
	INSERT INTO dbo.Task (ProjectId,ProjectName,Dictionary,
							Handler,Name,Frequency,InsertedDate,InsertedBy,UpdatedDate,UpdatedBy)
	VALUES (@ProjectId,@ProjectName,@Dictionary,
				'OperationsHandler','UpdateCaseTitleByProject','O',GETDATE(),SYSTEM_USER,GETDATE(),SYSTEM_USER)	

			
END
	

GO

