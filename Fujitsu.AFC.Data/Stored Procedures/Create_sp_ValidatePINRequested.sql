/* ============================================================================================== */
/* Name:        sp_ValidatePINRequested                                                           */
/* Author:		Ray Banister                                                                      */
/* Create date: 23/03/2016                                                                        */
/*                                                                                                */
/* Description:	Checks if the selected PIN has already been requested, via a previous AllocatePin */
/*              request but has not yet been processed.                                           */
/*		                                                                                          */
/* Mandatory Input Parameters: @PIN                                                               */
/* Optional Input Parameters:  None                                                               */
/* Returns: @PINRequested as an OUT parameter                                                     */
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
/* 22/03/2016     0.0      Ray Banister	     First implementation                                 */  
/* 30/03/2016     0.1	   Ray Banister      Replaced 0/1 with True/False                         */
/* 12/04/2016     0.2      Matt Jordan       Removed Explicit USE database                        */
/* 05/05/2016     0.3      Ray Banister      Method name corrected                                */
/* 02/08/2016     0.4      Ray Banister      Timestamp comparison now <= (ref: TFS: 32618)        */
/*                                                                                                */
/* ============================================================================================== */

CREATE PROCEDURE [dbo].[sp_ValidatePINRequested] 
	@PIN INT,
	@PINRequested BIT OUT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	IF EXISTS(SELECT  1 FROM dbo.Task
		WHERE (Handler = 'OperationsHandler' AND Name = 'AllocatePin' AND Pin = @PIN AND InsertedDate <= GETDATE()))  
		SET @PINRequested = 'True'
	ELSE SET @PINRequested = 'False'
END

GO

