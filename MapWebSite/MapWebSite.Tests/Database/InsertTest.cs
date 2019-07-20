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
    public class InsertTest
    {
        [TestMethod]
        public void InsertUser()
        {
            DatabaseInteractionHandler handler = new DatabaseInteractionHandler();

            bool response = handler.RegisterUser("woofwoof", "andrei", "andrei", "andrei1234");

            Assert.IsTrue(response);

        }
    }
}
