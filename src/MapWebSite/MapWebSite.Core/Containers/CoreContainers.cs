using MapWebSite.Core.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace MapWebSite.Core
{
    /// <summary>
    /// Use this class to handle 'core' containers
    /// </summary>
    public static class CoreContainers
    {
        /// <summary>
        /// Repository used to log data across application
        /// </summary>
        public static ILogsRepository LogsRepository { get; set; }
    }

}
