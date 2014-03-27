using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avilay.Utils;
using System.Collections.Specialized;
using System.Configuration;

namespace SdTools
{
    public class CopyDb
    {
        public static void RunMain(string fromDb, string toDb) {
            string fromDbConnStr = ConfigurationManager.ConnectionStrings[fromDb].ConnectionString;
            string toDbConnStr = ConfigurationManager.ConnectionStrings[toDb].ConnectionString;
            NameValueCollection tableNames = ConfigurationManager.GetSection("tables") as NameValueCollection;
            string[] tables = new string[tableNames.Count];
            int i = 0;
            foreach ( string key in tableNames.AllKeys.OrderBy(key => Int32.Parse(key)) ) {
                tables[i++] = tableNames[key];
            }
            DbUtils.CopyTables(fromDbConnStr, toDbConnStr, tables);
            Console.WriteLine("Copy complete");
        }
    }
}
