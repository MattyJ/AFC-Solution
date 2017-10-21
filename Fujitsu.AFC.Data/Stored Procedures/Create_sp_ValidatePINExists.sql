/* ============================================================================================== */
/* Name:        sp_ValidatePINExists                                                              */
/* Author:		Ray Banister                                                                      */
/* Create date: 23/03/2016                                                                        */
/*                                                                                                */
/* Description:	Checks if the selected PIN exists in either the Site Table or ProvisionedSite     */
/*              Table.                                                                            */
/*		                                                                                          */
/* Mandatory Input Parameters: @PIN                                                               */
/* Optional Input Parameters:  None                                                               */
/* Returns: @PINExits as an OUT parameter                                                         */
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
/* 12/04/2016     0.1      Matt Jordan       Removed Explicit USE database                        */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */

CREATE PROCEDURE [dbo].[sp_ValidatePINExists] 
	@PIN INT,
	@PinExists BIT OUT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	IF EXISTS(SELECT  1 FROM dbo.Site WHERE Pin = @PIN)
		SET @PinExists = 'True'
	-- May need to add the ProvisionedSite table in here but note name may be a GUID !!
	ELSE SET @PinExists = 'False'

END
	

GO


