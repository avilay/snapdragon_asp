using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Avilay.Utils.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Diagnostics;
using System.Configuration;

namespace Feeder
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Admin",
                "Daemon/{action}/{key}",
                new { controller = "Daemon" }
            );

            routes.MapRoute(
                "Default",                                              // Route name
                "{controller}/{action}/{id}",                           // URL with parameters
                new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
            );

        }

        protected static string _requestId = null;

        protected void Application_Start() {
            RegisterRoutes(RouteTable.Routes);
            LogFunctions.SetFunctions(Debug, Info, Warning, ErrorLog, ExceptionWarningLog, ExceptionErrorLog);
            string logLevel = ConfigurationManager.AppSettings["logLevel"];
            switch (logLevel) {
                case "debug":
                    LogFunctions.SetLogLevel(LogLevel.DEBUG);
                    break;
                case "info":
                    LogFunctions.SetLogLevel(LogLevel.INFO);
                    break;
                case "warn":
                    LogFunctions.SetLogLevel(LogLevel.WARN);
                    break;
                case "error":
                    LogFunctions.SetLogLevel(LogLevel.ERROR);
                    break;
                default:
                    LogFunctions.SetLogLevel(LogLevel.WARN);
                    break;
            }
            LogFunctions.Info("Application starting");
        }

        protected void Application_BeginRequest() {
            Guid rid = Guid.NewGuid();
            _requestId = rid.ToString();
        }

        public static void Debug(string msg) {
            if (_requestId != null) {
                msg = "RID: " + _requestId + " " + msg;
            }
            else {
                msg = "RID: none " + msg;
            }
            Logger.Write(msg, "General", 0, 0, TraceEventType.Verbose);
        }

        static void Warning(string msg) {
            if (_requestId != null) {
                msg = "RID: " + _requestId + " " + msg;
            }
            else {
                msg = "RID: none " + msg;
            }
            Logger.Write(msg, "General", 0, 0, TraceEventType.Warning);
        }

        static void Info(string msg) {
            if (_requestId != null) {
                msg = "RID: " + _requestId + " " + msg;
            }
            else {
                msg = "RID: none " + msg;
            }
            Logger.Write(msg, "General", 0, 0, TraceEventType.Information);
        }

        static void ErrorLog(string msg) {
            if (_requestId != null) {
                msg = "RID: " + _requestId + " " + msg;
            }
            else {
                msg = "RID: none " + msg;
            }
            Logger.Write(msg, "General", 0, 0, TraceEventType.Error);
        }

        static void ExceptionErrorLog(Exception e, string msg) {
            if (_requestId != null) {
                msg = "RID: " + _requestId + " " + msg;
            }
            else {
                msg = "RID: none " + msg;
            }
            msg = msg + "\n" + ExceptionToString(e);
            Logger.Write(msg, "General", 0, 0, TraceEventType.Error);
        }

        public static string ExceptionToString(Exception e) {
            string errMsg = e.GetType().FullName + " was thrown.\n";
            while (e != null) {
                errMsg += e.Message + "\n";
                errMsg += e.StackTrace + "\n";
                e = e.InnerException;
            }
            return errMsg;
        }

        static void ExceptionWarningLog(Exception e, string msg) {
            if (_requestId != null) {
                msg = "RID: " + _requestId + " " + msg;
            }
            else {
                msg = "RID: none " + msg;
            }
            msg = msg + "\n" + ExceptionToString(e);
            Logger.Write(msg, "General", 0, 0, TraceEventType.Warning);
        }

    }
}