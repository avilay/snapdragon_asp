using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Feeder.Models;
using Feeder.Services;
using Avilay.Utils.Logging;

namespace Feeder.Controllers
{
    [HandleError]
    public class DaemonController : Controller
    {
        private IDaemonService _daemonSvc;

        public DaemonController() {
            _daemonSvc = new DaemonService();
        }

        public DaemonController(IDaemonService daemonSvc) {
            _daemonSvc = daemonSvc;
        }

        public ActionResult Index(string key) {
            LogFunctions.Info("DaemonController.Index");
            return View();
        }

        public string CrawlAll(string key) {
            LogFunctions.Info(string.Format("DaemonController.CrawlAll({0})", key));

            if( _daemonSvc.IsValid(key) ) {
                _daemonSvc.AsyncCrawlAndClassify();
                return "Started " + DateTime.Now.ToShortTimeString();
            }
            else {
                return "Unauthorized"; // TODO: return proper HTTP 404 error here
            }                        
        }

        public string LearnAll(string key) {
            LogFunctions.Info(string.Format("DaemonController.LearnAll({0})", key));

            if( _daemonSvc.IsValid(key) ) {
                _daemonSvc.AsyncLearn();
                return "Started " + DateTime.Now.ToShortTimeString();
            }
            else {
                return "Unauthorized"; // TODO: return proper HTTP 404 error here
            }
        }
    }
}
