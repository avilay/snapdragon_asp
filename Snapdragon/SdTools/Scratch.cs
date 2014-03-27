using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Diagnostics;
using Avilay.Utils;
using System.Web.Security;
using Avilay.Utils.Logging;

namespace SdTools
{
    

    public class Scratch
    {
        
        public static void RunMain(string numItems) {
            Console.WriteLine("Starting scratch");
            int totItems = Int32.Parse(numItems);
            List<string> items = new List<string>();
            for (int i = 0; i < totItems; i++) {
                items.Add("item number " + i.ToString());
            }

            double x = (double)items.Count / 10;
            int totPages = (int)Math.Ceiling(x);
            int[] start = new int[totPages];
            int[] end = new int[totPages];
            for ( int i = 0; i < totPages; i++ ) {
                start[i] = i * 10;
                end[i] = Math.Min(start[i] + 10, items.Count);
            }

            for ( int i = 0; i < totPages; i++ ) {
                Console.WriteLine("Page : " + i.ToString());
                for ( int j = start[i]; j < end[i]; j++ ) {
                    Console.WriteLine(items[j]);
                }
            }

        }
    }
}
