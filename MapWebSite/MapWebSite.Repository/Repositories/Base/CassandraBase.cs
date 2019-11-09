using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace MapWebSite.Repository
{
    public class CassandraBaseRepository
    {
        protected string server = null;
        protected string keyspace = null;

        public CassandraBaseRepository()
        {
           
            server = ConfigurationManager.AppSettings["CassandraServer"];
            keyspace = ConfigurationManager.AppSettings["CassandraKeyspace"];
        }
    }
}
