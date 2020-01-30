using MapWebSite.Core.DataPoints;
using MapWebSite.Model;
using MapWebSite.Repository.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Tests
{
    [TestClass]
    public class PointsGenerations
    {

        [TestMethod]
        public void CheckDataSetCreation()
        {
            IDataPointsSource pointsSource = new TxtDataPointsSource();
            IDataPointsRegionsSource regionSource = new PowerOfTwoRegionsSource();

            (pointsSource as TxtDataPointsSource).HeaderFile = @"P:\Projects\Licence\Main\git\docs\Data points\Constanta\header.txt";
            (pointsSource as TxtDataPointsSource).DisplacementsFile = @"P:\Projects\Licence\Main\git\docs\Data points\Constanta\displacements.txt";
            (pointsSource as TxtDataPointsSource).LatitudeZone = 'T';
            (pointsSource as TxtDataPointsSource).Zone = 35;
            PointsDataSet dataset = pointsSource.CreateDataSet("Test");

            bool regionsGenerationResult = regionSource.GenerateRegions(dataset);

            IEnumerable<PointType> points = PointType.GetPoints(dataset);
           
          //  IDataPointsZoomLevelsSource zoomGenerator = new SquareMeanPZGenerator();

          //   PointsDataSet[] set = zoomGenerator.CreateDataSetsZoomSets(dataset, 3, 19);

            Assert.IsNotNull(dataset);
            Assert.IsNotNull(points);
            Assert.IsTrue(regionsGenerationResult);
        }

    }
}
