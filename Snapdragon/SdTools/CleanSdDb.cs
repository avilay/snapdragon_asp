using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Avilay.Utils.Logging;
using Avilay.Utils;
using System.Configuration;
using System.Collections.Specialized;
using System.Data.SqlClient;

namespace SdTools
{
    public class CleanSdDb
    {
        
        public static void RunMain(string fromDb, string toDb) {            
            //copy to local db
            CopyDb.RunMain(fromDb, toDb);
            LogFunctions.Info("Copy to local db complete");

            //backup local db
            string backupScriptPath = Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings["backupScriptPath"])  ;
            string sqlserverName = Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings["sqlserverName"]);
            string args = string.Format("-E -W -S {0} -i \"{1}\"", sqlserverName, backupScriptPath);
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "SQLCMD.EXE";
            p.StartInfo.Arguments = args;
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            if ( p.ExitCode != 0 ) {
                LogFunctions.Error("Backup did not succeed. Not proceeding with clean up. " + output);
                Environment.Exit(-1);
            }
            LogFunctions.Info("Backup of local db complete. " + output);

            //delete prod db
            string fromDbConnStr = ConfigurationManager.ConnectionStrings[fromDb].ConnectionString;
            string toDbConnStr = ConfigurationManager.ConnectionStrings[toDb].ConnectionString;
            NameValueCollection tableNames = ConfigurationManager.GetSection("tables") as NameValueCollection;
            string[] tables = new string[tableNames.Count];
            int i = 0;
            foreach ( string key in tableNames.AllKeys.OrderBy(key => Int32.Parse(key)) ) {
                tables[i++] = tableNames[key];
            }
            DbUtils.DbUsage dbUsage = new DbUtils.DbUsage(tables);
            dbUsage.Calculate(fromDbConnStr);
            LogFunctions.Info("BEFORE DELETE: " + dbUsage.ToString());
            SqlConnection conn = null;
            conn = new SqlConnection(fromDbConnStr);
            conn.Open();
            
            string daysToKeepItems = ConfigurationManager.AppSettings["daysToKeepItems"];
            string spCmd = "sp_clean_snapdragon " + daysToKeepItems.Trim();
            int rowsAffected = 0;
            using ( SqlCommand cmd = new SqlCommand(spCmd, conn) ) {
                rowsAffected = cmd.ExecuteNonQuery();
            }
            LogFunctions.Info(string.Format("Deleted {0} number of rows from Snapdragon", rowsAffected));

            string daysToKeepLogs = ConfigurationManager.AppSettings["daysToKeepLogs"];
            spCmd = "sp_clean_snapdragonlogs " + daysToKeepLogs.Trim();
            rowsAffected = 0;
            using (SqlCommand cmd = new SqlCommand(spCmd, conn)) {
                rowsAffected = cmd.ExecuteNonQuery();
            }
            LogFunctions.Info(string.Format("Deleted {0} number of rows from Logs", rowsAffected));

            conn.Close();

            dbUsage.Calculate(fromDbConnStr);
            LogFunctions.Info("AFTER DELETE: " + dbUsage.ToString());
            double totalAfterDelete = dbUsage.Total;
            if ( totalAfterDelete > 55000 ) {
                Alarm.Ring("Total space used is " + totalAfterDelete + " KB");
            }
        }
    }
}
