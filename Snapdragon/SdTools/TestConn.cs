using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;

namespace SdTools
{
    public class TestConn
    {
        public static void RunMain(string connStr) {
            string connToTest = ConfigurationManager.ConnectionStrings[connStr].ConnectionString;
            string query = "select * from Feed";
            SqlConnection conn = new SqlConnection(connToTest);
            conn.Open();
            SqlCommand cmd = new SqlCommand(query, conn);
            SqlDataReader reader = cmd.ExecuteReader();
            while ( reader.Read() ) {
                for ( int i = 0; i < reader.FieldCount; i++ ) {
                    Console.WriteLine("{0}: {1}", reader.GetName(i), reader[i].ToString());
                }
            }
        }
    }
}
