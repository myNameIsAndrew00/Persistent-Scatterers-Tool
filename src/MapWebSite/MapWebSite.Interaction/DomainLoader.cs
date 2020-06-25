using MapWebSite.Core.DataPoints;
using MapWebSite.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Domain
{
    /// <summary>
    /// Use this class to inject domain dependencies
    /// </summary>
    public static class DomainLoader
    {
        public static void Load()
        {
          
            /*Initialise the domain*/
            DomainInstance.Initialise();

            /*Inject the logs repository in core containers*/
            Core.CoreContainers.LogsRepository = new SQLLogsRepository();

            /*Inject the users repository in core containers*/
            Core.CoreContainers.UsersRepository = new SQLUserRepository();

            /*Inject the main datapoints repository in core containers*/
            Core.CoreContainers.DataPointsRepository = CassandraDataPointsRepository.Instance;

            Core.CoreContainers.DataPointsRegionSource = new PowerOfTwoRegionsSource();


            Core.CoreContainers.LogsRepository.LogInfo("Domain loaded", Core.Database.Logs.LogTrigger.Domain);

        }
    }
}
