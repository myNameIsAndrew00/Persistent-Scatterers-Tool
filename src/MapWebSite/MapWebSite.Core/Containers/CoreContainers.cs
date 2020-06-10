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

        /// <summary>
        /// Repository used to handle users across aplication
        /// </summary>
        public static IUserRepository UsersRepository { get; set; }


        /// <summary>
        /// Repository used to handle data points across aplication
        /// </summary>
        public static IDataPointsRepository DataPointsRepository { get; set; }
        

    }

}
