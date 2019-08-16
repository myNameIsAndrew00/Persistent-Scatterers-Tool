using MapWebSite.Core;
using MapWebSite.Core.Database;
using MapWebSite.Core.DataPoints;
using MapWebSite.Model;
using MapWebSite.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Interaction
{
    using Pair = Tuple<decimal, decimal>;
    public class DatabaseInteractionHandler
    {

        private readonly IUserRepository userRepository;

        private readonly IDataPointsRepository dataPointsRepository;

        public DatabaseInteractionHandler()
        {
            userRepository = new SQLUserRepository();
            dataPointsRepository = new CassandraDataPointsRepository();
        }

        public bool RegisterUser(string username, string firstName, string lastName, string password)
        {
            IUserRepository userRepository = new SQLUserRepository();

            byte[] passwordSalt = Helper.GenerateRandomBytes(32);

            return userRepository.InsertUser(new User()
            {
                FirstName = firstName,
                LastName = lastName,
                PasswordHash = Helper.HashData(Encoding.UTF8.GetBytes(password), passwordSalt),
                PasswordSalt = passwordSalt,
                Username = username
            });

        }

        public bool ValidateUser(string username, string password)
        { 
            return this.userRepository.CheckUser(username, password);
        }

        /// <summary>
        /// Insert a dataset for a user. Inserted data will contain zoomed versions of points
        /// </summary>
        /// <param name="pointsDataSet"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<bool> InsertDataSet(PointsDataSet pointsDataSet, string username)
        {
   
            pointsDataSet.ID = this.userRepository.CreateUserPointsDataset(username, pointsDataSet.Name);

            if (pointsDataSet.ID == -1) return false;

            IDataPointsZoomLevelsGenerator zoomGenerator = new SquareMeanPZGenerator();

            PointsDataSet[] zoomedDataSets = zoomGenerator.CreateDataSetsZoomSets(pointsDataSet, 3, 19);

            return await this.dataPointsRepository.InsertPointsDatasets(pointsDataSet, zoomedDataSets);
        }

        /// <summary>
        /// Returns data points which are inside a specific area of screen for a zoom level. Limits unit of measure is latitude/longitude.
        /// </summary>
        /// <param name="leftMargin"></param>
        /// <param name="rightMargin"></param>
        /// <param name="username"></param>
        /// <param name="dataSet"></param>
        /// <param name="zoomLevel"></param>
        /// <returns></returns>
        public IEnumerable<Point> RequestPoints(Pair leftMargin, Pair rightMargin, string username, string dataSet, int zoomLevel)
        {
            int dataSetID = this.userRepository.GetDatasetID(username, dataSet);
            if (dataSetID == -1) throw new ApplicationException($"User do not have a dataset with name {dataSet}");

            return this.dataPointsRepository.GetDataPoints(dataSetID, zoomLevel, leftMargin, rightMargin);
        }
    }
}
