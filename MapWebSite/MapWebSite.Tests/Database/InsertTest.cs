using MapWebSite.Core.DataPoints;
using MapWebSite.Domain;
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

         //   bool response = handler.RegisterUser("test45", "andrei", "andrei", "woofwoof");

            Assert.IsTrue(true);

        }
        
        [TestMethod]
        public void InsertDataPoints()
        {
            DatabaseInteractionHandler handler = new DatabaseInteractionHandler();
            IDataPointsSource pointsSource = new TxtDataPointsSource();

            (pointsSource as TxtDataPointsSource).HeaderFile = @"P:\Projects\Licence\Main\docs\Data points\Constanta\header.txt";
            (pointsSource as TxtDataPointsSource).DisplacementsFile = @"P:\Projects\Licence\Main\docs\Data points\Constanta\displacements.txt";
            (pointsSource as TxtDataPointsSource).LatitudeZone = 'T';   
            (pointsSource as TxtDataPointsSource).Zone = 35;
            PointsDataSet dataset = pointsSource.CreateDataSet("fourthTest");

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

            CassandraDataPointsRepository repository = CassandraDataPointsRepository.Instance;
            Task<bool> result = repository.InsertPointsDatasets(dataset, set);

            result.Wait();
        }
    }
}
