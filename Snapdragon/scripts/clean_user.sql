USE [Snapdragon]
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'sp_clean_user')
               AND (type = 'P')))
DROP PROCEDURE [dbo].sp_clean_user
GO

CREATE PROCEDURE [dbo].sp_clean_user
    @userId  uniqueidentifier    
AS
BEGIN
	DELETE FROM [UserItem]
	WHERE UserId = @userId
	
	DELETE FROM [UserFeed]
	WHERE UserId = @userId
	
	DELETE FROM [Probability]
	WHERE UserVocabularyId IN
	(SELECT Id FROM [UserVocabulary]
	 WHERE UserId = @userId)
	 
	DELETE FROM [UserVocabulary]
	WHERE UserId = @userId
	
	DELETE FROM [Prior]
	WHERE UserId = @userId
	
	DELETE FROM [UserHistory]
	WHERE UserId = @userId
	
	DELETE FROM [aspnet_Membership]
	WHERE UserId = @userId
	
	DELETE FROM [aspnet_Users]
	WHERE UserId = @userId
	    
END
GO