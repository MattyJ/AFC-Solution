/* ============================================================================================== */
/* Name:        vw_eAspireCase                                                                    */
/* Author:		Jonathan King                                                                     */
/* Create date: 03/05/2016                                                                        */
/*                                                                                                */
/* Description:	Retrieve the details about created and pending PIN creations    				  */
/*                                                                                                */
/*		                                                                                          */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 03/05/2016     0.0      Jonathan King         First implementation                             */
/* 05/05/2016	  0.1	   Ray Banister          Method name corrected                            */
/* ============================================================================================== */


CREATE VIEW [dbo].[vw_eAspirePIN]
AS
	SELECT PIN AS [PIN], 2 AS [Status]
	FROM dbo.Site  WITH (NOLOCK) 
		UNION
		-- Combine Results with Pending 'AllocatePin' requests
		SELECT PIN AS [PIN], 1 AS [Status] 
		FROM dbo.Task  WITH (NOLOCK) 
		WHERE (Handler = 'OperationsHandler' AND Name = 'AllocatePin')

GO

