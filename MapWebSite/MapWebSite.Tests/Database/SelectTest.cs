using MapWebSite.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MapWebSite.Tests.Database
{
    [TestClass]
    public class SelectTest
    {
        [TestMethod]
        public void SelectFromCassandra()
        {

            HomeController controller = new HomeController();

            controller.RequestDataPoints(44.1808535337334495m, 28.620140047623096m,
                                        44.1866236776545m, 28.629087896896777m, 17);

        }

        [TestMethod]
        public void SelectPointDetails()
        {
            Repository.CassandraDataPointsRepository repository =
                new Repository.CassandraDataPointsRepository();

            var res = repository.GetPointDetails(55, 3, new Model.BasicPoint() { Latitude = 44.44935m, Longitude = 26.15415m, Number = 5360 });
        }
    }
}
