using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Feeder.Services;
using Avilay.Syndication;
using System.Xml;
using System.Net;
using Feeder.Models;
using Avilay.Utils.Logging;
using Feeder.Controllers;

namespace Feeder.Views.Feed
{
    public partial class AddPopUp : ViewPage
    {
        public Dictionary<Uri, string> failures;
        protected void Page_Load(object sender, EventArgs e) {
            failures = new Dictionary<Uri, string>();
            
            Dictionary<Uri, AddFailureState> failStates = ViewData["failStates"] as Dictionary<Uri, AddFailureState>;
            foreach( Uri uri in failStates.Keys ) {
                AddFailureState state = failStates[uri];
                string reason = "";
                switch( state ) {
                    case AddFailureState.BADLY_FORMED_FEED:
                        reason = "This feed is badly formed. It can only be added after the publisher corrects the error";
                        break;
                    case AddFailureState.INACCESSIBLE_URL:
                        reason = "This URL is not accessible";
                        break;
                    case AddFailureState.INVALID_XML_FEED:
                        reason = "This feed has some parts missing. It can only be added after the publisher corrects the error";
                        break;
                    case AddFailureState.NON_FEED_MARKUP:
                        reason = "This is not a feed";
                        break;
                    case AddFailureState.NOT_SUPPORTED_FEED:
                        reason = "This feed format is not supported";
                        break;
                    case AddFailureState.OTHER:
                        reason = "This is not a feed";
                        break;
                }
                failures.Add(uri, reason);
            }
        }

    }
}
