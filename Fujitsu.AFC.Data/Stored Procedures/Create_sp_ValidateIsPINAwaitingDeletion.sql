/* ============================================================================================== */
/* Name:        sp_ValidateIsPIN_AwaitingDeletion                                                 */
/* Author:		Ray Banister                                                                      */
/* Create date: 17/08/2016                                                                        */
/*                                                                                                */
/* Description:	Checks if the selected PIN is awaiting a DeletePIN request                        */
/*              but has not yet been processed.                                                   */
/*		                                                                                          */
/* Mandatory Input Parameters: @PIN                                                               */
/* Optional Input Parameters:  None                                                               */
/* Returns: @PINAwaitingDeletion as an OUT parameter                                              */
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
/* 17/08/2016     0.0      Ray Banister	     First implementation                                 */  
/*                                                                                                */
/* ============================================================================================== */

CREATE PROCEDURE [dbo].[sp_ValidateIsPIN_AwaitingDeletion] 
	@PIN INT,
	@PINAwaitingDeletion BIT OUT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	IF EXISTS(SELECT  1 FROM dbo.Task
		WHERE (Handler = 'OperationsHandler' AND Name = 'DeletePin' AND Pin = @PIN AND InsertedDate <= GETDATE()))  
		SET @PINAwaitingDeletion = 'True'
	ELSE SET @PINAwaitingDeletion = 'False'
END


GO

