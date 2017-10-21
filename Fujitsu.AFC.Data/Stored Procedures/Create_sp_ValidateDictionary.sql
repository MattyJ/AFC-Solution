/* ============================================================================================== */
/* Name:        sp_ValidateDictionary                                                             */
/* Author:		Ray Banister                                                                      */
/* Create date: 23/03/2016                                                                        */
/*                                                                                                */
/* Description:	Checks the Dictionary string for well formed xml.                                 */
/*                                                                                                */
/*		                                                                                          */
/* Mandatory Input Parameters: @Dictionary                                                        */
/* Optional Input Parameters:  None                                                               */
/* Returns: @IsValid as an OUT parameter                                                          */
/* Errors Raised: None                                                                            */
/*                                                                                                */
/*               NOTE: XML VALIDATION NEEDS IMPROVEMENT                                           */
/*                                                                                                */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 23/03/2016     0.0      Ray Banister	     First implementation                                 */  
/* 11/04/2016	  0.1	   Ray Banister      Dictionary size now NVARCHAR(MAX)                    */
/* 12/04/2016     0.2      Matt Jordan       Removed Explicit USE database                        */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */

CREATE PROCEDURE [dbo].[sp_ValidateDictionary] 
	@Dictionary NVARCHAR(MAX),
	@IsValid BIT OUT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @XML AS XML
	
	-- Check Dictionary XML is valid. 
	-- *********************************************************
	--  This currently isn't good enough  !!! But it's a starter
	-- *********************************************************
	BEGIN TRY
		SELECT @XML = CAST(REPLACE(REPLACE(REPLACE(@Dictionary,CHAR(10) + CHAR(13), ''),CHAR(10), ''), CHAR(13), '') AS XML)
		SET @IsValid = 1
	END TRY
	BEGIN CATCH
		SET @IsValid = 0
	END CATCH


END
	

GO


