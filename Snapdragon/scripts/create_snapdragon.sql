USE [Snapdragon]
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

-- ** Text mining tables
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Classification]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
    CREATE TABLE [dbo].[Classification](
        [Id] [int] IDENTITY(1,1) NOT NULL,
	    [Label] [varchar](50) NOT NULL,
        CONSTRAINT [PK_Classfiication] PRIMARY KEY CLUSTERED (
            [Id] ASC
        ) ON [PRIMARY]
    ) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Vocabulary]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
    CREATE TABLE [dbo].[Vocabulary](
        [Id] [int] IDENTITY(1,1) NOT NULL,
		[Word] [varchar](50) NOT NULL,
        CONSTRAINT [PK_Vocabulary] PRIMARY KEY CLUSTERED (
            [Id] ASC
        ) ON [PRIMARY]
    ) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Stopword]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
    CREATE TABLE [dbo].[Stopword](
        [Id] [int] IDENTITY(1,1) NOT NULL,
		[Word] [varchar](50) NOT NULL,
        CONSTRAINT [PK_Stopword] PRIMARY KEY CLUSTERED (
            [Id] ASC
        ) ON [PRIMARY]
    ) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Prior]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
    CREATE TABLE [dbo].[Prior](
        [Id] [int] IDENTITY(1,1) NOT NULL,
		[ClassificationId] [int] NOT NULL CONSTRAINT [FK_Prior_Classification] FOREIGN KEY REFERENCES [dbo].[Classification] ([Id]),
		[UserId] [uniqueidentifier] NOT NULL CONSTRAINT [FK_Prior_User] FOREIGN KEY REFERENCES [dbo].[aspnet_Users] ([UserId]),
		[Probability] [float] NOT NULL,
        CONSTRAINT [PK_Prior] PRIMARY KEY CLUSTERED (
            [Id] ASC
        ) ON [PRIMARY]
    ) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[UserVocabulary]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
    CREATE TABLE [dbo].[UserVocabulary](
        [Id] [int] IDENTITY(1,1) NOT NULL,
		[UserId] [uniqueidentifier] NOT NULL CONSTRAINT [FK_UserVocabulary_User] FOREIGN KEY REFERENCES [dbo].[aspnet_Users] ([UserId]),
		[VocabularyId] [int] NOT NULL CONSTRAINT [FK_UserVocabulary_Vocabulary] FOREIGN KEY REFERENCES [dbo].[Vocabulary] ([Id]),
        CONSTRAINT [PK_UserVocabulary] PRIMARY KEY CLUSTERED (
            [Id] ASC
        ) ON [PRIMARY]
    ) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Probability]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
    CREATE TABLE [dbo].[Probability](
        [Id] [int] IDENTITY(1,1) NOT NULL,
		[UserVocabularyId] [int] NOT NULL CONSTRAINT [FK_Probability_UserVocabulary] FOREIGN KEY REFERENCES [dbo].	[UserVocabulary] ([Id]),
		[ClassificationId] [int] NOT NULL CONSTRAINT [FK_Probability_Classification] FOREIGN KEY REFERENCES [dbo].[Classification] ([Id]),
		[Estimate] [float] NOT NULL,
        CONSTRAINT [PK_Probability] PRIMARY KEY CLUSTERED (
            [Id] ASC
        ) ON [PRIMARY]
    ) ON [PRIMARY]
END
GO

-- *** Feed tables
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Feed]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
    CREATE TABLE [dbo].[Feed] (
        [Id] [int] IDENTITY(1,1) NOT NULL,
		[ContentUrl] [varchar](256) NOT NULL,
		[Description] [varchar](1024) NOT NULL,
		[LastChecked] [datetime] NOT NULL,
		[LastPublished] [datetime] NOT NULL,
		[Title] [varchar] (1024) NOT NULL,
		[Url] [varchar](256) NOT NULL,
		CONSTRAINT [PK_Feed] PRIMARY KEY CLUSTERED (
			[Id] ASC
		) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[UserFeed]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
    CREATE TABLE [dbo].[UserFeed](
        [Id] [int] IDENTITY(1,1) NOT NULL,
	    [UserId] [uniqueidentifier] NOT NULL CONSTRAINT [FK_UserFeed_User] FOREIGN KEY REFERENCES [dbo].[aspnet_Users] ([UserId]),
	    [FeedId] [int] NOT NULL CONSTRAINT [FK_UserFeed_Feed] FOREIGN KEY REFERENCES [dbo].[Feed] ([Id]),
        CONSTRAINT [PK_UserFeed] PRIMARY KEY CLUSTERED (
            [Id] ASC
        ) ON [PRIMARY]
    ) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Item]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
    CREATE TABLE [dbo].[Item](
        [Id] [int] IDENTITY(1,1) NOT NULL,
		[FeedId] [int] NOT NULL CONSTRAINT [FK_Item_Feed] FOREIGN KEY REFERENCES [dbo].[Feed] ([Id]),
		[Author] [nvarchar](256) NOT NULL,
		[Description] [text] NOT NULL,
		[Excerpt] [text] NOT NULL,
		[Link] [varchar](1024) NOT NULL,
		[PubDate] [datetime] NOT NULL,
		[Title] [nvarchar](1024) NOT NULL,
		[InsertedOn] [datetime] NOT NULL,
		CONSTRAINT [PK_Item] PRIMARY KEY CLUSTERED (
            [Id] ASC
        ) ON [PRIMARY]
    ) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[UserItem]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
    CREATE TABLE [dbo].[UserItem](
        [Id] [int] IDENTITY(1,1) NOT NULL,
		[UserId] [uniqueidentifier] NOT NULL CONSTRAINT [FK_UserItem_User] FOREIGN KEY REFERENCES [dbo].[aspnet_Users] ([UserId]),
		[ItemId] [int] NOT NULL CONSTRAINT [FK_UserItem_Item] FOREIGN KEY REFERENCES [dbo].[Item] ([Id]),
		[PredictedClassificationId] [int] CONSTRAINT [FK_UserItem_Classification_Predicted] FOREIGN KEY REFERENCES [dbo].[Classification] ([Id]),
		[IsRead] [bit] NOT NULL,
		[ReadTime] [datetime] NOT NULL,
		[PredictionScore] [float] NOT NULL,
		[IsClicked] [bit] NOT NULL,
		CONSTRAINT [PK_UserItem] PRIMARY KEY CLUSTERED (
			[Id] ASC
		) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[UserHistory]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
    CREATE TABLE [dbo].[UserHistory](
        [Id] [int] IDENTITY(1,1) NOT NULL,
		[UserId] [uniqueidentifier] NOT NULL CONSTRAINT [FK_UserHistory_User] FOREIGN KEY REFERENCES [dbo].[aspnet_Users] ([UserId]),
		[HasModel] [bit] NOT NULL,		
		CONSTRAINT [PK_UserHistory] PRIMARY KEY CLUSTERED (
			[Id] ASC
		) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

