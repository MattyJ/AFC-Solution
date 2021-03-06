/* ============================================================================================== */
/* Name:        Create_AFC_Error_Messages                                                         */
/* Author:		Ray Banister                                                                      */
/* Create date: 14/03/2016                                                                        */
/*                                                                                                */
/* Description:	Adds the user defined error messages to the messages table. These are called by   */
/*              other Stored Procedures to pass errors to the caller.                             */
/*		                                                                                          */
/* Mandatory Input Parameters: NONE                                                               */
/* Optional Input Parameters:  NONE                                                               */
/* Returns:                    NONE                                                               */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/* Copyright 2016 Fujitsu Services Ltd                                                            */
/* ============================================================================================== */
/*                                        Change Record                                           */
/* ============================================================================================== */
/*    Date      Version     Changed By                     Comments                               */
/* ============================================================================================== */
/* 14/03/2016     0.0      Ray Banister	     First implementation                                 */  
/* 22/03/2016     0.1	   Ray Banister      Added code 60000 for ERR_INVALID_PARAMETERS. No      */
/*                                           longer a stored procedure.                           */
/* 23/03/2016     0.2      Ray Banister      Renamed and put 'remove' in a separate file          */
/* 11/04/2016	  0.3	   Ray Banister      Error code 60014 added                               */
/* 14/04/2016	  0.4	   Ray Banister      USE statement removed                                */
/* 21/04/2016	  0.5	   Ray Banister      Error code 60015 added                               */
/* 05/05/2016	  0.6	   Ray Banister      Error code 60016 added                               */
/* 12/05/2016	  0.7	   Ray Banister      Error code 60017 added                               */
/* 18/05/2016	  0.8	   Ray Banister      Error code 60017 corrected                           */
/* 18/08/2016	  0.9	   Ray Banister      Error code 60018 added                               */
/*                                                                                                */
/* ============================================================================================== */

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		
    -- Add messages
    exec sp_addmessage @msgnum = 60000,@severity = 16, @msgtext = 'ERR_INVALID_PARAMETERS'
    exec sp_addmessage @msgnum = 60001,@severity = 16, @msgtext = 'ERR_NO_SITES'
    exec sp_addmessage @msgnum = 60002,@severity = 16, @msgtext = 'ERR_PIN_INUSE'
    exec sp_addmessage @msgnum = 60003,@severity = 16, @msgtext = 'ERR_CASE_INUSE'
    exec sp_addmessage @msgnum = 60004,@severity = 16, @msgtext = 'ERR_NO_PIN_DCF'
    exec sp_addmessage @msgnum = 60005,@severity = 16, @msgtext = 'ERR_PIN_ALREADY_REQUESTED'
    
    exec sp_addmessage @msgnum = 60006,@severity = 16, @msgtext = 'ERR_NO_CASE_DCF'
    exec sp_addmessage @msgnum = 60007,@severity = 16, @msgtext = 'ERR_DICT_INVAL'
    exec sp_addmessage @msgnum = 60008,@severity = 16, @msgtext = 'ERR_SEC_UPDATE'
    exec sp_addmessage @msgnum = 60009,@severity = 16, @msgtext = 'ERR_SP_CONNECT'
    exec sp_addmessage @msgnum = 60010,@severity = 16, @msgtext = 'ERR_PROJ_NOT_ASSIGNED_TO_CASE'
    
    exec sp_addmessage @msgnum = 60011,@severity = 16, @msgtext = 'ERR_NO_SPACE'
    exec sp_addmessage @msgnum = 60012,@severity = 16, @msgtext = 'ERR_PIN_BEING_MERGED'
    exec sp_addmessage @msgnum = 60013,@severity = 16, @msgtext = 'ERR_PIN_BEING_DELETED'
    exec sp_addmessage @msgnum = 60014,@severity = 16, @msgtext = 'ERR_TOPIN_EQUALS_FROMPIN'
	exec sp_addmessage @msgnum = 60015,@severity = 16, @msgtext = 'ERR_CASEID_ALREADY_REQUESTED'
	
	exec sp_addmessage @msgnum = 60016,@severity = 16, @msgtext = 'ERR_NEW_PROJECT_EQUALS_CURRENT_PROJECT'
	exec sp_addmessage @msgnum = 60017,@severity = 16, @msgtext = 'ERR_PROJECTID_INUSE_BY_PIN'
	exec sp_addmessage @msgnum = 60018,@severity = 16, @msgtext = 'ERR_PIN_PENDING_DELETION'

     
END

GO
