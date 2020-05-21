using MapWebSite.Core.Database.Logs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MapWebSite.Core.Database
{
    namespace Logs
    {
        public enum LogTrigger
        {
            Core,
            CoreModule,
            Controllers,
            DataAccess,
            DataServices,
            Domain,

            Other
        }

        public enum LogType
        {
            Info,
            Warning,
            Error                
        }
    }


    public interface ILogsRepository
    {
        /// <summary>
        /// Log a info message into the repository
        /// </summary>
        /// <param name="message">The message which must be logged</param>     
        void LogInfo(string message);

        /// <summary>
        /// Log a info message into the repository
        /// </summary>
        /// <param name="message">The message which must be logged</param>
        /// <param name="trigger">The module or entity which triggered the log</param>
        void LogInfo(string message, LogTrigger trigger);

        /// <summary>
        /// Log a warning message into the repository
        /// </summary>
        /// <param name="message">The message which must be logged</param>     
        void LogWarning(string message);

        /// <summary>
        /// Log a warning message into the repository
        /// </summary>
        /// <param name="message">The message which must be logged</param>
        /// <param name="trigger">The module or entity which triggered the log</param>
        void LogWarning(string message, LogTrigger trigger);

        /// <summary>
        /// Log a error message into the repository
        /// </summary>
        /// <param name="message">The message which must be logged</param>  
        void LogError(string message, string stacktrace);
     
        /// <summary>
        /// Log a error message into the repository
        /// </summary>
        /// <param name="message">The message which must be logged</param>
        /// <param name="trigger">The module or entity which triggered the log</param>
        void LogError(string message, string stacktrace, LogTrigger trigger);

        /// <summary>
        /// Log a error message from an exception into the repository
        /// </summary>
        /// <param name="exception">The exception which must be loged</param>
        void LogError(Exception exception);
        
        /// <summary>
        /// Log a error message from an exception into the repository
        /// </summary>
        /// <param name="exception">The exception which must be loged</param>
        /// <param name="trigger">The module or entity which tirggered the log</param>
        void LogError(Exception exception, LogTrigger trigger);


    }
}
