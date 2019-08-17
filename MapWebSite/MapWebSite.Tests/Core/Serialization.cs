using MapWebSite.Core;
using MapWebSite.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Tests.Core
{
    [TestClass]
    public class Serialization
    {
        [TestMethod]
        public void JSONSerialize()
        {

            Point point = new Point();

            var data = point.JSONSerialize<Point>();
        }
    }
}
