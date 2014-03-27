using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Avilay.Utils.Logging;

namespace Feeder.Controllers
{
    public class HelpController : Controller
    {
        //
        // GET: /Help/

        public ActionResult Index() {
            LogFunctions.Info("HelpController.Index");
            return View();
        }

        public ActionResult AddButton() {
            LogFunctions.Info("HelpController.AddButton");
            return View("Index");
        }

    }
}
