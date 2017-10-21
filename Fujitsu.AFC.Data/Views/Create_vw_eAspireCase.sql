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
/*                                                                                                */
/* ============================================================================================== */


CREATE VIEW [dbo].[vw_eAspireCase]
AS
	SELECT PIN as [PIN], CaseId as [CaseID], 2 as [Status]
	FROM dbo.Site WITH (NOLOCK) INNER JOIN 
		dbo.Library WITH (NOLOCK) ON dbo.Site.Id = dbo.Library.SiteId
		UNION
			-- Combine Results with Pending 'AllocateCase' requests
			SELECT PIN as [PIN], CaseId as [CaseID], 1 as [Status] 
			FROM dbo.Task WITH (NOLOCK)
			WHERE (Handler = 'OperationsHandler' AND Name = 'AllocateCase')

GO

