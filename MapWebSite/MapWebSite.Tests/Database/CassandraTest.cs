using MapWebSite.Core;
using MapWebSite.Repository.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Tests.Database
{
    [TestClass]
    public class CassandraTest
    {
        [TestMethod]
        public void CheckQueryBuilder()
        {
            CassandraAccess.CassandraQueryBuilder builder = new CassandraAccess.CassandraQueryBuilder();
            builder.TableName = "points_by_dataset";
            builder.Type = typeof(PointType);

            PointDisplacementType type = new PointDisplacementType()
            {
                days_from_reference = 1,
                date = DateTime.Now,
                jd = 23.3m,
                value = 23.4m
            };

            var jsonString = type.JSONSerialize(false);

            string result = builder.BuildInsertQueryFromType();

            Assert.IsNotNull(result);
        }
    }
}
