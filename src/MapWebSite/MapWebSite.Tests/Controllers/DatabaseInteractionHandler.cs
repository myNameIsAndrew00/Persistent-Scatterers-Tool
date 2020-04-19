using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Tests.Controllers
{
    [TestClass]
    public class DatabaseInteractionHandler
    {

        [TestMethod]
        public void ValidateOrSetPaletteToGeoserverLayerTest()
        {
            Domain.DatabaseInteractionHandler handler = 
                new Domain.DatabaseInteractionHandler();


            bool result = handler.ValidateOrSetPaletteToGeoserverLayer(
                "",
                "",
                "dem",
                "woofwoof123"
                );

            Assert.IsTrue(result);
        }
    }
}
