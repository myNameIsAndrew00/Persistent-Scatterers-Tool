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
    
            var res = repository.GetDataPoints(0,
                                    0,
                                    new Tuple<decimal, decimal>(0m, 0m),
                                      new Tuple<decimal, decimal>(1144.4467674m, 1126.132538390000063m));
        }
    }
}
