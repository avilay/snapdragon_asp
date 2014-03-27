using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Avilay.Utils.Logging;

namespace Feeder.Views.Shared
{
    public partial class Error : ViewPage<HandleErrorInfo>
    {
        protected void Page_Load(object sender, EventArgs e) {
            //HandleErrorInfo errorInfo = ViewData.Model;
            //msg = errorInfo.ControllerName + "/" + errorInfo.ActionName;
            //msg += MvcApplication.ExceptionToString(errorInfo.Exception);
            try {
                HandleErrorInfo errorInfo = ViewData.Model;
                string log = errorInfo.ControllerName + "/" + errorInfo.ActionName;
                LogFunctions.Error(log, errorInfo.Exception);
                msg = errorInfo.Exception.Message;
            }
            catch( Exception ex ) {
                msg = "CRITICAL!! Could not retrieve or log error message<br />" + MvcApplication.ExceptionToString(ex);
            }
        }
    }
}
