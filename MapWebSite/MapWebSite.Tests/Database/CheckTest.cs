using MapWebSite.Core.DataPoints;
using MapWebSite.Interaction;
using MapWebSite.Model;
using MapWebSite.Repository.Entities;
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

            bool response = handler.ValidateUser("woofwoofa", "andrei1234");

            Assert.IsTrue(response);

        }

        [TestMethod]
        public void CheckDataSetCreation()
        {
            IDataPointsSource pointsSource = new TxtDataPointsSource();

            (pointsSource as TxtDataPointsSource).HeaderFile = @"P:\Projects\Licence\Main\docs\Data points\Constanta\secondHeader.txt";
            (pointsSource as TxtDataPointsSource).DisplacementsFile = @"P:\Projects\Licence\Main\docs\Data points\Constanta\secondDisplacements.txt";

            PointsDataSet dataset = pointsSource.CreateDataSet("Test");

            IEnumerable<PointType> points = PointType.GetPoints(dataset);

            IDataPointsZoomLevelsGenerator zoomGenerator = new SquareMeanPZGenerator();

            PointsDataSet[] set = zoomGenerator.CreateDataSetsZoomSets(dataset, 4, 20);

            Assert.IsNotNull(dataset);
            Assert.IsNotNull(points);
        }
    }
}
