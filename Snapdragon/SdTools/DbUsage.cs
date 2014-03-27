using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avilay.Utils;
using System.Collections.Specialized;
using System.Configuration;

namespace SdTools
{
    public class DbUsage
    {
        public static void RunMain(string dbName) {
            NameValueCollection tableNames = ConfigurationManager.GetSection("tables") as NameValueCollection;
            string[] tables = new string[tableNames.Count];
            int i = 0;
            foreach ( string key in tableNames.AllKeys.OrderBy(key => Int32.Parse(key)) ) {
                tables[i++] = tableNames[key];
            }
            string connStr = ConfigurationManager.ConnectionStrings[dbName].ConnectionString;
            
            DbUtils.DbUsage dbusage = new DbUtils.DbUsage(tables);
            dbusage.Calculate(connStr);
            Console.WriteLine(dbusage);
        }
    }
}
