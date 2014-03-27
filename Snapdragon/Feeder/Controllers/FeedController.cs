using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Feeder.Services;
using Avilay.Utils.Logging;
using Feeder.Models;
using System.Xml;
using System.Net;
using Feeder.Repositories;
using Avilay.Syndication;
using System.IO;
using Feed = Feeder.Models.Feed;

namespace Feeder.Controllers
{
    public enum AddFailureState
    {
        BADLY_FORMED_FEED,
        INVALID_XML_FEED,
        NOT_SUPPORTED_FEED,
        NON_FEED_MARKUP,
        INACCESSIBLE_URL,
        OTHER
    }

    [HandleError]
    [Authorize]
    public class FeedController : Controller
    {
        IFeedService _feedSvc;
        HttpSessionStateBase _session;
        HttpRequestBase _request;

        public FeedController() : base() {
            _feedSvc = new FeedService();
        }

        public FeedController(FeedService feedSvc, HttpSessionStateBase session, HttpRequestBase request) {
            _feedSvc = feedSvc;
            _session = session;
            _request = request;
        }
        
        public ActionResult Index() {
            LogFunctions.Info("FeedController.Index");
            return View(_feedSvc.GetSubscribed());
        }

        public ActionResult RemoveFeed(int id) {
            LogFunctions.Info(string.Format("FeedController.RemoveFeed({0})", id));
            _feedSvc.Unsubscribe(id);
            return RedirectToAction("Index");
        }

        public ActionResult SearchOnPage(string url) {
            LogFunctions.Info(string.Format("FeedController.Search({0})", url));
            
            _feedSvc.ClearCache(); 
            Uri uri = new Uri(url);
            LogFunctions.Info(uri.Host);
            if( uri.Host == Request.ServerVariables["SERVER_NAME"] ) {
                return RedirectToAction("Index", "Help");
            }
            UrlState state = _feedSvc.GetUrlState(uri);
            if( state == UrlState.NON_FEED_MARKUP ) {
                ICollection<Uri> feedUrlsOnPage = _feedSvc.ScanPage(uri);
                List<Feed> newFeedsOnPage = new List<Feed>();
                List<Feed> oldFeedsOnPage = new List<Feed>();
                Dictionary<Uri, AddFailureState> failStates = new Dictionary<Uri,AddFailureState>();
                foreach( Uri feedUrlOnPage in feedUrlsOnPage ) {
                    try {
                        if( _feedSvc.GetUrlState(feedUrlOnPage) == UrlState.SUPPORTED_FEED ) {
                            Feed feed = _feedSvc.GetFeed(feedUrlOnPage);
                            if( _feedSvc.IsSubscribed(feedUrlOnPage) ) {
                                oldFeedsOnPage.Add(feed);                
                            }
                            else {
                                newFeedsOnPage.Add(feed);
                            }
                        }
                        else {                
                            failStates.Add(feedUrlOnPage, AddFailureState.NOT_SUPPORTED_FEED);
                        }
                    }
                    catch( BadlyFormedFeedException ) {
                        failStates.Add(feedUrlOnPage, AddFailureState.BADLY_FORMED_FEED);
                    }
                    catch( XmlException ) {
                        failStates.Add(feedUrlOnPage, AddFailureState.INVALID_XML_FEED);
                    }
                    catch( WebException we ) {
                        LogFunctions.Warn("Feed/SearchOnPage: dead link found in " + url, we);
                    }
                }
                ViewData.Add("newFeedsOnPage", newFeedsOnPage);
                ViewData.Add("oldFeedsOnPage", oldFeedsOnPage);
                ViewData.Add("failStates", failStates);
                GetSession().Add("cache", _feedSvc.GetCache());

                return View();
            }
            else {
                GetSession().Add("cache", _feedSvc.GetCache());
                return RedirectToAction("AddPopUp", new { url = url });
            }
        }

        public ActionResult AddPopUp(string url) {
            LogFunctions.Info(string.Format("FeedController.AddPopUp({0})", url));
            Dictionary<Uri, CacheData> cache = (Dictionary<Uri, CacheData>)GetSession()["cache"];
            _feedSvc.SetCache(cache);
            List<Uri> urisToAdd = new List<Uri>();
            if( url == null ) {
                foreach( string key in GetRequest().Form.Keys ) {
                    if( GetRequest().Form[key] == "on" ) {
                        urisToAdd.Add(new Uri(key));
                    }
                }
            }
            else {
                urisToAdd.Add(new Uri(url));
            }
            Add(urisToAdd);
            return View();
        }

        public ActionResult AddOpmlForm() {
            LogFunctions.Info("FeedController.AddOpmlForm");
            return View();
        }

        public ActionResult AddOpml() {
            LogFunctions.Info("FeedController.AddOpml");
            HttpPostedFileBase file = GetRequest().Files["opmlFile"];
            string content = "";
            using( StreamReader reader = new StreamReader(file.InputStream) ) {
                content = reader.ReadToEnd();
            }
            Uri[] urisToAdd = _feedSvc.ScanOpml(content);
            Add(urisToAdd.ToList<Uri>());
            return View("AddManually");
        }

        public ActionResult AddManuallyForm() {
            LogFunctions.Info("FeedController.AddManuallyForm");            
            return View();
        }

        public ActionResult AddManually(string feedUrl1, string feedUrl2, string feedUrl3, string feedUrl4, string feedUrl5) {
            LogFunctions.Info("FeedController.AddManually");

            string[] feedUrls = new string[] { feedUrl1, feedUrl2, feedUrl3, feedUrl4, feedUrl5 };
            List<Uri> urisToAdd = new List<Uri>();
            foreach( string feedUrl in feedUrls ) {
                try {
                    if( !string.IsNullOrEmpty(feedUrl) ) {
                        string url = feedUrl;
                        if( !feedUrl.StartsWith("http://") ) {
                            url = "http://" + feedUrl;
                        }
                        urisToAdd.Add(new Uri(url));
                    }
                }
                catch( UriFormatException ufe ) {
                    throw new UriFormatException(feedUrl + " " + ufe.Message);
                }
            }
            Add(urisToAdd);
            return View();
        }

        private HttpSessionStateBase GetSession() {
            if( _session != null ) return _session;
            else return Session;
        }

        private HttpRequestBase GetRequest() {
            if( _request != null ) return _request;
            else return Request;
        }

        private AddFailureState ConvertState(UrlState urlState) {
            AddFailureState ret = AddFailureState.OTHER;
            switch( urlState ) {
                case UrlState.NON_FEED_MARKUP:
                    ret = AddFailureState.NON_FEED_MARKUP;
                    break;
                case UrlState.NOT_SUPPORTED_FEED:
                    ret = AddFailureState.NOT_SUPPORTED_FEED;
                    break;
                case UrlState.OTHER:
                    ret = AddFailureState.OTHER;
                    break;
            }
            return ret;
        }

        private void Add(List<Uri> urisToAdd) {
            List<Feed> successes = new List<Feed>();
            List<Feed> oldFeeds = new List<Feed>();
            Dictionary<Uri, AddFailureState> failStates = new Dictionary<Uri, AddFailureState>();
            foreach( Uri uri in urisToAdd ) {
                try {
                    UrlState state = _feedSvc.GetUrlState(uri);
                    if( state == UrlState.SUPPORTED_FEED ) {
                        if( _feedSvc.IsSubscribed(uri) ) {
                            oldFeeds.Add(_feedSvc.GetFeed(uri));
                        }
                        else {
                            Feed Feed = _feedSvc.Subscribe(uri);
                            successes.Add(Feed);
                        }
                    }
                    else {
                        LogFunctions.Warn("Unable to add feed: " + uri.ToString() + ": state is " + state.ToString());
                        failStates.Add(uri, ConvertState(state));
                    }
                }
                catch( BadlyFormedFeedException bffe ) {
                    LogFunctions.Warn("Unable to add feed: Badly formed feed " + uri.ToString(), bffe);
                    failStates.Add(uri, AddFailureState.BADLY_FORMED_FEED);
                }
                catch( XmlException xe ) {
                    LogFunctions.Warn("Unable to add feed: Invalid feed " + uri.ToString(), xe);
                    failStates.Add(uri, AddFailureState.INVALID_XML_FEED);
                }
                catch( WebException we ) {
                    LogFunctions.Warn("Unable to add feed: Dead link " + uri.ToString(), we);
                    failStates.Add(uri, AddFailureState.INACCESSIBLE_URL);
                }
            }
            ViewData.Add("successes", successes);
            ViewData.Add("oldFeeds", oldFeeds);
            ViewData.Add("failStates", failStates);
        }

    }
}
