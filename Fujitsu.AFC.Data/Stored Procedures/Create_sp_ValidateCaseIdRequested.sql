/* ============================================================================================== */
/* Name:        sp_ValidateCaseIdRequested                                                        */
/* Author:		Ray Banister                                                                      */
/* Create date: 21/04/2016                                                                        */
/*                                                                                                */
/* Description:	Checks if the selected CaseId has already been requested, via a previous          */
/*              AllocateCaseId  request but has not yet been processed.                           */
/*		                                                                                          */
/* Mandatory Input Parameters: @CaseId                                                            */
/* Optional Input Parameters:  None                                                               */
/* Returns: @CaseIdRequested as an OUT parameter                                                  */
/* Errors Raised: None                                                                            */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 21/04/2016     0.0      Ray Banister	     First implementation                                 */  
/* 02/08/2016     0.1      Ray Banister      Timestamp comparison now <= (ref: TFS: 32618)        */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */

CREATE PROCEDURE [dbo].[sp_ValidateCaseIdRequested] 
	@CaseId INT,
	@CaseIdRequested BIT OUT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	IF EXISTS(SELECT  1 FROM dbo.Task
		WHERE (Handler = 'OperationsHandler' AND Name = 'AllocateCase' AND CaseId = @CaseId AND InsertedDate <= GETDATE()))  
		SET @CaseIdRequested = 'True'
	ELSE SET @CaseIdRequested = 'False'
END

GO

