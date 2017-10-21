/* ============================================================================================== */
/* Name:        Drop_AFC_Error_Messages                                                           */
/* Author:		Ray Banister                                                                      */
/* Create date: 23/03/2016                                                                        */
/*                                                                                                */
/* Description:	Removes the user defined error messages from the messages table.                  */
/*              Messages start at 60000                                                           */
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
/* 23/03/2016     0.0      Ray Banister	     First implementation                                 */  
/* 11/04/2016	  0.1	   Ray Banister      Error code 60014 added                               */
/* 21/04/2016	  0.2	   Ray Banister      Remove range changed to 60000 - 60099                */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/*                                                                                                */
/* ============================================================================================== */

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @ERR_NUMBER int
	
	SET @ERR_NUMBER = 60000
	WHILE (@ERR_NUMBER <= 60099)
		BEGIN
			IF EXISTS (SELECT * FROM sys.messages WHERE message_id = @ERR_NUMBER)
			BEGIN
				exec sp_dropmessage @msgnum = @ERR_NUMBER
			END
			SET @ERR_NUMBER = @ERR_NUMBER +1
		END
END
GO