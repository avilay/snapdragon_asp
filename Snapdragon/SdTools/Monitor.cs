using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace SdTools
{
    public class Monitor
    {
        public static void RunMain(string dbName) {
            string connStr = ConfigurationManager.ConnectionStrings[dbName].ConnectionString;

            string query1 = "select count(*) as ErrorCount from Log where Severity = 'Error'";
            string query2 = "select count(*) as SpaceError from Log where Severity = 'Error' and Message LIKE '%Could not allocate space for object%'";
            SqlConnection conn = new SqlConnection(connStr);
            conn.Open();
            int errorCount = -1;
            int spaceErrorCount = -1;
            using ( SqlCommand cmd = new SqlCommand() ) {
                cmd.Connection = conn;

                cmd.CommandText = query1;
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                errorCount = (int)reader["ErrorCount"];
                reader.Close();

                cmd.CommandText = query2;
                reader = cmd.ExecuteReader();
                reader.Read();
                spaceErrorCount = (int)reader["SpaceError"];
                reader.Close();
            }
            if ( errorCount > 0 ) {
                Alarm.Ring(string.Format("There are {0} errors.", errorCount));
            }
            if ( spaceErrorCount > 0 ) {
                Alarm.Ring(string.Format("There are {0} space errors.", spaceErrorCount));
            }
            conn.Close();
        }
    }
}
