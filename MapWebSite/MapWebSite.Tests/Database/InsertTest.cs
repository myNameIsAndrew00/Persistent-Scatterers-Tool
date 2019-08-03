using MapWebSite.Core.DataPoints;
using MapWebSite.Interaction;
using MapWebSite.Model;
using MapWebSite.Repository;
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

            bool response = handler.RegisterUser("woofwoofr", "andrei", "andrei", "andrei1234");

            Assert.IsTrue(response);

        }
        
        [TestMethod]
        public async Task InsertDataPoints()
        {
            DatabaseInteractionHandler handler = new DatabaseInteractionHandler();
            IDataPointsSource pointsSource = new TxtDataPointsSource();

            (pointsSource as TxtDataPointsSource).HeaderFile = @"P:\Projects\Licence\Main\docs\Data points\Constanta\header.txt";
            (pointsSource as TxtDataPointsSource).DisplacementsFile = @"P:\Projects\Licence\Main\docs\Data points\Constanta\displacements.txt";

            PointsDataSet dataset = pointsSource.CreateDataSet("rada45687");

            Task<bool> result = handler.InsertDataSet(dataset, "woofwoof");

            result.Wait();

            Assert.IsTrue(result.Result);
        }

        [TestMethod]
        public async Task InsertCassandraDataPoints()
        {
            PointsDataSet dataSet = new PointsDataSet()
            {
                ID = 1,
                Name = "test",
                Points = new List<Point>()
                {
                    new Point()
                    {
                        Number = 4,
                        Displacements = new List<Displacement>()
                        {
                            new Displacement() { Date = DateTime.Now },
                            new Displacement() { Date = DateTime.Now }
                        }
                    }
                }
            };

            CassandraDataPointsRepository repository = new CassandraDataPointsRepository();
            await repository.InsertPointsDataset(dataSet);
        }
    }
}
