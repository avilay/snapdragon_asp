using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avilay.Utils;
using System.Configuration;
using System.Collections.Specialized;
using System.Data.SqlClient;
using Avilay.Utils.Logging;
using System.IO;

/*
 * Use this executable for crawling, learning, cleaning out the db, monitoring, copying the production db, etc..
 * DbCopy belongs to Avilay.Utils as it is a generic copy utility that I can invoke from here
 */ 
namespace SdTools
{
    class Program
    {
        static StreamWriter _writer = null;
 
        static void Main(string[] args) {
            string logFilePath = Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings["logFilePath"]);
            _writer = new StreamWriter(File.Open(logFilePath, FileMode.Append));
            _writer.AutoFlush = true;
            LogFunctions.SetFunctions(Debug,
                                      Info,
                                      Warning,
                                      ErrorLog,
                                      ExceptionWarningLog,
                                      ExceptionWarningLog);

            string logLevel = ConfigurationManager.AppSettings["logLevel"];
            switch ( logLevel ) {
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

            string validOpts = "y:h::npf:t:m:c:l:u:";
            Dictionary<string, string> longOpts = new Dictionary<string, string>();
            longOpts.Add("testconn", "y");
            longOpts.Add("scratch", "h");
            longOpts.Add("cleandb", "n");
            longOpts.Add("copydb", "p");
            longOpts.Add("monitor", "m");
            longOpts.Add("crawl", "c");
            longOpts.Add("learn", "l");
            longOpts.Add("from", "f");
            longOpts.Add("to", "t");
            longOpts.Add("dbusage", "u");
            Dictionary<string, string> options;
            GetOpts.Parse(validOpts, args, out options, longOpts);            
            if (options.ContainsKey("y")) {
                TestConn.RunMain(options["y"]);
            }
            if ( options.ContainsKey("h") ) {
                Scratch.RunMain(options["h"]);
            }
            if ( options.ContainsKey("n") && options.ContainsKey("f") && options.ContainsKey("t") ) {
                CleanSdDb.RunMain(options["f"], options["t"]);
            }
            if ( options.ContainsKey("m") ) {
                Monitor.RunMain(options["m"]);
            }
            if ( options.ContainsKey("c") ) {
                Crawl.RunMain(options["c"]);
            }
            if ( options.ContainsKey("l") ) {
                Learn.RunMain(options["l"]);
            }
            if (options.ContainsKey("u")) {
                DbUsage.RunMain(options["u"]);
            }
            if ( options.ContainsKey("p") && options.ContainsKey("f") && options.ContainsKey("t") ) {
                CopyDb.RunMain(options["f"], options["t"]);
            }
            _writer.Flush();
            _writer.Close();
        }

        static void Usage() {
            Console.WriteLine(".\\SdTools -testconn [conn name from config]");
        }

        private static string FormatMsg(string severity, string msg) {
            StringBuilder ret = new StringBuilder();
            ret.AppendLine("-------------------------------------");
            ret.AppendLine(DateTime.Now + ": " + severity + ": " + msg);
            ret.AppendLine("-------------------------------------");
            return ret.ToString();            
        }

        private static string ExceptionToString(Exception e) {
            string msg = e.GetType().FullName + " was thrown.\n";
            while ( e != null ) {
                msg += e.Message + "\n";
                msg += e.StackTrace + "\n";
                e = e.InnerException;
            }
            return msg;
        }

        public static void Debug(string msg) {
            _writer.WriteLine(FormatMsg("DEBUG", msg));            
        }

        public static void Warning(string msg) {
            _writer.WriteLine(FormatMsg("WARNING", msg));
        }

        public static void Info(string msg) {
            _writer.WriteLine(FormatMsg("INFO", msg));
        }

        public static void ErrorLog(string msg) {
            _writer.WriteLine(FormatMsg("ERROR", msg));
        }

        public static void ExceptionErrorLog(Exception e, string msg) {
            msg = msg + "\n" + ExceptionToString(e);
            _writer.WriteLine(FormatMsg("ERROR", msg));
        }

        public static void ExceptionWarningLog(Exception e, string msg) {
            msg = msg + "\n" + ExceptionToString(e);
            _writer.WriteLine(FormatMsg("WARNING", msg));
        }

        
    }
}
