/* ============================================================================================== */
/* Name:        sp_ValidatePIN_ProjectId_Exist                                                    */
/* Author:		Ray Banister                                                                      */
/* Create date: 12/05/2016                                                                        */
/*                                                                                                */
/* Description:	Checks if the selected PIN/ProjectId combination exists in either the Library     */ 
/*              table or in any pending Allocate case requests.                                   */
/*                                                                                                */
/*		                                                                                          */
/* Mandatory Input Parameters: @PIN, @ProjectId                                                   */
/* Optional Input Parameters:  None                                                               */
/* Returns: @PINProjectIdExists as an OUT parameter                                               */
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
/* 12/05/2016     0.0      Ray Banister       Removed Explicit USE database                        */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */
CREATE PROCEDURE [dbo].[sp_ValidatePIN_ProjectId_Exists] 
	@PIN INT,
	@ProjectId INT,
	@PINProjectIdExists BIT OUT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SET @PINProjectIdExists = 'False'
	-- Check if the PIN/ProjectId exists in the Libray Table
	IF EXISTS (SELECT ProjectId
			FROM dbo.Library 
			WHERE SiteId = (SELECT ID from Site where Pin = @PIN) AND ProjectId = @Projectid)
		BEGIN
			SET @PINProjectIdExists = 'True'
		END	
	ELSE
		BEGIN
		-- Check if the PIN/ProjectId exists in the Task table for an Allocate Case
		IF EXISTS(SELECT  1 FROM dbo.Task
				WHERE (Handler = 'OperationsHandler' AND 
					Name = 'AllocateCase' AND 
					PIN = @PIN AND
					ProjectId = @ProjectId AND
					InsertedDate < GETDATE()))  
				SET @PINProjectIdExists = 'True'
		END

END
	
GO

