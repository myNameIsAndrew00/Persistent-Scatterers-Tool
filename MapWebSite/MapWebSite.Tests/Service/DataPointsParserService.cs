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

    }
}
