/* ============================================================================================== */
/* Name:        sp_ValidateCaseIdInUse                                                            */
/* Author:		Ray Banister                                                                      */
/* Create date: 23/03/2016                                                                        */
/*                                                                                                */
/* Description:	Checks if the CaseId has already been used.                                       */
/*                                                                                                */
/*		                                                                                          */
/* Mandatory Input Parameters: @CaseId                                                            */
/* Optional Input Parameters:  None                                                               */
/* Returns: @CaseIdInUse as an OUT parameter                                                      */
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
/* 21/04/2016     0.2      Ray Banister      Correct functionality now added                      */
/* 06/05/2016     0.3      Matt Jordan       Removed Explicit USE database                        */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */

CREATE PROCEDURE [dbo].[sp_ValidateCaseIdInUse] 
	@CaseId INT,
	@CaseIdInUse BIT OUT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	
	IF EXISTS(SELECT  1 FROM dbo.Library WHERE CaseId = @CaseId)
		SET @CaseIdInUse = 'True'
	ELSE SET @CaseIdInUse = 'False'

END
	


GO

