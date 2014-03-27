using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using Avilay.Utils.Logging;
using Feeder.Repositories;
using Feeder.Models;
using Avilay.Syndication;
using Feed = Feeder.Models.Feed;
using Item = Feeder.Models.Item;
using Avilay.TextMining;
using Avilay.TextMining.Bayes;
using System.Web.Configuration;
using System.Web.Security;
using Avilay.Utils;
using System.Configuration;

namespace Feeder.Services
{
    public class DaemonService : IDaemonService
    {
        private IFeedRepository _feedRepo;
        private IItemRepository _itemRepo;
        private IFeederNaiveBayesModel _model;
        private DateTime _cutOff = DateTime.Now.AddDays(-1 * cutoffPeriod);
        private DateTime _learnCutoff = DateTime.Now.AddDays(-1 * learnCutoffPeriod);
        private Guid[] _allTestUsers = null;

        // TODO: The following four values must come for a config file
        private static int cutoffPeriod = Int32.Parse(ConfigurationManager.AppSettings["daysForWhichToShowItems"]);
        private static string interestingLabel = ConfigurationManager.AppSettings["interestingLabel"];
        private static string uninterestingLabel = ConfigurationManager.AppSettings["uninterestingLabel"];
        private static int excerptSize = Int32.Parse(ConfigurationManager.AppSettings["excerptSizeInChars"]);
        private static int learnCutoffPeriod = Int32.Parse(ConfigurationManager.AppSettings["daysForWhichToLearn"]);

        public DaemonService(IFeedRepository feedRepo, IItemRepository itemRepo, IFeederNaiveBayesModel model, Guid[] allTestUsers) {
            _feedRepo = feedRepo;
            _itemRepo = itemRepo;
            _model = model;
            _allTestUsers = allTestUsers;
        }

        public DaemonService() {
            _feedRepo = new FeedRepository();
            _itemRepo = new ItemRepository();
            _model = new FeederNaiveBayesModel();
        }

        public void AsyncCrawlAndClassify() {
            Thread crawlAndClassify = new Thread(new ThreadStart(CrawlAndClassify));
            crawlAndClassify.Start();            
        }

        public void AsyncLearn() {
            Thread learn = new Thread(new ParameterizedThreadStart(Learn));
            learn.Start(GetAllUserIds());

        }

        public bool IsValid(string key) {
            // TODO: implement some kind of key validation scheme
            return true;
        }

        private void CrawlAndClassify(){
            LogFunctions.Info("CrawlAndClassify started");
            Feed[] feeds = _feedRepo.GetAllFeeds().ToArray();
            Crawl(feeds);
            Classify(GetAllUserIds());
            LogFunctions.Info("CrawlAndClassify complete");
        }

        public void Crawl(Feed[] feeds) {
            DateTime crawlTime = DateTime.Now.ToUniversalTime();
            foreach( Feed feed in feeds ) {
                try {
                    DateTime lastCrawl = feed.LastChecked;
                    FeedReader reader = FeedReaderFactory.Create(new Uri(feed.Url));
                    Avilay.Syndication.Feed feedDetails = reader.FeedDetails();
                    DateTime lastPub = reader.GetLastPublishedDate();
                    if( lastPub > lastCrawl ) {
                        Item[] items = Transform(reader.AllFeedItems());
                        foreach( Item item in items ) {
                            if( item.PubDate > lastCrawl ) {
                                _itemRepo.Add(item, feed.Id);
                            }
                            else if( item.PubDate == DateTime.MinValue ) {
                                ProcessDateLess(item, feed);
                            }
                        }
                    }
                    else if( lastPub == DateTime.MinValue ) {
                        Item[] items = Transform(reader.AllFeedItems());
                        foreach( Item item in items ) {
                            ProcessDateLess(item, feed);
                        }
                    }
                    //TODO: test if feed got updated
                    Feed fm = new Feed {
                        ContentUrl = feedDetails.ContentUrl.ToString(),
                        Description = feedDetails.Description,
                        LastChecked = crawlTime,                        
                        LastPublished = reader.GetLastPublishedDate(),
                        Title = feedDetails.Title,
                        Url = feed.Url
                    };
                    _feedRepo.Update(fm);
                }
                catch( Exception e ) {                    
                    //LogFunctions.Error(feed.Title + " was not crawled", e);
                    LogFunctions.Error(string.Format("{0} {1} was not crawled", feed.Id, feed.Url), e);
                }
            }
        }

        public void Classify(Guid[] userIds) {
            foreach (Guid userId in userIds) {
                try {
                    _model.SetUser(userId);
                    if( _model.HasModel() ) {
                        NaiveBayesClassifier nb = new NaiveBayesClassifier(_model);
                        nb.Load();
                        Dictionary<int, Avilay.TextMining.Classification> predictions = new Dictionary<int, Avilay.TextMining.Classification>();
                        foreach( Item item in _itemRepo.GetUnreadItems(userId, _cutOff) ) {
                            Avilay.TextMining.Classification classification = nb.Classify(item.Title + " " + item.Excerpt);
                            if( classification.Score == 0.5 ) {
                                classification = new Avilay.TextMining.Classification(uninterestingLabel, classification.Score);
                            }
                            predictions.Add(item.Id, classification);
                        }
                        _itemRepo.SetClassification(userId, predictions);
                    }
                }
                catch( Exception e ) {
                    LogFunctions.Error("Items for " + userId.ToString() + " was not classified", e);
                }
            }
        }

        // TODO: I have deleted the model.Clear method to save on db calls. This means that the system never forgets!
        // Even if the user was interested in a word some months ago, if that word appears in a new item it will
        // be treated as if it were still interesting. Don't know the consequences of this yet. Will deploy this version
        // and see what happens.
        // If the word keeps apperaing in new interesting items but the user keeps ignoring it, then its interesting probability
        // will go down and uninteresting probability will go up.
        // the weirdness is only for word that stops appearing for a while and then suddenly pops up.
        public void Learn(object userIdObjects) {
            LogFunctions.Info("Learn started");
            
            Guid[] userIds = (Guid[])userIdObjects;
            foreach( Guid userId in userIds ) {
                try {
                    LogFunctions.Debug("Learning model for user: " + userId);
                    List<TextInstance> instances = new List<TextInstance>();
                    Avilay.TextMining.Classification interesting = new Avilay.TextMining.Classification(interestingLabel, 0);
                    Avilay.TextMining.Classification uninteresting = new Avilay.TextMining.Classification(uninterestingLabel, 0);
                    foreach( Item item in _itemRepo.GetClickedItems(userId, _learnCutoff) ) {
                        TextInstance instance = new TextInstance(item.Title + " " + item.Excerpt, interesting);
                        instances.Add(instance);
                    }
                    foreach( Item item in _itemRepo.GetReadUnclickedItems(userId, _learnCutoff) ) {
                        TextInstance instance = new TextInstance(item.Title + " " + item.Excerpt, uninteresting);
                        instances.Add(instance);
                    }
                    LogFunctions.Debug("Number of instances: " + instances.Count);
                    if (instances.Count > 0) {
                        _model.SetUser(userId);
                        NaiveBayesClassifier nb = new NaiveBayesClassifier(_model);
                        nb.Learn(instances.ToArray());
                        nb.Save();
                        _model.MarkAsHasModel();
                    }
                    else {
                        _model.SetUser(userId);
                        _model.DeleteModel();
                        LogFunctions.Info("Model for " + userId.ToString() + " was deleted");
                    }
                }
                catch( Exception e ) {
                    LogFunctions.Error("Model for " + userId.ToString() + " was not learnt", e);
                }
            }
            LogFunctions.Info("Learn complete");
        }
        
        private void ProcessDateLess(Item item, Feed feed) {
            var allItems = from i in feed.Items
                           where i.InsertedOn > _cutOff && i.IsSame(item)
                           select i;
            if( allItems.Count() == 0 ) {
                _itemRepo.Add(item, feed.Id);
            }
        }

        private Item[] Transform(Avilay.Syndication.Item[] itemDetails) {
            Item[] items = new Item[itemDetails.Length];
            for( int i = 0; i < items.Length; i++ ) {
                items[i] = new Item {
                    Author = itemDetails[i].Author,
                    Description = itemDetails[i].Description,
                    Excerpt = ComputeExcerpt(itemDetails[i].Title, itemDetails[i].Description),
                    Link = itemDetails[i].Link.ToString(),
                    PubDate = itemDetails[i].PubDate,
                    Title = itemDetails[i].Title
                };
            }
            return items;
        }

        private string ComputeExcerpt(string title, string description) {
            int size = excerptSize - 3; //for the trailing ...
            string ret = "";
            int t = title.Length + 1;
            int left = size - t;
            if( left > 0 ) {
                //HtmlParser parser = new HtmlParser(null, description);
                string description2 = HtmlParser.ExtractText(description, new List<string>());
                if( description2.Length > left ) {
                    ret = description2.Substring(0, left) + "...";
                }
                else {
                    //the description will fit in the excerpt
                    ret = description2;
                }
            }
            else {
                ret = ""; //the title is longer than the required size.
            }
            return ret;
        }

        private Guid[] GetAllUserIds() {
            if( _allTestUsers == null ) {
                MembershipUserCollection allUsers = Membership.GetAllUsers();
                Guid[] userIds = new Guid[allUsers.Count];
                int i = 0;
                foreach( MembershipUser user in allUsers ) {
                    userIds[i++] = (Guid)user.ProviderUserKey;
                }
                return userIds;
            }
            else {
                return _allTestUsers;
            }
        }
    }
}
