USE [Snapdragon]
GO

SELECT [Severity]
      ,[Timestamp]
      ,[Message]
      ,[FormattedMessage]
  FROM [dbo].[Log]
  WHERE [Timestamp] > DATEADD(day, -3, GETUTCDATE())
  ORDER BY [Timestamp] DESC
  
  GO 