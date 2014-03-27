using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Configuration;
using Avilay.Utils.Logging;

namespace SdTools
{
    public class Crawl
    {
        public static void RunMain(string domain) {
            string daemonClientKey = ConfigurationManager.AppSettings["daemonClientKey"];
            string url = string.Format("http://{0}/Daemon/CrawlAll/{1}", domain, daemonClientKey);
            using ( WebClient client = new WebClient() ) {
                client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                string content = client.DownloadString(url);
            }
            LogFunctions.Info("Started crawl with " + url);
        }
    }
}
