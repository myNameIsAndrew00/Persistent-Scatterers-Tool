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
            Domain.DomainInstance handler = 
                new Domain.DomainInstance();


            bool result = handler.ValidateGeoserverLayer(
                "",
                "",
                "dem",
                "woofwoof123"
                );

            Assert.IsFalse(result);
        }
    }
}
