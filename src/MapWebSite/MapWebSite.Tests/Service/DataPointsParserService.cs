using MapWebSite.Core.DataPoints;
using MapWebSite.DataPointsParserService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Tests.Service
{
    [TestClass]
    public class DataPointsParserService
    {

        [TestMethod]
        public void ServiceRun()
        {
            Parser parser = new Parser();

            parser.ParserAction(null, null); 

        }

        [TestMethod]
        public void RegionsTest()
        {
            IDataPointsRegionsSource source = new PowerOfTwoRegionsSource();

            var x = source.GetRegionIndexes(longitude: 28.597728655515253600863950727m, latitude: 44.304150309297175136564201538m, zoomLevel:20);
            var y = source.GetRegionIndexes(-90, -180, 1);
        }

    }
}
