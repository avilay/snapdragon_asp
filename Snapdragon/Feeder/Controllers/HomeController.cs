using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Net;
using Avilay.Utils.Logging;

namespace Feeder.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index() {
            LogFunctions.Info("HomeController.Index");

            ViewData["Title"] = "Home Page";
            ViewData["Message"] = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult About() {
            LogFunctions.Info("HomeController.About");

            ViewData["Title"] = "About Page";

            return View();
        }

        public ActionResult ExamineRequest() {
            return View();
        }

        [HandleError]
        public ActionResult TestExceptionHandler() {
            LogFunctions.Info("HomeController.TestExceptionHandler");

            WebRequest request = WebRequest.Create("http://avilayparekh.com");
            request.GetResponse();
            return View();
        }

        [HandleError]
        public ActionResult TestLogging() {
            LogFunctions.Info("HomeController.TestLogging");            
            LogFunctions.Debug("sample log message");
            return View();
        }
    }
}
