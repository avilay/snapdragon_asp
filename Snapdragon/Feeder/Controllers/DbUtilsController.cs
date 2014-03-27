using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Feeder.Models;
using Avilay.Utils.Logging;
using System.Reflection;

namespace Feeder.Controllers
{
    [HandleError]
    public class DbUtilsController : Controller
    {
        private SnapdragonDataContext _dataContext;

        public DbUtilsController() {
            _dataContext = new SnapdragonDataContext();
        }

        public ActionResult Index() {
            LogFunctions.Info("DbUtilsController.Index");
            return View();
        }

        public ActionResult Users() {
            LogFunctions.Info("DbUtilsController.Users");
            var users = from u in _dataContext.aspnet_Users
                        select u;
            string[] propertyNames = new string[] { "UserId", "UserName", "LastActivityDate" };
            List<string[]> propertiesList = new List<string[]>();
            foreach( aspnet_User user in users ) {
                string[] properties = new string[3];
                properties[0] = user.UserId.ToString();
                properties[1] = user.UserName;
                properties[2] = user.LastActivityDate.ToString();
                propertiesList.Add(properties);
            }
                        
            ViewData["propertyNames"] = propertyNames;
            ViewData["propertiesList"] = propertiesList;
            return View("AdHocQuery");
        }

        public ActionResult AdHocQueryForm() {
            LogFunctions.Info("DbUtils.AdHocQueryForm");
            return View();
        }

        public ActionResult AdHocQuery(string tableName, string query) {            
            LogFunctions.Info("DbUtils.AdHocQuery");
            Type table = null;
            switch( tableName ) {
                case "Classification":
                    table = typeof(Classification);
                    break;
                case "Feed":
                    table = typeof(Feed);
                    break;
                case "Item":
                    table = typeof(Item);
                    break;
                case "Prior":
                    table = typeof(Prior);
                    break;
                case "Probability":
                    table = typeof(Probability);
                    break;
                case "Stopword":
                    table = typeof(Stopword);
                    break;
                case "UserFeed":
                    table = typeof(UserFeed);
                    break;
                case "UserItem":
                    table = typeof(UserItem);
                    break;
                case "UserVocabulary":
                    table = typeof(UserVocabulary);
                    break;
                case "UserHistory":
                    table = typeof(UserHistory);
                    break;
                case "Vocabulary":
                    table = typeof(Vocabulary);
                    break;
                case "Aggregate":
                    table = typeof(Aggregate);
                    break;
            }
            PropertyInfo[] props = table.GetProperties();
            string[] propertyNames = new string[props.Length];
            for( int i = 0; i < propertyNames.Length; i++ ) {
                propertyNames[i] = props[i].Name;
            }
            var results = _dataContext.ExecuteQuery(table, query);
            List<string[]> propertiesList = new List<string[]>();
            foreach( object o in results ) {
                string[] properties = new string[props.Length];
                for( int i = 0; i < props.Length; i++ ) {
                    properties[i] = props[i].GetValue(o, null).ToString();
                }
                propertiesList.Add(properties);
            }
            
            ViewData["propertyNames"] = propertyNames;
            ViewData["propertiesList"] = propertiesList;
            return View();
        }

        public ActionResult PrepDb() {
            LogFunctions.Info("DbUtilsController.PrepDb");

            DeleteSnapdragon();

            Classification interesting = new Classification {
                Label = "interesting"
            };
            Classification uninteresting = new Classification {
                Label = "uninteresting"
            };
            Classification unknown = new Classification {
                Label = "unknown"
            };
            _dataContext.Classifications.InsertOnSubmit(interesting);
            _dataContext.Classifications.InsertOnSubmit(uninteresting);
            _dataContext.Classifications.InsertOnSubmit(unknown);
            _dataContext.SubmitChanges();

            AddStopwords();
            return View("Index");
        }

        public ActionResult Logs(int days) {
            LogFunctions.Info("DbUtils.ViewLogs");
            
            var logs = from l in _dataContext.Logs
                       where l.Timestamp >= DateTime.Now.ToUniversalTime().AddDays(-1 * days).Date
                       && !l.FormattedMessage.Contains("Empty date value")
                       && !l.FormattedMessage.Contains("Missing item published")
                       select l.FormattedMessage;
            ViewData["logs"] = logs;
            return View();
        }

        public ActionResult CleanUser(string userid) {
            Guid userId = new Guid(userid);
            
            // DELETE FROM [UserItem]
            var useritems = from ui in _dataContext.UserItems
                            where ui.UserId == userId
                            select ui;
            _dataContext.UserItems.DeleteAllOnSubmit(useritems);            

            // DELETE FROM [UserFeed]
            var userfeeds = from uf in _dataContext.UserFeeds
                            where uf.UserId == userId
                            select uf;
            _dataContext.UserFeeds.DeleteAllOnSubmit(userfeeds);
            
            // DELETE FROM [Probability]
            var probs = from p in _dataContext.Probabilities
                        where p.UserVocabulary.UserId == userId
                        select p;
            _dataContext.Probabilities.DeleteAllOnSubmit(probs);

            // DELETE FROM [UserVocabulary]
            var uservocabs = from uv in _dataContext.UserVocabularies
                             where uv.UserId == userId
                             select uv;
            _dataContext.UserVocabularies.DeleteAllOnSubmit(uservocabs);
            
            // DELETE FROM [Prior]
            var priors = from pr in _dataContext.Priors
                         where pr.UserId == userId
                         select pr;
            _dataContext.Priors.DeleteAllOnSubmit(priors);

            // DELETE FROM [UserHistory]
            var uhistory = from h in _dataContext.UserHistories
                           where h.UserId == userId
                           select h;
            _dataContext.UserHistories.DeleteAllOnSubmit(uhistory);

            _dataContext.SubmitChanges();

            // DELETE unused FEEDS and ITEMS
            //var userfeedids = from uf in _dataContext.UserFeeds
            //                  select uf.FeedId;
            //int[] userFeedIds = userfeedids.ToArray();
            
            //var vfeeds = from f in _dataContext.Feeds
            //             select f;
            //Feed[] afeeds = vfeeds.ToArray();
            //Dictionary<int, Feed> feeds = new Dictionary<int, Feed>();
            //foreach( Feed f in afeeds ) {
            //    feeds.Add(f.Id, f);
            //}

            //int[] feedIds = feeds.Keys.ToArray();
            //int[] keys = new int[feedIds.Max()];
            //List<int> tbdFeedIds = new List<int>();
            //keys.Initialize();
            //foreach( int feedId in feedIds ) {
            //    keys[feedId] = 1;
            //}
            //foreach( int userFeedId in userFeedIds ) {
            //    keys[userFeedId] = 2;
            //}
            //for( int i = 0; i < keys.Length; i++ ) {
            //    if( keys[i] == 1 ) tbdFeedIds.Add(i);
            //}

            //foreach( int tbdFeedId in tbdFeedIds ) {
            //    var items = from i in _dataContext.Items
            //                where i.FeedId == tbdFeedId
            //                select i;
            //    _dataContext.Items.DeleteAllOnSubmit(items);
            //    _dataContext.Feeds.DeleteOnSubmit(feeds[tbdFeedId]);
            //}
            //_dataContext.SubmitChanges();
            return View("Index");
        }

        public void DeleteSnapdragon() {
            // DELETE FROM [UserItem]
            var useritems = from ui in _dataContext.UserItems
                            select ui;
            _dataContext.UserItems.DeleteAllOnSubmit(useritems);

            // DELETE FROM [Item]
            var items = from i in _dataContext.Items
                        select i;
            _dataContext.Items.DeleteAllOnSubmit(items);

            // DELETE FROM [UserFeed]
            var userfeeds = from uf in _dataContext.UserFeeds
                            select uf;
            _dataContext.UserFeeds.DeleteAllOnSubmit(userfeeds);

            // DELETE FROM [Feed]
            var feeds = from f in _dataContext.Feeds
                        select f;
            _dataContext.Feeds.DeleteAllOnSubmit(feeds);

            // DELETE FROM [Stopword]
            var stopwords = from sw in _dataContext.Stopwords
                            select sw;
            _dataContext.Stopwords.DeleteAllOnSubmit(stopwords);

            // DELETE FROM [Probability]
            var probs = from p in _dataContext.Probabilities
                        select p;
            _dataContext.Probabilities.DeleteAllOnSubmit(probs);

            // DELETE FROM [UserVocabulary]
            var uservocabs = from uv in _dataContext.UserVocabularies
                             select uv;
            _dataContext.UserVocabularies.DeleteAllOnSubmit(uservocabs);

            // DELETE FROM [Vocabulary]
            var vocabs = from v in _dataContext.Vocabularies
                         select v;
            _dataContext.Vocabularies.DeleteAllOnSubmit(vocabs);

            // DELETE FROM [Prior]
            var priors = from pr in _dataContext.Priors
                         select pr;
            _dataContext.Priors.DeleteAllOnSubmit(priors);

            // DELETE FROM [Classification]
            var classes = from c in _dataContext.Classifications
                          select c;
            _dataContext.Classifications.DeleteAllOnSubmit(classes);

            _dataContext.SubmitChanges();
        }

        public void AddStopwords() {
            foreach( string stopword in stopwords ) {
                Stopword sw = new Stopword {
                    Word = stopword
                };
                _dataContext.Stopwords.InsertOnSubmit(sw);
            }
            _dataContext.SubmitChanges();
            
        }

        private string[] stopwords = new string[] {                
                "a",
                "able",
                "about",
                "above",
                "according",
                "accordingly",
                "across",
                "actually",
                "after",
                "afterwards",
                "again",
                "against",
                "all",
                "allow",
                "allows",
                "almost",
                "alone",
                "along",
                "already",
                "also",
                "although",
                "always",
                "am",
                "among",
                "amongst",
                "an",
                "and",
                "another",
                "any",
                "anybody",
                "anyhow",
                "anyone",
                "anything",
                "anyway",
                "anyways",
                "anywhere",
                "apart",
                "appear",
                "appreciate",
                "appropriate",
                "are",
                "around",
                "as",
                "aside",
                "ask",
                "asking",
                "associated",
                "at",
                "available",
                "away",
                "awfully",
                "b",
                "be",
                "became",
                "because",
                "become",
                "becomes",
                "becoming",
                "been",
                "before",
                "beforehand",
                "behind",
                "being",
                "believe",
                "below",
                "beside",
                "besides",
                "best",
                "better",
                "between",
                "beyond",
                "both",
                "brief",
                "but",
                "by",
                "c",
                "came",
                "can",
                "cannot",
                "cant",
                "cause",
                "causes",
                "certain",
                "certainly",
                "changes",
                "clearly",
                "co",
                "com",
                "come",
                "comes",
                "concerning",
                "consequently",
                "consider",
                "considering",
                "contain",
                "containing",
                "contains",
                "corresponding",
                "could",
                "course",
                "currently",
                "d",
                "definitely",
                "described",
                "despite",
                "did",
                "different",
                "do",
                "does",
                "doing",
                "done",
                "down",
                "downwards",
                "during",
                "e",
                "each",
                "edu",
                "eg",
                "eight",
                "either",
                "else",
                "elsewhere",
                "enough",
                "entirely",
                "especially",
                "et",
                "etc",
                "even",
                "ever",
                "every",
                "everybody",
                "everyone",
                "everything",
                "everywhere",
                "ex",
                "exactly",
                "example",
                "except",
                "f",
                "far",
                "few",
                "fifth",
                "first",
                "five",
                "followed",
                "following",
                "follows",
                "for",
                "former",
                "formerly",
                "forth",
                "four",
                "from",
                "further",
                "furthermore",
                "g",
                "get",
                "gets",
                "getting",
                "given",
                "gives",
                "go",
                "goes",
                "going",
                "gone",
                "got",
                "gotten",
                "greetings",
                "h",
                "had",
                "happens",
                "hardly",
                "has",
                "have",
                "having",
                "he",
                "hello",
                "help",
                "hence",
                "her",
                "here",
                "hereafter",
                "hereby",
                "herein",
                "hereupon",
                "hers",
                "herself",
                "hi",
                "him",
                "himself",
                "his",
                "hither",
                "hopefully",
                "how",
                "howbeit",
                "however",
                "i",
                "ie",
                "if",
                "ignored",
                "immediate",
                "in",
                "inasmuch",
                "inc",
                "indeed",
                "indicate",
                "indicated",
                "indicates",
                "inner",
                "insofar",
                "instead",
                "into",
                "inward",
                "is",
                "it",
                "its",
                "itself",
                "j",
                "just",
                "k",
                "keep",
                "keeps",
                "kept",
                "know",
                "knows",
                "known",
                "l",
                "last",
                "lately",
                "later",
                "latter",
                "latterly",
                "least",
                "less",
                "lest",
                "let",
                "like",
                "liked",
                "likely",
                "little",
                "ll",
                "look",
                "looking",
                "looks",
                "ltd",
                "m",
                "mainly",
                "many",
                "may",
                "maybe",
                "me",
                "mean",
                "meanwhile",
                "merely",
                "might",
                "more",
                "moreover",
                "most",
                "mostly",
                "much",
                "must",
                "my",
                "myself",
                "n",
                "name",
                "namely",
                "nd",
                "near",
                "nearly",
                "necessary",
                "need",
                "needs",
                "neither",
                "never",
                "nevertheless",
                "new",
                "next",
                "nine",
                "no",
                "nobody",
                "non",
                "none",
                "noone",
                "nor",
                "normally",
                "not",
                "nothing",
                "novel",
                "now",
                "nowhere",
                "o",
                "obviously",
                "of",
                "off",
                "often",
                "oh",
                "ok",
                "okay",
                "old",
                "on",
                "once",
                "one",
                "ones",
                "only",
                "onto",
                "or",
                "other",
                "others",
                "otherwise",
                "ought",
                "our",
                "ours",
                "ourselves",
                "out",
                "outside",
                "over",
                "overall",
                "own",
                "p",
                "particular",
                "particularly",
                "per",
                "perhaps",
                "placed",
                "please",
                "plus",
                "possible",
                "presumably",
                "probably",
                "provides",
                "q",
                "que",
                "quite",
                "qv",
                "r",
                "rather",
                "rd",
                "re",
                "really",
                "reasonably",
                "regarding",
                "regardless",
                "regards",
                "relatively",
                "respectively",
                "right",
                "s",
                "said",
                "same",
                "saw",
                "say",
                "saying",
                "says",
                "second",
                "secondly",
                "see",
                "seeing",
                "seem",
                "seemed",
                "seeming",
                "seems",
                "seen",
                "self",
                "selves",
                "sensible",
                "sent",
                "serious",
                "seriously",
                "seven",
                "several",
                "shall",
                "she",
                "should",
                "since",
                "six",
                "so",
                "some",
                "somebody",
                "somehow",
                "someone",
                "something",
                "sometime",
                "sometimes",
                "somewhat",
                "somewhere",
                "soon",
                "sorry",
                "specified",
                "specify",
                "specifying",
                "still",
                "sub",
                "such",
                "sup",
                "sure",
                "t",
                "take",
                "taken",
                "tell",
                "tends",
                "th",
                "than",
                "thank",
                "thanks",
                "thanx",
                "that",
                "thats",
                "the",
                "their",
                "theirs",
                "them",
                "themselves",
                "then",
                "thence",
                "there",
                "thereafter",
                "thereby",
                "therefore",
                "therein",
                "theres",
                "thereupon",
                "these",
                "they",
                "think",
                "third",
                "this",
                "thorough",
                "thoroughly",
                "those",
                "though",
                "three",
                "through",
                "throughout",
                "thru",
                "thus",
                "to",
                "together",
                "too",
                "took",
                "toward",
                "towards",
                "tried",
                "tries",
                "truly",
                "try",
                "trying",
                "twice",
                "two",
                "u",
                "un",
                "under",
                "unfortunately",
                "unless",
                "unlikely",
                "until",
                "unto",
                "up",
                "upon",
                "us",
                "use",
                "used",
                "useful",
                "uses",
                "using",
                "usually",
                "uucp",
                "v",
                "value",
                "various",
                "ve",
                "very",
                "via",
                "viz",
                "vs",
                "w",
                "want",
                "wants",
                "was",
                "way",
                "we",
                "welcome",
                "well",
                "went",
                "were",
                "what",
                "whatever",
                "when",
                "whence",
                "whenever",
                "where",
                "whereafter",
                "whereas",
                "whereby",
                "wherein",
                "whereupon",
                "wherever",
                "whether",
                "which",
                "while",
                "whither",
                "who",
                "whoever",
                "whole",
                "whom",
                "whose",
                "why",
                "will",
                "willing",
                "wish",
                "with",
                "within",
                "without",
                "wonder",
                "would",
                "would",
                "x",
                "y",
                "yes",
                "yet",
                "you",
                "your",
                "yours",
                "yourself",
                "yourselves",
                "z",
                "zero"
            };


    }
}
