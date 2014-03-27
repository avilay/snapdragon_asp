using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Feeder.Services;
using Feeder.Models;
using Avilay.Utils.Logging;
using System.Threading;
using System.Configuration;

namespace Feeder.Controllers
{
    [HandleError]
    [Authorize]    
    public class ReaderController : Controller
    {
        private IReaderService _readerSvc;
        private IFeedService _feedSvc;
        private int _totPages;
        private int[] _starts;
        private int[] _ends;

        public ReaderController() {
            _readerSvc = new ReaderService();
            _feedSvc = new FeedService();
        }
 
        public ActionResult Index() {
            LogFunctions.Info("ReadController.Index");

            return RedirectToAction("ReadInterestingItems");
            //return RedirectToAction("TestAction");
        }

        public ActionResult ReadFeedItems(int id) {
            LogFunctions.Info("ReadController.ReadFeedItems("+id+")");

            Feed feed = _feedSvc.GetFeed(id);
            List<ItemUserDetails> items = _readerSvc.GetSessionItemsByFeed(id);
            //List<int> ids = new List<int>(items.Count);
            //for( int i = 0; i < items.Count; i++ ) {
            //    ids.Add(items[i].ItemId);
            //}
            //_readerSvc.MarkItemsAsRead(ids);
            IOrderedEnumerable<ItemUserDetails> sortedItems = items.OrderByDescending(item => item.PubDate);
            IOrderedEnumerable<Feed> subscribedFeeds = _feedSvc.GetSubscribed().OrderByDescending(sfeed => sfeed.LastPublished);
            Paginate(sortedItems.Count());
            if ( _totPages > 0 ) {
                List<int> idsPage1 = new List<int>();
                for ( int i = _starts[0]; i < _ends[0]; i++ ) {
                    idsPage1.Add(sortedItems.ElementAt(i).ItemId);
                }
                _readerSvc.MarkItemsAsRead(idsPage1);
            }
            ViewData.Add("feed", feed);
            ViewData.Add("sortedItems", sortedItems);
            ViewData.Add("subscribedFeeds", subscribedFeeds);
            ViewData.Add("totPages", _totPages);
            ViewData.Add("starts", _starts);
            ViewData.Add("ends", _ends);
            ViewData.Add("currentFeedId", id);
            return View();
        }

        public ActionResult ReadInterestingItems() {
            LogFunctions.Info("ReaderController.ReadInterestingItems");

            List<ItemUserDetails> items = _readerSvc.GetSessionInterestingItems();
            //List<int> ids = new List<int>(items.Count);
            //for( int i = 0; i < items.Count; i++ ) {
            //    ids.Add(items[i].ItemId);
            //}
            //_readerSvc.MarkItemsAsRead(ids);
            IOrderedEnumerable<ItemUserDetails> sortedItems = items.OrderByDescending(item => item.Score);
            IOrderedEnumerable<Feed> subscribedFeeds = _feedSvc.GetSubscribed().OrderByDescending(sfeed => sfeed.LastPublished);
            Paginate(sortedItems.Count());
            if ( _totPages > 0 ) {
                List<int> idsPage1 = new List<int>();
                for ( int i = _starts[0]; i < _ends[0]; i++ ) {
                    idsPage1.Add(sortedItems.ElementAt(i).ItemId);
                }
                _readerSvc.MarkItemsAsRead(idsPage1);            
            }
            ViewData.Add("sortedItems", sortedItems);
            ViewData.Add("subscribedFeeds", subscribedFeeds);
            ViewData.Add("totPages", _totPages);
            ViewData.Add("starts", _starts);
            ViewData.Add("ends", _ends);            
            return View();
        }

        private void Paginate(int total) {
            int readerPageSize = Int32.Parse(ConfigurationManager.AppSettings["readerPageSize"]);
            double x = (double)total / readerPageSize;
            _totPages = (int)Math.Ceiling(x);
            _starts = new int[_totPages];
            _ends = new int[_totPages];
            for ( int i = 0; i < _totPages; i++ ) {
                _starts[i] = i * 10;
                _ends[i] = Math.Min(_starts[i] + 10, total);
            }
        }

        public string MarkItemAsClicked(int id) {
            LogFunctions.Info("ReaderController.MarkItemAsClicked(" + id + ")");

            _readerSvc.MarkItemAsClicked(id);
            return "done";
        }

        public string MarkItemsAsRead(string csvItemIds) {
            LogFunctions.Info("ReaderController.MarkItemAsRead(" + csvItemIds + ")");

            string[] itemIds = csvItemIds.Split(new char[] { ',' });
            List<int> ids = new List<int>();
            foreach (string itemId in itemIds) {
                ids.Add(Int32.Parse(itemId));
            }
            _readerSvc.MarkItemsAsRead(ids);
            return "done";
        }

        public ActionResult ThrowException() {
            LogFunctions.Info("ReaderController.ThrowException");

            object o = null;
            o.ToString();
            return View();
        }

        public ActionResult TestAction() {
            LogFunctions.Info("ReaderController.TestAction");

            ViewData["ver"] = "3";
            return View();
        }
    }
}
