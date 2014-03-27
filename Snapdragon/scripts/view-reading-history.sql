use snapdragon

select I.Id as ItemId, I.InsertedOn, I.Title, F.Title as Feed, F.Id as FeedId, UI.IsRead, UI.IsClicked, C.Label, UI.PredictionScore
from [UserItem] as UI
inner join [Item] as I on UI.ItemId = I.Id
inner join [Feed] as F on I.FeedId = F.Id
inner join [aspnet_Users] as U on UI.UserId = U.UserId
inner join [Classification] as C on C.Id = UI.PredictedClassificationId
where U.UserName = 'aptg'
order by F.Id

