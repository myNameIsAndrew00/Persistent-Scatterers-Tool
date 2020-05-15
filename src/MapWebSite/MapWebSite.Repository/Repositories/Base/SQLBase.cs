using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Repository
{
    public class SQLBaseRepository
    {
        protected string connectionString = null;

        public SQLBaseRepository()
        {
            this.connectionString = ConfigurationManager.ConnectionStrings["PersistantScatterersDatabase"].ToString();
        }

        public SQLBaseRepository(string configurationStringKey)
        {
            this.connectionString = ConfigurationManager.ConnectionStrings[configurationStringKey].ToString();
        }

    }
}
