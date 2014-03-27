USE [Snapdragon]
GO

SELECT U.UserName, C.Label, P.Probability
FROM [Prior] AS P
INNER JOIN [Classification] AS C ON P.ClassificationId = C.Id
INNER JOIN [aspnet_Users] AS U ON P.UserId = U.UserId

SELECT U.UserName, V.Word, C.Label, PR.Estimate
FROM [Probability] AS PR
INNER JOIN [UserVocabulary] AS UV ON PR.UserVocabularyId = UV.Id
INNER JOIN [Classification] AS C ON PR.ClassificationId = C.Id
INNER JOIN [Vocabulary] AS V ON UV.VocabularyId = V.Id
INNER JOIN [aspnet_Users] AS U ON UV.UserId = U.UserId

