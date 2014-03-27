using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Feeder.Models;

namespace Feeder.Services
{
    public enum UrlState
    {
        SUPPORTED_FEED,
        NOT_SUPPORTED_FEED,
        NON_FEED_MARKUP,
        OTHER
    }

    public class CacheData
    {
        public UrlState State { get; set; }
        public string Content { get; set; }
    }

    public interface IFeedService
    {
        void ClearCache();

        Dictionary<Uri, CacheData> GetCache();

        void SetCache(Dictionary<Uri, CacheData> cache);

        UrlState GetUrlState(Uri uri);

        ICollection<Uri> ScanPage(Uri uri);

        bool IsSubscribed(Uri uri);

        Feed Subscribe(Uri uri);

        ICollection<Feed> GetSubscribed();

        ICollection<Feed> GetAll();

        Feed GetFeed(Uri feedUri);

        Feed GetFeed(int feedId);

        Uri[] ScanOpml(string content);

        void Unsubscribe(int feedId);
    }
}
