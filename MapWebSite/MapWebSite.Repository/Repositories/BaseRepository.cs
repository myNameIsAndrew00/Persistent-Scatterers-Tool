using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Repository
{
    public class BaseRepository
    {
        protected string connectionString = null;

        public BaseRepository()
        {
            this.connectionString = ConfigurationManager.ConnectionStrings["PersistantScatterersDatabase"].ToString();
        }

    }
}
