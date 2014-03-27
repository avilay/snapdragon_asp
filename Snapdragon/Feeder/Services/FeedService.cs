using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Avilay.Syndication;
using Feeder.Models;
using Avilay.Utils.Logging;
using Feeder.Repositories;
using System.Xml;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using Feed = Feeder.Models.Feed;
using Item = Feeder.Models.Item;
using System.Web.Security;
using System.Configuration;

namespace Feeder.Services
{
    public class FeedService : IFeedService
    {
        private IFeedRepository _repository;
        private IItemRepository _itemRepo;
        private Dictionary<Uri, CacheData> _cache;

        private IDaemonService _daemonSvc;

        private static int cutoffPeriod = Int32.Parse(ConfigurationManager.AppSettings["daysForWhichToShowItems"]);

        /// <summary>
        /// Uses the default repository.
        /// </summary>
        public FeedService() {
            _cache = new Dictionary<Uri, CacheData>();
            _repository = new FeedRepository();
            _daemonSvc = new DaemonService();
            _itemRepo = new ItemRepository();
        }

        /// <summary>
        /// Uses the passed in repository, to be used with tests
        /// </summary>
        /// <param name="repository"></param>
        public FeedService(IFeedRepository repository) {
            _repository = repository;
            _cache = new Dictionary<Uri, CacheData>();
        }

        /// <summary>
        /// Clears the internal cache of visited URLs
        /// </summary>
        public void ClearCache() {
            _cache.Clear();
        }

        /// <summary>
        /// Gets the internal cache of visited URLs so it can be stored in a session across different requests.
        /// </summary>
        /// <returns></returns>
        public Dictionary<Uri, CacheData> GetCache() {
            return _cache;
        }

        /// <summary>
        /// Sets the internal cache.
        /// </summary>
        /// <param name="cache"></param>
        public void SetCache(Dictionary<Uri, CacheData> cache) {
            foreach( Uri key in cache.Keys ) {
                if( !_cache.ContainsKey(key) ) {
                    _cache.Add(key, cache[key]);
                }
            }
        }

        /// <summary>
        /// Checks the given URL and determines whether it points to a SUPPORTED_FEED, NOT_SUPPORTED_FEED, NON_FEED_MARKUP, OTHER.
        /// This method is guranteed to return with a UrlState unless the passed in uri is a dead link and causes a WebException.
        /// Also, passed in URI cannot be NULL. A call to this method updates the internal cache if needed.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        /// <exception cref="WebException">Thrown if the passed in URI is inaccessible</exception>
        public UrlState GetUrlState(Uri uri) {
            if( _cache.ContainsKey(uri) ) {
                return _cache[uri].State;
            }
            else {
                WebRequest request = WebRequest.Create(uri.ToString());
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string content = reader.ReadToEnd();
                reader.Close();
                response.Close();
                
                if( response.ContentType.Contains("text") || response.ContentType.Contains("xml") ) {
                    try {
                        FeedState fstate = FeedReaderFactory.GetFeedState(uri, content);
                        if( fstate == FeedState.NOT_SUPPORTED_FEED ) {
                            _cache.Add(uri, new CacheData { State = UrlState.NOT_SUPPORTED_FEED, Content = null });
                        }
                        else if( fstate == FeedState.SUPPORTED_FEED ) {
                            _cache.Add(uri, new CacheData { State = UrlState.SUPPORTED_FEED, Content = content });
                        }
                        else if( fstate == FeedState.UNKNOWN_MARKUP ) {
                            _cache.Add(uri, new CacheData { State = UrlState.NON_FEED_MARKUP, Content = content });
                        }
                    }
                    catch(XmlException xe){
                        _cache.Add(uri, new CacheData { State = UrlState.OTHER, Content = null });
                        LogFunctions.Warn(uri.ToString() + " has a content type of " + response.ContentType + " did not have xml", xe);
                    }
                }
                else {
                    _cache.Add(uri, new CacheData {
                        State = UrlState.OTHER, Content = null });
                }
            }
            return _cache[uri].State;
        }

        /// <summary>
        /// Scans in the passed in URI and extracts links to feeds - both supported and unsupported.
        /// This ignores any non-feed links, non-markup links, dead links, etc.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        /// <exception cref="WebException">Thrown if the passed in URI is inaccessible</exception>
        public ICollection<Uri> ScanPage(Uri uri) {
            if( GetUrlState(uri) == UrlState.NON_FEED_MARKUP ) {
                string content = _cache[uri].Content;
                Regex link = new Regex("<link[^>]*type\\s*=[^>]*(rss|atom)+[^>]*>", RegexOptions.IgnoreCase);
                Regex href = new Regex("href\\s*=\\s*(\"|')+([^'\"]*)", RegexOptions.IgnoreCase);
                MatchCollection matches = link.Matches(content);
                HashSet<Uri> feedUris = new HashSet<Uri>();
                for( int i = 0; i < matches.Count; i++ ) {
                    string linkTag = matches[i].Value;
                    string url = href.Match(linkTag).Result("$2");
                    url = HttpUtility.HtmlDecode(url);
                    try {
                        Uri feedUri = new Uri(uri, url);                        
                        UrlState state = GetUrlState(feedUri);
                        if( state == UrlState.SUPPORTED_FEED || state == UrlState.NOT_SUPPORTED_FEED ) {
                            feedUris.Add(feedUri);
                        }
                    }
                    catch( WebException we ) {
                        LogFunctions.Warn(uri.ToString() + " has a dead link: " + matches[i].Result("$3"), we);
                    }
                    catch( UriFormatException ufe ) {
                        LogFunctions.Warn(uri.ToString() + " has a malformed link: " + matches[i].Result("$3"), ufe);
                    }
                }
                return feedUris;
            }
            else {
                return null;
            }
        }

        /// <summary>
        /// If the current user has subscribed to this URL then returns true otherwise false.
        /// It does not load the URL.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException">Thrown if multiple feeds in the db have the same URL</exception>
        public bool IsSubscribed(Uri uri) {
            string url = uri.ToString();
            var subFeeds = from sf in _repository.GetSubscribedFeeds()
                           where sf.Url == url
                           select sf;
            if( subFeeds.Count() == 0 ) {
                return false;
            }
            else if( subFeeds.Count() == 1 ) {
                return true;
            }
            else {
                throw new ApplicationException("Multiple subscribed feeds have the same URL: " + url);
            }
        }

        /// <summary>
        /// Subscribes the current user to the passed in URI and returns a Feed object from the database representing that feed.
        /// If the URI does not point to a supported feed, then a null value is returned. If the feed does not exist in the db
        /// and is current inaccessible, then a WebException is thrown. If there is a load or a parse error then a BadlyFormedFeedException is thrown.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        /// <exception cref="WebException">Thrown if the URL is inaccessible</exception>
        /// <exception cref="BadlyFormedFeedException">Thrown if the URL points to a supported feed but there is a load or a parse error in the feed XML</exception>
        public Feed Subscribe(Uri uri) {
            string url = uri.ToString();
            Feed fm = null;
            var allFeeds = from af in _repository.GetAllFeeds()
                           where af.Url == url
                           select af;
            if( allFeeds.Count() == 0 ) {
                // nobody has subscribed to this feed
                if( GetUrlState(uri) != UrlState.SUPPORTED_FEED ) {
                    return null;
                }
                FeedReader reader = FeedReaderFactory.Create(uri, _cache[uri].Content);
                Avilay.Syndication.Feed feedDetails = reader.FeedDetails();
                fm = new Feed();
                fm.Id = -1;
                fm.ContentUrl = feedDetails.ContentUrl.ToString();
                fm.Description = feedDetails.Description;
                fm.Title = feedDetails.Title;
                fm.Url = feedDetails.Url.ToString();
                fm.LastPublished = reader.GetLastPublishedDate();
                fm.LastChecked = DateTime.MinValue;
                fm = _repository.Add(fm);
                _repository.Subscribe(fm.Id);
                _daemonSvc.Crawl(new Feed[] { fm });
                return fm;
            }
            else if( allFeeds.Count() == 1 ) {
                fm = allFeeds.First();
                var subFeeds = from sf in _repository.GetSubscribedFeeds()
                               where sf.Id == fm.Id
                               select sf;
                if( subFeeds.Count() == 1 ) {
                    return fm;
                }
                else if( subFeeds.Count() == 0 ) {
                    _repository.Subscribe(fm.Id);
                    Guid userId = (Guid)Membership.GetUser().ProviderUserKey;
                    _itemRepo.AddItemsForUser(userId, fm.Id, DateTime.Now.AddDays(-1 * cutoffPeriod));
                    return fm;
                }
                else {
                    throw new ApplicationException("Multiple feeds in the db have the same ID: " + fm.Id);
                }
                
            }
            else {
                throw new ApplicationException("Multiple feeds in the db have the same URL: " + uri.ToString());
            }
        }

        /// <summary>
        /// Gets all subscribed feeds for the current user
        /// </summary>
        /// <returns></returns>
        public ICollection<Feed> GetSubscribed() {
            var subFeeds = _repository.GetSubscribedFeeds();            
            List<Feed> subscribed = new List<Feed>();
            foreach( Feed fm in subFeeds ) {
                subscribed.Add(fm);
            }
            return subscribed;
        }

        public ICollection<Feed> GetAll() {
            var allFeeds = _repository.GetAllFeeds();
            List<Feed> all = new List<Feed>();
            foreach( Feed fm in allFeeds ) {
                all.Add(fm);
            }
            return all;
        }

        /// <summary>
        /// Look for the feed in the db and return that if it exists otherwise load the feed and return it without adding it to the db.
        /// The feed id for a feed model that does not exist in the db will be -1. If the URI is not a supported feed it will return null.
        /// </summary>
        /// <param name="feedUri"></param>
        /// <returns></returns>
        public Feed GetFeed(Uri feedUri) {
            Feed fm = null;
            var allFeeds = from af in _repository.GetAllFeeds()
                           where af.Url.Equals(feedUri)
                           select af;
            if( allFeeds.Count() == 0 ) {
                // nobody has subscribed to this feed
                if( GetUrlState(feedUri) != UrlState.SUPPORTED_FEED ) {
                    return null;
                }
                FeedReader reader = FeedReaderFactory.Create(feedUri, _cache[feedUri].Content);
                Avilay.Syndication.Feed feedDetails = reader.FeedDetails();
                fm = new Feed();
                fm.Id = -1;
                fm.ContentUrl = feedDetails.ContentUrl.ToString();
                fm.Description = feedDetails.Description;
                fm.Title = feedDetails.Title;
                fm.Url = feedDetails.Url.ToString();
                fm.LastPublished = reader.GetLastPublishedDate();
                return fm;
            }
            else if( allFeeds.Count() == 1 ) {
                fm = allFeeds.First();
                return fm;
            }
            else {
                throw new ApplicationException("Multiple feeds in the db have the same URL: " + feedUri.ToString());
            }
        }

        public Feed GetFeed(int feedId) {
            return _repository.GetFeed(feedId);
        }

        public Uri[] ScanOpml(string content) {
            Uri[] ret = null;
            try {
                OpmlReader reader = new OpmlReader(content);
                ret = reader.Feeds();
            }
            catch( XmlException xmle ) {
                LogFunctions.Warn("Bad OMPL", xmle);
                ret = new Uri[] { };
            }            
            return ret;
        }

        public void Unsubscribe(int feedId) {
            _repository.Unsubscribe(feedId);
        }
    }
}
