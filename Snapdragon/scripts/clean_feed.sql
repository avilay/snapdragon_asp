USE [Snapdragon]
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'sp_clean_feed')
               AND (type = 'P')))
DROP PROCEDURE [dbo].sp_clean_feed
GO

CREATE PROCEDURE [dbo].sp_clean_feed
    @feedId int    
AS
BEGIN
	DELETE FROM [UserItem]
	WHERE ItemId IN
	(SELECT Id from [Item]
	 WHERE FeedId = @feedId)
	
	DELETE FROM [Item]
	WHERE FeedId = @feedId
	
	DELETE FROM [UserFeed]
	WHERE FeedId = @feedId
	
	DELETE FROM [Feed]
	WHERE Id = @feedId
	    
END
GO