using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Feeder.Models;
using System.Web.Security;
using Avilay.Utils.Logging;

namespace Feeder.Repositories
{
    public class FeedRepository : IFeedRepository
    {
        private SnapdragonDataContext _dataContext;
        private Guid _userId;

        public FeedRepository() {            
            _dataContext = new SnapdragonDataContext();
            if( Membership.GetUser() != null ) {
                _userId = (Guid)Membership.GetUser().ProviderUserKey;
            }
        }

        public FeedRepository(SnapdragonDataContext dataContext, Guid userId) {
            _dataContext = dataContext;
            _userId = userId;
        }

        public IQueryable<Feed> GetSubscribedFeeds() {
            var subFeeds = from f in _dataContext.Feeds
                           join uf in _dataContext.UserFeeds
                           on f.Id equals uf.FeedId
                           where uf.UserId == _userId
                           select f;

            return subFeeds;
        }

        public void Subscribe(int feedId) {            
            var userFeeds = from uf in _dataContext.UserFeeds
                            where uf.UserId == _userId && uf.FeedId == feedId
                            select uf;
            if( userFeeds.Count() == 0 ) {
                UserFeed userFeed = new UserFeed {
                    UserId = _userId,
                    FeedId = feedId
                };
                _dataContext.UserFeeds.InsertOnSubmit(userFeed);
                _dataContext.SubmitChanges();
            }
        }

        public void Unsubscribe(int feedId) {
            var userFeeds = from uf in _dataContext.UserFeeds
                            where uf.UserId == _userId && uf.FeedId == feedId
                            select uf;
            if( userFeeds.Count() > 0 ) {
                _dataContext.UserFeeds.DeleteOnSubmit(userFeeds.First());
                _dataContext.SubmitChanges();
            }
        }

        public Feed Add(Feed feedToAdd) {
            var feeds = from f in _dataContext.Feeds
                        where f.Url == feedToAdd.Url
                        select f;
            Feed feed = null;
            if( feeds.Count() == 0 ) {
                feedToAdd.LastChecked = feedToAdd.LastChecked == DateTime.MinValue ? SqlHelper.GetSqlMinDateTime() : feedToAdd.LastChecked;
                feedToAdd.LastPublished = feedToAdd.LastPublished == DateTime.MinValue ? SqlHelper.GetSqlMinDateTime() : feedToAdd.LastPublished;
                feed = new Feed {
                    ContentUrl = feedToAdd.ContentUrl,
                    Description = feedToAdd.Description,
                    LastChecked = feedToAdd.LastChecked,
                    LastPublished = feedToAdd.LastPublished,
                    Title = feedToAdd.Title,
                    Url = feedToAdd.Url
                };
                _dataContext.Feeds.InsertOnSubmit(feed);
                _dataContext.SubmitChanges();                
            }
            else {
                feed = feeds.First();
            }

            return feed;
        }

        /// <summary>
        /// LastChecked and LastPublished have to be present in feedToUpdate
        /// Url cannot be updated and it has to be provided in feedToUpdate
        /// All other fields are optional
        /// </summary>
        /// <param name="feedToUpdate"></param>
        public void Update(Feed feedToUpdate) {
            var feeds = from f in _dataContext.Feeds
                        where f.Url == feedToUpdate.Url
                        select f;
            Feed feed = feeds.First();
            if( !string.IsNullOrEmpty(feedToUpdate.ContentUrl) )
                feed.ContentUrl = feedToUpdate.ContentUrl;
            if( !string.IsNullOrEmpty(feedToUpdate.Description) )
                feed.Description = feedToUpdate.Description;
            feed.LastChecked = feedToUpdate.LastChecked == DateTime.MinValue ? SqlHelper.GetSqlMinDateTime() : feedToUpdate.LastChecked;
            feed.LastPublished = feedToUpdate.LastPublished == DateTime.MinValue ? SqlHelper.GetSqlMinDateTime() : feedToUpdate.LastPublished;                
            if( !string.IsNullOrEmpty(feedToUpdate.Title) )
                feed.Title = feedToUpdate.Title;
            _dataContext.SubmitChanges();
        }

        public IQueryable<Feed> GetAllFeeds() {
            var allFeeds = from f in _dataContext.Feeds
                           select f;
            return allFeeds;
        }

        public Feed GetFeed(int feedId) {
            var feed = from f in _dataContext.Feeds
                       where f.Id == feedId
                       select f;
            if( feed.Count() == 1 ) {
                return feed.First();
            }
            else {
                return null;
            }
        }

        public void Debug() {
            Feed feed = new Feed {
                ContentUrl = "http://tempuri.org",
                Description = "From Debug",
                LastChecked = SqlHelper.GetSqlMinDateTime(),
                LastPublished  = DateTime.Now,
                Title = "Debug Method",
                Url = "http://tempuri.org/rss"
            };
            _dataContext.Feeds.InsertOnSubmit(feed);
            _dataContext.SubmitChanges();
        }
    }
}
