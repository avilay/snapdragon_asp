USE [Snapdragon]
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'sp_clean_snapdragon')
               AND (type = 'P')))
DROP PROCEDURE [dbo].sp_clean_snapdragon
GO

CREATE PROCEDURE [dbo].sp_clean_snapdragon
    @daysToKeep  int
AS
BEGIN
	DECLARE @history int
	SELECT @history = @daysToKeep*-1
    
    DECLARE @itemId int
    SELECT @itemId = MAX(Id)
    FROM [Item]
    WHERE (InsertedOn <= DATEADD(day, @history, GETUTCDATE()))
    
    DELETE FROM [UserItem]
	WHERE ItemId <= @itemId     
    
    DELETE FROM [Item] 
    WHERE Id <= @itemId
           
END
GO

