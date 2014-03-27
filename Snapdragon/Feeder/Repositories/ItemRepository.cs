using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Feeder.Models;
using System.Web.Security;
using Avilay.Utils.Logging;

namespace Feeder.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private SnapdragonDataContext _dataContext;
        //private Guid _userId;
        private int _interestingId;
        private int _uninterestingId;
        private int _unknownId;

        public ItemRepository() {
            _dataContext = new SnapdragonDataContext();
            Initialize();            
        }

        //public ItemRepository(SnapdragonDataContext dataContext, Guid userId) {
        //    _dataContext = dataContext;
        //    _userId = userId;
        //}

        public ItemRepository(SnapdragonDataContext dataContext) {
            _dataContext = dataContext;
            Initialize();
        }

        private void Initialize() {
            var interestingId = from c in _dataContext.Classifications
                                where c.Label == "interesting"
                                select c.Id;
            _interestingId = interestingId.First();

            var uninterestingId = from c in _dataContext.Classifications
                                  where c.Label == "uninteresting"
                                  select c.Id;
            _uninterestingId = uninterestingId.First();

            var unknownId = from c in _dataContext.Classifications
                            where c.Label == "unknown"
                            select c.Id;
            _unknownId = unknownId.First();
        }
        
        public void Add(Item itemToAdd, int feedId) {
            itemToAdd.PubDate = itemToAdd.PubDate == DateTime.MinValue ? SqlHelper.GetSqlMinDateTime() : itemToAdd.PubDate;
            itemToAdd.InsertedOn = new DateTime(DateTime.Now.ToUniversalTime().Ticks, DateTimeKind.Utc);
            itemToAdd.FeedId = feedId;
            _dataContext.Items.InsertOnSubmit(itemToAdd);
            _dataContext.SubmitChanges();
            
            //get all users who subscribe to this feed and add UserItem entries for those users
            var userIds = from uf in _dataContext.UserFeeds
                           where uf.FeedId == feedId
                           select uf.UserId;
            foreach (Guid userId in userIds) {
                UserItem uitem = new UserItem {
                    UserId = userId,
                    ItemId = itemToAdd.Id,
                    PredictedClassificationId = _unknownId,
                    IsRead = false,
                    ReadTime = SqlHelper.GetSqlMinDateTime(),
                    PredictionScore = -1,
                    IsClicked = false
                };
                _dataContext.UserItems.InsertOnSubmit(uitem);
            }
            _dataContext.SubmitChanges();
        }

        public void AddItemsForUser(Guid userId, int feedId, DateTime insertedAfter) {
            var vitems = from i in _dataContext.Items
                        where i.FeedId == feedId
                        && i.InsertedOn >= insertedAfter.ToUniversalTime()
                        select i;
            List<Item> items = vitems.ToList();
            foreach( Item item in items ) {
                UserItem uitem = new UserItem {
                    UserId = userId,
                    ItemId = item.Id,
                    PredictedClassificationId = _unknownId,
                    IsRead = false,
                    ReadTime = SqlHelper.GetSqlMinDateTime(),
                    PredictionScore = -1,
                    IsClicked = false
                };
                _dataContext.UserItems.InsertOnSubmit(uitem);
            }
            _dataContext.SubmitChanges();
        }        

        public IQueryable<Item> GetClickedItems(Guid userId, DateTime insertedAfter) {
            var items = from i in _dataContext.Items
                        join ui in _dataContext.UserItems
                        on i.Id equals ui.ItemId
                        where ui.IsClicked
                        && ui.UserId == userId
                        && i.InsertedOn >= insertedAfter.ToUniversalTime()
                        select i;
            return items;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="predictions"></param>
        public IQueryable<ItemUserDetails> GetReadItemUserDetails(Guid userId, int feedId, DateTime readAfter) {
            var uitems = from i in _dataContext.Items
                         join ui in _dataContext.UserItems
                         on i.Id equals ui.ItemId
                         where ui.IsRead
                         && ui.ReadTime >= readAfter.ToUniversalTime()
                         && i.FeedId == feedId
                         && ui.UserId == userId
                         select new ItemUserDetails {
                             Author = i.Author,
                             Description = i.Description,
                             Excerpt = i.Excerpt,
                             FeedId = i.FeedId,
                             FeedTitle = i.Feed.Title,
                             IsClicked = ui.IsClicked,
                             IsRead = ui.IsRead,
                             ItemId = i.Id,
                             Link = i.Link,
                             PubDate = i.PubDate,
                             Score = ui.PredictionScore,
                             Title = i.Title
                         };
            return uitems;
        }

        public IQueryable<Item> GetReadUnclickedItems(Guid userId, DateTime insertedAfter) {
            var items = from i in _dataContext.Items
                        join ui in _dataContext.UserItems
                        on i.Id equals ui.ItemId
                        where ui.IsRead && !ui.IsClicked
                        && ui.UserId == userId
                        && i.InsertedOn >= insertedAfter.ToUniversalTime()
                        select i;
            return items;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="predictions"></param>
        public IQueryable<ItemUserDetails> GetReadInterestingItemUserDetails(Guid userId, DateTime readAfter) {
            var items = from i in _dataContext.Items
                        join ui in _dataContext.UserItems
                        on i.Id equals ui.ItemId
                        where ui.IsRead
                        && ui.ReadTime >= readAfter.ToUniversalTime()
                        && ui.PredictedClassificationId == _interestingId
                        && ui.UserId == userId
                        select new ItemUserDetails {
                            Author = i.Author,
                            Description = i.Description,
                            Excerpt = i.Excerpt,
                            FeedId = i.FeedId,
                            FeedTitle = i.Feed.Title,
                            IsClicked = ui.IsClicked,
                            IsRead = ui.IsRead,
                            ItemId = i.Id,
                            Link = i.Link,
                            PubDate = i.PubDate,
                            Score = ui.PredictionScore,
                            Title = i.Title
                        };
            return items;
        }

        /// <summary>
        /// Gets all unread items. Does not matter if it has been classified before.
        /// If the item has been classified and the user has not yet read it, there is a chance that the new model developed for the user
        /// will classify it differently. I want to show the latest classification.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IQueryable<Item> GetUnreadItems(Guid userId, DateTime insertedAfter) {
            var items = from i in _dataContext.Items
                        join ui in _dataContext.UserItems
                        on i.Id equals ui.ItemId
                        where !ui.IsRead
                        && i.InsertedOn >= insertedAfter.ToUniversalTime()
                        && ui.UserId == userId
                        select i;
            return items;
        }

        public IQueryable<ItemUserDetails> GetUnreadItemUserDetails(Guid userId, int feedId, DateTime insertedAfter) {
            var items = from i in _dataContext.Items
                        join ui in _dataContext.UserItems
                        on i.Id equals ui.ItemId
                        where !ui.IsRead
                        && i.InsertedOn >= insertedAfter.ToUniversalTime()
                        && i.FeedId == feedId
                        && ui.UserId == userId
                        select new ItemUserDetails {
                            Author = i.Author,
                            Description = i.Description,
                            Excerpt = i.Excerpt,
                            FeedId = i.FeedId,
                            FeedTitle = i.Feed.Title,
                            IsClicked = ui.IsClicked,
                            IsRead = ui.IsRead,
                            ItemId = i.Id,
                            Link = i.Link,
                            PubDate = i.PubDate,
                            Score = ui.PredictionScore,
                            Title = i.Title
                        };
            return items;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="predictions"></param>
        public IQueryable<ItemUserDetails> GetUnreadInterestingItemUserDetails(Guid userId, DateTime insertedAfter) {
            var items = from i in _dataContext.Items
                        join ui in _dataContext.UserItems
                        on i.Id equals ui.ItemId
                        where !ui.IsRead
                        && i.InsertedOn >= insertedAfter.ToUniversalTime()
                        && ui.PredictedClassificationId == _interestingId
                        && ui.UserId == userId
                        select new ItemUserDetails {
                            Author = i.Author,
                            Description = i.Description,
                            Excerpt = i.Excerpt,
                            FeedId = i.FeedId,
                            FeedTitle = i.Feed.Title,
                            IsClicked = ui.IsClicked,
                            IsRead = ui.IsRead,
                            ItemId = i.Id,
                            Link = i.Link,
                            PubDate = i.PubDate,
                            Score = ui.PredictionScore,
                            Title = i.Title
                        };
            return items;
        }
        
        public void SetClassification(Guid userId, Dictionary<int, Avilay.TextMining.Classification> predictions) {
            List<int> itemIds = predictions.Keys.ToList();
            List<int> brokenIds = new List<int>();
            for( int i = 0; i < itemIds.Count; i = i + 2000 ) {
                int x = Math.Min(2000, itemIds.Count - i);
                brokenIds = itemIds.GetRange(i, x);
                var uitems = from ui in _dataContext.UserItems
                             where ui.UserId == userId
                             && brokenIds.Contains(ui.ItemId)
                             select ui;
                foreach( UserItem userItem in uitems.ToArray() ) {
                    Avilay.TextMining.Classification c = predictions[userItem.ItemId];
                    if( c.Label == "interesting" )
                        userItem.PredictedClassificationId = _interestingId;
                    else if( c.Label == "uninteresting" )
                        userItem.PredictedClassificationId = _uninterestingId;
                    else
                        throw new ArgumentException("Invalid label value: " + c.Label);
                    userItem.PredictionScore = c.Score;
                }
                _dataContext.SubmitChanges();
            }
        }

        public void MarkItemsAsRead(Guid userId, List<int> ids) {            
            List<int> brokenIds = new List<int>();
            for( int i = 0; i < ids.Count; i = i + 2000 ) {
                int x = Math.Min(2000, ids.Count - i);
                brokenIds = ids.GetRange(i, x);
                var uitems = from ui in _dataContext.UserItems
                             where ui.UserId == userId
                             && brokenIds.Contains(ui.ItemId)
                             select ui;
                foreach( UserItem userItem in uitems ) {
                    userItem.IsRead = true;
                    userItem.ReadTime = DateTime.Now.ToUniversalTime();
                }
                _dataContext.SubmitChanges();
            }            
        }

        public void MarkItemAsClicked(Guid userId, int itemId) {
            var uitems = from ui in _dataContext.UserItems
                         where ui.UserId == userId
                         && ui.ItemId == itemId
                         select ui;
            if( uitems.Count() == 1 ) {
                UserItem userItem = uitems.First();
                userItem.IsClicked = true;
                _dataContext.SubmitChanges();
            }
            else {
                throw new Exception("Number of UserItem is " + uitems.Count() + " for user " + userId + " and item id " + itemId);
            }
        }
    }
}
