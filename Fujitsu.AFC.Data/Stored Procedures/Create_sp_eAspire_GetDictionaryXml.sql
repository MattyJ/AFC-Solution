/* ============================================================================================== */
/* Name:        sp_eAspire_GetDictionaryXml                                                    */
/* Author:		Ray Banister                                                                      */
/* Create date: 29/03/2016                                                                        */
/*                                                                                                */
/* Description:	Retrieves a sample Dictionary XML                                                 */
/*                                                                                                */
/*		                                                                                          */
/* Mandatory Input Parameters: NONE                                                               */
/* Optional Input Parameters:  NONE                                                               */
/* Returns: @Dictionary                                                                           */
/* Errors Raised: NONE                                                                            */
/*                                                                                                */
/*                       TEST FUNCTION ONLY                                                       */
/*                                                                                                */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 29/03/2016     0.0      Ray Banister	     First implementation                                 */  
/* 11/04/2016	  0.1	   Ray Banister      Renamed to GetDictionaryXml as per v10 of spec.      */
/* 11/04/2016	  0.2	   Ray Banister      Dictionary size increased to NVARCHAR(max)           */
/* 12/04/2016     0.3      Matt Jordan       Removed Explicit USE database                        */
/* 06/05/2016     0.4      Matt Jordan       Spurious ; removed before GO command                 */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */

CREATE PROCEDURE [dbo].[sp_eAspire_GetDictionaryXml] 
	@Dictionary NVARCHAR(MAX) OUT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @RESULT NVARCHAR(300) = ''
	
	BEGIN
		SET @Dictionary = '<items><item><key>Service Type</key><value/></item><item><key>Service User Pin</key><value/></item></items>'
	END	
END

GO