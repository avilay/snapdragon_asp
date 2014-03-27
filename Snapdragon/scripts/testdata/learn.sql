USE [Snapdragon]
GO

DBCC CHECKIDENT(Item, reseed, 0)
GO

DBCC CHECKIDENT(UserItem, reseed, 0)
GO

DBCC CHECKIDENT(Feed, reseed, 0)
GO

DBCC CHECKIDENT(UserFeed, reseed, 0)
GO

DBCC CHECKIDENT(Classification, reseed, 0)
GO

INSERT INTO [Classification] (Label) VALUES ('interesting');
INSERT INTO [Classification] (Label) VALUES ('uninteresting');
INSERT INTO [Classification] (Label) VALUES ('unknown'); -- only needed when adding a new Item/UserItem.
GO

-- user1 id: 29b90b21-946f-401b-ac9d-a6ea0dcbbc1f

-- user2 id: c0581088-8c6f-4840-80d5-d5639447d4b3

INSERT INTO [Feed] (ContentUrl, Description, LastChecked, LastPublished, Title, Url) 
VALUES ('http://www.nytimes.com/pages/index.html?partner=rss', '', DATEADD(minute, -5, GETUTCDATE()), DATEADD(day, -2, GETUTCDATE()), 'NYT > Home Page', 'http://www.nytimes.com/services/xml/rss/nyt/HomePage.xml');

INSERT INTO [UserFeed] (UserId, FeedId)
VALUES ('29b90b21-946f-401b-ac9d-a6ea0dcbbc1f', 1);

INSERT INTO [UserFeed] (UserId, FeedId)
VALUES ('c0581088-8c6f-4840-80d5-d5639447d4b3', 1);

GO

-- unread and unclassified items
INSERT INTO [Item] (FeedId, Author, Description, Excerpt, Link, PubDate, Title, InsertedOn) VALUES (
1, 
'noreply@blogger.com (Avilay Parekh)', 
'My parents are visiting me right now. It is a lot of fun to eat undhiyu and badam paak everyday!', 
'', 
'http://avilay.blogspot.com/2008/04/having-fun-with-ma-and-baba.html', 
DATEADD(day, -2, GETUTCDATE()), 
'Having fun with Ma and Baba', 
DATEADD(minute, -5, GETUTCDATE())
);

INSERT INTO [UserItem] (UserId, ItemId, PredictedClassificationId, IsRead, ReadTime, PredictionScore, IsClicked) VALUES (
'29b90b21-946f-401b-ac9d-a6ea0dcbbc1f',
1,
3,
0,
'1/1/1753',
-1,
0);


INSERT INTO [Item] (FeedId, Author, Description, Excerpt, Link, PubDate, Title, InsertedOn) VALUES (
1, 
'noreply@blogger.com (Avilay Parekh)', 
'I just read that in-store pick up is an emerging trend in e-commerce these days. A company called shopatron is utilizing this for their service. <br /><br /><a href=\"http://www.nytimes.com/2007/09/24/technology/24ecom.html?ex=1348286400&amp;en=8ab0536e538a78e3&amp;ei=5088&amp;partner=rssnyt&amp;emc=rss\">http://www.nytimes.com/2007/09/24/technology/24ecom.html?ex=1348286400&amp;en=8ab0536e538a78e3&amp;ei=5088&amp;partner=rssnyt&amp;emc=rss</a>', 
'', 
'http://avilay.blogspot.com/2007/09/new-e-commerce-trend.html', 
DATEADD(day, -2, GETUTCDATE()), 
'New e-commerce trend', 
DATEADD(minute, -5, GETUTCDATE())
);

INSERT INTO [UserItem] (UserId, ItemId, PredictedClassificationId, IsRead, ReadTime, PredictionScore, IsClicked) VALUES (
'29b90b21-946f-401b-ac9d-a6ea0dcbbc1f',
2,
3,
0,
'1/1/1753',
-1,
0);

GO

-- unread but potentailly interesting items
INSERT INTO [Item] (FeedId, Author, Description, Excerpt, Link, PubDate, Title, InsertedOn) VALUES (
1, 
'Avilay Parekh', 
'', 
'',
'http://fabrikam.com/like2', 
DATEADD(day, -15, GETUTCDATE()),  
'yellow yellow red', 
DATEADD(day, -14, GETUTCDATE())
);

INSERT INTO [UserItem] (UserId, ItemId, PredictedClassificationId, IsRead, ReadTime, PredictionScore, IsClicked) VALUES (
'29b90b21-946f-401b-ac9d-a6ea0dcbbc1f',
3,
1,
0,
'1/1/1753',
0.9,
0);


INSERT INTO [Item] (FeedId, Author, Description, Excerpt, Link, PubDate, Title, InsertedOn) VALUES (
1, 
'Avilay Parekh', 
'', 
'',
'http://fabrikam.com/like3', 
DATEADD(day, -17, GETUTCDATE()),  
'yellow yellow blue blue', 
DATEADD(day, -15, GETUTCDATE())
);

INSERT INTO [UserItem] (UserId, ItemId, PredictedClassificationId, IsRead, ReadTime, PredictionScore, IsClicked) VALUES (
'29b90b21-946f-401b-ac9d-a6ea0dcbbc1f',
4,
1,
0,
'1/1/1753',
0.6,
0);

GO

-- unread but potentially uninteresting items
INSERT INTO [Item] (FeedId, Author, Description, Excerpt, Link, PubDate, Title, InsertedOn) VALUES (
1, 
'Avilay Parekh', 
'', 
'',
'http://fabrikam.com/dislike1', 
DATEADD(day, -2, GETUTCDATE()),  
'yellow yellow blue blue', 
DATEADD(day, -1, GETUTCDATE())
);

INSERT INTO [UserItem] (UserId, ItemId, PredictedClassificationId, IsRead, ReadTime, PredictionScore, IsClicked) VALUES (
'29b90b21-946f-401b-ac9d-a6ea0dcbbc1f',
5,
2,
0,
'1/1/1753',
0.8,
0);

GO

-- clicked items
INSERT INTO [Item] (FeedId, Author, Description, Excerpt, Link, PubDate, Title, InsertedOn) VALUES (
1, 
'noreply@blogger.com (Avilay Parekh)', 
'My objective is to use iPod as a player as well as a storage for all my songs so that I dont have to store songs on my computer. So far it does not seem possible to use it as both - a storage device and a player. I can choose one.<br /><br />', 
'',
'http://avilay.blogspot.com/2007/07/ipod-exploration.html', 
DATEADD(day, -7, GETUTCDATE()),  
'iPod exploration', 
DATEADD(day, -5, GETUTCDATE())
);

INSERT INTO [UserItem] (UserId, ItemId, PredictedClassificationId, IsRead, ReadTime, PredictionScore, IsClicked) VALUES (
'29b90b21-946f-401b-ac9d-a6ea0dcbbc1f',
6,
1,
1,
DATEADD(day, -4, GETUTCDATE()),
0.9,
1);


INSERT INTO [Item] (FeedId, Author, Description, Excerpt, Link, PubDate, Title, InsertedOn) VALUES (
1, 
'Avilay Parekh', 
'', 
'',
'http://fabrikam.com/like1', 
DATEADD(day, -10, GETUTCDATE()),  
'yellow yellow yellow yellow red', 
DATEADD(day, -9, GETUTCDATE())
);

INSERT INTO [UserItem] (UserId, ItemId, PredictedClassificationId, IsRead, ReadTime, PredictionScore, IsClicked) VALUES (
'29b90b21-946f-401b-ac9d-a6ea0dcbbc1f',
7,
2,
1,
DATEADD(day, -8, GETUTCDATE()),
0.8,
1);

GO
-- unclicked read items
INSERT INTO [Item] (FeedId, Author, Description, Excerpt, Link, PubDate, Title, InsertedOn) VALUES (
1, 
'Avilay Parekh', 
'', 
'',
'http://fabrikam.com/like2', 
DATEADD(day, -7, GETUTCDATE()),  
'yellow yellow red', 
DATEADD(day, -5, GETUTCDATE())
);

INSERT INTO [UserItem] (UserId, ItemId, PredictedClassificationId, IsRead, ReadTime, PredictionScore, IsClicked) VALUES (
'29b90b21-946f-401b-ac9d-a6ea0dcbbc1f',
8,
1,
1,
DATEADD(day, -4, GETUTCDATE()),
0.7,
0);


INSERT INTO [Item] (FeedId, Author, Description, Excerpt, Link, PubDate, Title, InsertedOn) VALUES (
1, 
'Avilay Parekh', 
'', 
'',
'http://fabrikam.com/like3', 
DATEADD(day, -10, GETUTCDATE()),  
'yellow yellow blue blue', 
DATEADD(day, -9, GETUTCDATE())
);

INSERT INTO [UserItem] (UserId, ItemId, PredictedClassificationId, IsRead, ReadTime, PredictionScore, IsClicked) VALUES (
'29b90b21-946f-401b-ac9d-a6ea0dcbbc1f',
9,
2,
1,
DATEADD(day, -8, GETUTCDATE()),
0.8,
0);

GO




DBCC CHECKIDENT(Classification, reseed, 0)
GO

DBCC CHECKIDENT([Prior], reseed, 0)
GO

DBCC CHECKIDENT(Vocabulary, reseed, 0)
GO

DBCC CHECKIDENT(UserVocabulary, reseed, 0)
GO

DBCC CHECKIDENT(Probability, reseed, 0)
GO

DBCC CHECKIDENT(Stopword, reseed, 0)
GO

DBCC CHECKIDENT(UserHistory, reseed, 0)
GO

INSERT INTO [Classification] (Label) VALUES ('interesting');
INSERT INTO [Classification] (Label) VALUES ('uninteresting');
GO

-- INSERT INTO [] () VALUES ();

INSERT INTO [Stopword] (Word) VALUES ('of');
INSERT INTO [Stopword] (Word) VALUES ('the');
GO

INSERT INTO [Vocabulary] (Word) VALUES ('yellow');
INSERT INTO [Vocabulary] (Word) VALUES ('blue');
INSERT INTO [Vocabulary] (Word) VALUES ('red');
INSERT INTO [Vocabulary] (Word) VALUES ('green');
GO

INSERT INTO [UserHistory] (UserId, HasModel) VALUES ('29b90b21-946f-401b-ac9d-a6ea0dcbbc1f', 1);
INSERT INTO [UserHistory] (UserId, HasModel) VALUES ('c0581088-8c6f-4840-80d5-d5639447d4b3', 1);
GO

INSERT INTO [UserVocabulary] (UserId, VocabularyId) VALUES ('29b90b21-946f-401b-ac9d-a6ea0dcbbc1f', 1);
INSERT INTO [UserVocabulary] (UserId, VocabularyId) VALUES ('29b90b21-946f-401b-ac9d-a6ea0dcbbc1f', 2);
INSERT INTO [UserVocabulary] (UserId, VocabularyId) VALUES ('29b90b21-946f-401b-ac9d-a6ea0dcbbc1f', 3);
INSERT INTO [UserVocabulary] (UserId, VocabularyId) VALUES ('c0581088-8c6f-4840-80d5-d5639447d4b3', 3);
INSERT INTO [UserVocabulary] (UserId, VocabularyId) VALUES ('c0581088-8c6f-4840-80d5-d5639447d4b3', 4);
GO

INSERT INTO [Prior] (ClassificationId, UserId, Probability) VALUES (1, '29b90b21-946f-401b-ac9d-a6ea0dcbbc1f', 0.5);
INSERT INTO [Prior] (ClassificationId, UserId, Probability) VALUES (2, '29b90b21-946f-401b-ac9d-a6ea0dcbbc1f', 0.5);
INSERT INTO [Prior] (ClassificationId, UserId, Probability) VALUES (1, 'c0581088-8c6f-4840-80d5-d5639447d4b3', 0.5);
INSERT INTO [Prior] (ClassificationId, UserId, Probability) VALUES (2, 'c0581088-8c6f-4840-80d5-d5639447d4b3', 0.5);
GO

--                +------+---------+
--                | LIKE | DISLIKE |
--                +------+---------+
--         yellow | 0.6  |  0.35   |
--                +------+---------+
--         blue   | 0.2  |  0.6    |
--                +------+---------+
--         red    | 0.2  |  0.05   |
--                +------+---------+
INSERT INTO [Probability] (UserVocabularyId, ClassificationId, Estimate) VALUES (1, 1, 0.6);
INSERT INTO [Probability] (UserVocabularyId, ClassificationId, Estimate) VALUES (2, 1, 0.2);
INSERT INTO [Probability] (UserVocabularyId, ClassificationId, Estimate) VALUES (3, 1, 0.2);
INSERT INTO [Probability] (UserVocabularyId, ClassificationId, Estimate) VALUES (1, 2, 0.35);
INSERT INTO [Probability] (UserVocabularyId, ClassificationId, Estimate) VALUES (2, 2, 0.6);
INSERT INTO [Probability] (UserVocabularyId, ClassificationId, Estimate) VALUES (3, 2, 0.05);

INSERT INTO [Probability] (UserVocabularyId, ClassificationId, Estimate) VALUES (4, 1, 0.05);
INSERT INTO [Probability] (UserVocabularyId, ClassificationId, Estimate) VALUES (5, 1, 0.05);
INSERT INTO [Probability] (UserVocabularyId, ClassificationId, Estimate) VALUES (4, 2, 0.05);
INSERT INTO [Probability] (UserVocabularyId, ClassificationId, Estimate) VALUES (5, 2, 0.05);
GO
