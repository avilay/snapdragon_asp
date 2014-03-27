USE [Snapdragon]
GO

DELETE FROM [UserHistory]
DELETE FROM [UserItem]
DELETE FROM [Item]
DELETE FROM [UserFeed]
DELETE FROM [Feed]
--DELETE FROM [Stopword]
DELETE FROM [Probability]
DELETE FROM [UserVocabulary]
DELETE FROM [Vocabulary]
DELETE FROM [Prior]
--DELETE FROM [Classification]
GO

DBCC CHECKIDENT(UserHistory, reseed, 0)
DBCC CHECKIDENT(UserItem, reseed, 0)
DBCC CHECKIDENT(Item, reseed, 0)
DBCC CHECKIDENT(UserFeed, reseed, 0)
DBCC CHECKIDENT(Feed, reseed, 0)
--DBCC CHECKIDENT(Stopword, reseed, 0)
DBCC CHECKIDENT(Probability, reseed, 0)
DBCC CHECKIDENT(UserVocabulary, reseed, 0)
DBCC CHECKIDENT(Vocabulary, reseed, 0)
DBCC CHECKIDENT([Prior], reseed, 0)
--DBCC CHECKIDENT(Classification, reseed, 0)
GO


