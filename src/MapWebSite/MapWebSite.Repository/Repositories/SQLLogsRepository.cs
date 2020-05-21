using MapWebSite.Core.Database;
using MapWebSite.Core.Database.Logs;
using MapWebSite.SQLAccess;
using SqlKata;
using SqlKata.Compilers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Repository
{
    public class SQLLogsRepository : SQLBaseRepository, ILogsRepository
    {

        private static readonly string logTable = "ApplicationLogs";

        public void LogError(string message, string stacktrace)
        {
            log(message, stacktrace, LogTrigger.Other, LogType.Error);
        }

        public void LogError(string message, string stacktrace, LogTrigger trigger)
        {
            log(message, stacktrace, trigger, LogType.Error);
        }

        public void LogError(Exception exception)
        {
            log(exception.Message, exception.StackTrace, LogTrigger.Other, LogType.Error);
        }

        public void LogError(Exception exception, LogTrigger trigger)
        {
            log(exception.Message, exception.StackTrace, trigger, LogType.Error);
        }

        public void LogInfo(string message)
        {
            log(message, null, LogTrigger.Other, LogType.Info);
        }

        public void LogInfo(string message, LogTrigger trigger)
        {
            log(message, null, trigger, LogType.Info);
        }

        public void LogWarning(string message)
        {
            log(message, null, LogTrigger.Other, LogType.Warning);
        }

        public void LogWarning(string message, LogTrigger trigger)
        {
            log(message, null, trigger, LogType.Warning);
        }



        #region Private

        private void log(string message, string stacktrace, LogTrigger logTrigger, LogType logType)
        {
            try
            {
                var query = new Query(logTable)
                                .AsInsert(new
                                {
                                    stacktrace,
                                    message,
                                    log_type = logType.ToString(),
                                    log_trigger = logTrigger.ToString(),
                                    creation_date = DateTime.Now,
                                });

                SqlResult queryResult = new SqlServerCompiler().Compile(query);

                SqlExecutionInstance.ExecuteNonQuery(new SqlCommand(queryResult.Sql),

                    new SqlParameter[] {
                    new SqlParameter("p0", queryResult.Bindings[0]),
                    new SqlParameter("p1", queryResult.Bindings[1]),
                    new SqlParameter("p2", queryResult.Bindings[2]),
                    new SqlParameter("p3", queryResult.Bindings[3]),
                    new SqlParameter("p4", queryResult.Bindings[4])
                    },
                    new SqlConnection(this.connectionString));
            }
            catch { }
        }

        #endregion
    
    }
}
