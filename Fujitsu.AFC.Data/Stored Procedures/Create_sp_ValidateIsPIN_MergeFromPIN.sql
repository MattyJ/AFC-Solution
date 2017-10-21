/* ============================================================================================== */
/* Name:        sp_ValidateIsPIN_MergeFromPIN                                                     */
/* Author:		Ray Banister                                                                      */
/* Create date: 23/03/2016                                                                        */
/*                                                                                                */
/* Description:	Checks if the selected PIN = FromPin in any earlier 'merge' task awaiting         */ 
/*              processing.                                                                       */
/*                                                                                                */
/*		                                                                                          */
/* Mandatory Input Parameters: @PIN                                                               */
/* Optional Input Parameters:  None                                                               */
/* Returns: @IsMergeFromPIN as an OUT parameter                                                   */
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
/* 23/03/2016     0.0      Ray Banister	     First implementation                                 */  
/* 12/04/2016     0.1      Matt Jordan       Removed Explicit USE database                        */
/* 02/08/2016     0.2      Ray Banister      Timestamp comparison now <= (ref: TFS: 32618)        */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */

CREATE PROCEDURE [dbo].[sp_ValidateIsPIN_MergeFromPIN] 
	@PIN INT,
	@IsMergeFromPIN BIT OUT
	
AS
BEGIN
	-- Check if there is an earlier 'merge' task with the same PIN
	IF EXISTS(SELECT  1 FROM dbo.Task
		WHERE (Handler = 'OperationsHandler' AND Name = 'MergePin' AND FromPin = @PIN AND InsertedDate <= GETDATE())) 
		SET @IsMergeFromPIN = 'True'
	ELSE SET @IsMergeFromPIN = 'False'
END
	

GO

