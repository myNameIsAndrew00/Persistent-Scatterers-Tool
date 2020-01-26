using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MapWebSite;
using MapWebSite.Controllers;
using MapWebSite.HtmlHelpers;

namespace MapWebSite.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
      
        [TestMethod]
        public void MessageBox()
        {
            var result = MessageBoxBuilder.Create("titlu", "mesaj");

            Assert.IsNotNull(result);
        }
       
 
    }
}
