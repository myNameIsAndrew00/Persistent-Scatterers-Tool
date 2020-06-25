using MapWebSite.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Tests.Controllers
{
    [TestClass]
    public class CacheTest
    {

        [TestMethod]
        public void StoreKey()
        { ;
            PointsCacheManager.Create(0, 0, 0, 0);

            PointsCacheManager.Write(0, 0, 0, 0, new List<Model.PointBase>()
            {
                new Model.PointBase(),
                new Model.PointBase()
            });

            var result = PointsCacheManager.Get(new Tuple<int, int>(0, 0),
                new Tuple<int, int>(0, 0),
                0,
                0,
                new string[] { },
                out var uncachedRegions);

            Assert.IsNull(uncachedRegions);
            Assert.IsNotNull(result);
        }
    }
}
