using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Feeder.Models;

namespace Feeder.Repositories
{
    public interface IFeedRepository
    {
        IQueryable<Feed> GetAllFeeds();
        Feed Add(Feed fm);
        void Update(Feed fm);
        void Subscribe(int id);
        void Unsubscribe(int id);
        IQueryable<Feed> GetSubscribedFeeds();
        Feed GetFeed(int id);
    }
}
