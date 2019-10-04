using MapWebSite.Core;
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

//            bool response = handler.ValidateUser("woofwoofa", "andrei1234");

            Assert.IsTrue(true);

        }

        [TestMethod]
        public void CheckDataSetCreation()
        {
            IDataPointsSource pointsSource = new TxtDataPointsSource();

            (pointsSource as TxtDataPointsSource).HeaderFile = @"P:\Projects\Licence\Main\docs\Data points\Constanta\header.txt";
            (pointsSource as TxtDataPointsSource).DisplacementsFile = @"P:\Projects\Licence\Main\docs\Data points\Constanta\displacements.txt";
            (pointsSource as TxtDataPointsSource).LatitudeZone = 'T';
            (pointsSource as TxtDataPointsSource).Zone = 35;
            PointsDataSet dataset = pointsSource.CreateDataSet("Test");

            IEnumerable<PointType> points = PointType.GetPoints(dataset);

            IDataPointsZoomLevelsGenerator zoomGenerator = new SquareMeanPZGenerator();

            PointsDataSet[] set = zoomGenerator.CreateDataSetsZoomSets(dataset, 3, 19);

            Assert.IsNotNull(dataset);
            Assert.IsNotNull(points);
        }


        [TestMethod]
        public void UTMConverterTest()
        {
            Helper.UTMConverter converter = new Helper.UTMConverter();
            var result = converter.ToLatLong(35, 'T', (double)632353.875m, (double)4919262.500m);
        }
    }
}
