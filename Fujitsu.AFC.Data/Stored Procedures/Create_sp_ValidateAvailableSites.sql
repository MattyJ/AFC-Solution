/* ============================================================================================== */
/* Name:        sp_ValidateAvailableSites                                                     */
/* Author:		Ray Banister                                                                      */
/* Create date: 23/03/2016                                                                        */
/*                                                                                                */
/* Description:	Retrieves the number of sites which have been provisioned but not yet allocated   */
/*                                                                                                */
/*		                                                                                          */
/* Mandatory Input Parameters: None                                                               */
/* Optional Input Parameters:  None                                                               */
/* Returns: @AvailableSiteCount as an OUT parameter                                               */
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
/* 19/04/2016     0.2      Matt Jordan       Fixed bug the code was returning the number          */
/*                                           of allocated sites and not unallocated sites.        */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */

CREATE PROCEDURE [dbo].[sp_ValidateAvailableSites] 
	@AvailableSiteCount AS INTEGER OUT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SET @AvailableSiteCount = (SELECT COUNT(IsAllocated) 
		FROM dbo.ProvisionedSite
		WHERE IsAllocated = 0)
END
	

GO


