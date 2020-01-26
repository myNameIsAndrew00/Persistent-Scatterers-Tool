using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.SQLAccess
{
    /// <summary>
    /// use this class to log errors or exceptions into the database.
    /// </summary>
    public class Logger
    {
        // set the default connection here
        // TO DO: find a way to put this connectionString in a config file
        private static SqlConnection sqlConnection = new SqlConnection("");
        public static void Log(string ErrorMessage, string StackTrace, DateTime Date)
        {
            // ***** TO DO: modify and here
            try
            {
                SqlExecutionInstance.ExecuteNonQuery(new SqlCommand(), new SqlParameter[] { }, sqlConnection);
            }
            catch { }
        }
    }
}

