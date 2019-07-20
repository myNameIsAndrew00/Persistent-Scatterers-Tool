using MapWebSite.Interaction;
using MapWebSite.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Tests.Database
{
    [TestClass]
    public class CheckTest
    {
        [TestMethod]
        public void CheckUser()
        {
            DatabaseInteractionHandler handler = new DatabaseInteractionHandler();

            bool response = handler.ValidateUser("woofwoof", "314");

            Assert.IsTrue(response);

        }
    }
}
