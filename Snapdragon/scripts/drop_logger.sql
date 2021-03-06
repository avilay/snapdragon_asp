USE [Snapdragon]
GO

-- drop procedures
IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'AddCategory')
               AND (type = 'P')))
DROP PROCEDURE [dbo].AddCategory
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'ClearLogs')
               AND (type = 'P')))
DROP PROCEDURE [dbo].ClearLogs
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'InsertCategoryLog')
               AND (type = 'P')))
DROP PROCEDURE [dbo].InsertCategoryLog
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'WriteLog')
               AND (type = 'P')))
DROP PROCEDURE [dbo].WriteLog
GO

-- drop tables
IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'CategoryLog')
               AND (type = 'U')))
DROP TABLE [dbo].CategoryLog
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'Log')
               AND (type = 'U')))
DROP TABLE [dbo].Log
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'Category')
               AND (type = 'U')))
DROP TABLE [dbo].Category
GO


