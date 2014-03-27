USE [Snapdragon]
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'UserItem')
               AND (type = 'U')))
DROP TABLE [dbo].UserItem
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'Item')
               AND (type = 'U')))
DROP TABLE [dbo].Item
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'UserFeed')
               AND (type = 'U')))
DROP TABLE [dbo].UserFeed
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'Feed')
               AND (type = 'U')))
DROP TABLE [dbo].Feed
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'Stopword')
               AND (type = 'U')))
DROP TABLE [dbo].Stopword
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'Probability')
               AND (type = 'U')))
DROP TABLE [dbo].Probability
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'UserVocabulary')
               AND (type = 'U')))
DROP TABLE [dbo].UserVocabulary
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'Vocabulary')
               AND (type = 'U')))
DROP TABLE [dbo].Vocabulary
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'Prior')
               AND (type = 'U')))
DROP TABLE [dbo].Prior
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'Classification')
               AND (type = 'U')))
DROP TABLE [dbo].Classification
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'UserHistory')
               AND (type = 'U')))
DROP TABLE [dbo].UserHistory
GO



