using MapWebSite.Core.DataPoints;
using MapWebSite.Interaction;
using MapWebSite.Model;
using MapWebSite.Repository;
using MapWebSite.Repository.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MapWebSite.Model.Point;

namespace MapWebSite.Tests.Database
{
    [TestClass]
    public class InsertTest
    {
        [TestMethod]
        public void InsertUser()
        {
            DatabaseInteractionHandler handler = new DatabaseInteractionHandler();

            bool response = handler.RegisterUser("woofwoofr", "andrei", "andrei", "andrei1234");

            Assert.IsTrue(response);

        }
        
        [TestMethod]
        public async Task InsertDataPoints()
        {
            DatabaseInteractionHandler handler = new DatabaseInteractionHandler();
            IDataPointsSource pointsSource = new TxtDataPointsSource();

            (pointsSource as TxtDataPointsSource).HeaderFile = @"P:\Projects\Licence\Main\docs\Data points\Constanta\secondHeader.txt";
            (pointsSource as TxtDataPointsSource).DisplacementsFile = @"P:\Projects\Licence\Main\docs\Data points\Constanta\secondDisplacements.txt";

            PointsDataSet dataset = pointsSource.CreateDataSet("rada456879");

            Task<bool> result = handler.InsertDataSet(dataset, "woofwoof");

            result.Wait();

            Assert.IsTrue(result.Result);
        }

        [TestMethod]
        public void InsertCassandraDataPoints()
        {
            IDataPointsSource pointsSource = new TxtDataPointsSource();

            (pointsSource as TxtDataPointsSource).HeaderFile = @"P:\Projects\Licence\Main\docs\Data points\Constanta\secondHeader.txt";
            (pointsSource as TxtDataPointsSource).DisplacementsFile = @"P:\Projects\Licence\Main\docs\Data points\Constanta\secondDisplacements.txt";

            PointsDataSet dataset = pointsSource.CreateDataSet("Test");
             

            IDataPointsZoomLevelsGenerator zoomGenerator = new SquareMeanPZGenerator();

            PointsDataSet[] set = zoomGenerator.CreateDataSetsZoomSets(dataset, 3, 19);

            CassandraDataPointsRepository repository = new CassandraDataPointsRepository();
            Task<bool> result = repository.InsertPointsDatasets(dataset, set);

            result.Wait();
        }
    }
}
