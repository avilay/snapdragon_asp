USE [Snapdragon]
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'sp_clean_snapdragonlogs')
               AND (type = 'P')))
DROP PROCEDURE [dbo].sp_clean_snapdragonlogs
GO

CREATE PROCEDURE [dbo].sp_clean_snapdragonlogs
    @daysToKeep  int
AS
BEGIN
	DECLARE @history int
	SELECT @history = @daysToKeep*-1
    
    DECLARE @logId int
    SELECT @logId = MAX(LogId)
    FROM [Log]
    WHERE ([Log].Timestamp <= DATEADD(day, @history, GETUTCDATE()))
    
    DELETE FROM [CategoryLog]
    WHERE LogId <= @logId
    
    DELETE FROM [Log]
    WHERE LogId <= @logId
END
GO
