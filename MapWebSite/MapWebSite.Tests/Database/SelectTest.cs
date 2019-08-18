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
            Repository.CassandraDataPointsRepository repository =
                new Repository.CassandraDataPointsRepository();
    
            var res = repository.GetDataPointsBasicInfo(0,
                                    0,
                                    new Tuple<decimal, decimal>(0m, 0m),
                                      new Tuple<decimal, decimal>(1144.4467674m, 1126.132538390000063m));
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
