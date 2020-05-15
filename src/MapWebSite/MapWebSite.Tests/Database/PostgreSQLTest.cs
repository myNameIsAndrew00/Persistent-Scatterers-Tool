using MapWebSite.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Tests.Database
{

    [TestClass]
    public class PostgreSQLTest
    {

        [TestMethod]
        public void CheckSelect()
        {
            PostgreSQLDataPointsRepository repo = new PostgreSQLDataPointsRepository();


            repo.GetPointDetails(1, new Model.PointBase() { Latitude = 27.9386897m, Longitude = 45.3758754m });


        }
    }
}
