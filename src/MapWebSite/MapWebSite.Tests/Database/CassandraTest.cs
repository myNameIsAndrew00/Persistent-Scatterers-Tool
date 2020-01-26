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
    using Builder = CassandraAccess.CassandraQueryBuilder;
    [TestClass]
    public class CassandraTest
    {
        [TestMethod]
        public void CheckQueryBuilder()
        {
            CassandraAccess.CassandraQueryBuilder builder = new CassandraAccess.CassandraQueryBuilder();
            builder.TableName = "points_by_dataset";
            builder.ClausesList.Add(new Tuple<string, string, Builder.Clauses>
                ("top_latitude", "latitude", Builder.Clauses.LessOrEqual));                   

            string result = builder.BuildSelectQuery();

            Assert.IsNotNull(result);
        }




    }
}
