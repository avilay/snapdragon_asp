using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.WebServices.Data;
using Avilay.Utils.Logging;

namespace SdTools
{
    public class Alarm
    {
        public static void Ring(string msg) {
            ExchangeService service = new ExchangeService();
            service.UseDefaultCredentials = true;
            service.AutodiscoverUrl("avilayp@microsoft.com");
            EmailMessage message = new EmailMessage(service);
            message.Subject = "ALARM from snapdragon";
            message.Body = msg;
            message.ToRecipients.Add("avilay@gmail.com");
            message.SendAndSaveCopy();

            LogFunctions.Info(msg);
        }
    }
}
