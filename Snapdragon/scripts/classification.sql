USE [Snapdragon]
GO

DELETE FROM [Classification]
GO

DBCC CHECKIDENT(Classification, reseed, 0)
GO

INSERT INTO [Classification] (Label) VALUES('interesting')
INSERT INTO [Classification] (Label) VALUES('uninteresting')
INSERT INTO [Classification] (Label) VALUES('unknown')
GO
