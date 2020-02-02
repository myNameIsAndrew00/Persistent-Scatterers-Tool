using MapWebSite.Core;
using MapWebSite.Core.Database;
using MapWebSite.Core.DataPoints;
using MapWebSite.Model;
using MapWebSite.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Domain
{
    using Pair = Tuple<decimal, decimal>;
    using Coordinates = Tuple<int, int, int>;

    using PointsZonePair = Tuple<IEnumerable<PointBase>, string>;


    /// <summary>
    /// Provides methods for interacting with the database 
    /// </summary>
    public class DatabaseInteractionHandler
    {
        private readonly int pointsPerBlock = 1000;

        private readonly IUserRepository userRepository;

        private readonly IDataPointsRepository dataPointsRepository;

        private readonly IDataPointsRegionsSource dataPointsRegionSource;

        /// <summary>
        /// Use this method to initialise the any database connections
        /// </summary>
        public static void Initialise()
        {
            CassandraDataPointsRepository.Initialise();
        }

        public DatabaseInteractionHandler()
        {
            userRepository = new SQLUserRepository();
            dataPointsRepository = CassandraDataPointsRepository.Instance;
            dataPointsRegionSource = new PowerOfTwoRegionsSource();
        }

        /// <summary>
        /// Create a dataset in the database. Created dataset will wave status Pending 
        /// </summary>
        /// <param name="datasetName"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool CreateDataSet(string datasetName, string username)
        {
            int datasetId = this.userRepository.CreateUserPointsDataset(username, datasetName);
            return datasetId != -1;
        }

        public bool UpdateDatasetStatus(string datasetName, DatasetStatus status, string username)
        {
            return this.userRepository.UpdateDatasetStatus(datasetName, status, username);
        }

        [Obsolete("This method is implemented in service")]
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

            IDataPointsZoomLevelsSource zoomGenerator = new SquareMeanPZGenerator();
            PointsDataSet[] zoomedDataSets = zoomGenerator.CreateDataSetsZoomSets(pointsDataSet, 3, 19);


            return await this.dataPointsRepository.InsertPointsDataset(pointsDataSet);
        }

        /// <summary>
        /// Returns data points regions which are inside a specific area of screen for a zoom level. Limits unit of measure with latitude/longitude.
        /// </summary>
        /// <param name="topLeftCorner"></param>
        /// <param name="bottomRightCorner"></param>
        /// <param name="username"></param>
        /// <param name="dataSet"></param>
        /// <param name="cachedRegions">A list of regions keys which are cached on browser</param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public void RequestPointsRegions(Pair topLeftCorner,
                                                     Pair bottomRightCorner,
                                                     int zoomLevel,
                                                     string username,
                                                     string dataSet,
                                                     string[] cachedRegions,
                                                     Action<IEnumerable<PointBase>, string> callback)
        {          
            //todo: get the datasetId and request maximum/minimum lat/long for optimizations
          
            int dataSetID = this.userRepository.GetDatasetID(username, dataSet);
            if (dataSetID == -1) throw new ApplicationException($"User do not have a dataset with name {dataSet}");

            var topLeftIndexes = this.dataPointsRegionSource.GetRegionIndexes(topLeftCorner.Item1,
                                                                                    topLeftCorner.Item2,
                                                                                    zoomLevel);
            var bottomRightIndexes = this.dataPointsRegionSource.GetRegionIndexes(bottomRightCorner.Item1,
                                                                                    bottomRightCorner.Item2,
                                                                                    zoomLevel);

            var serverCachedRegions = PointsCacheManager.Get(topLeftIndexes,
                                                         bottomRightIndexes,
                                                         zoomLevel,
                                                         dataSetID,
                                                         cachedRegions,
                                                         out List<Coordinates> requiredRegions);

            if (requiredRegions != null)
            {
                Parallel.ForEach(requiredRegions, coordinate =>
                {
                    try
                    {
                        var result = this.dataPointsRepository.GetRegion(dataSetID, coordinate.Item1, coordinate.Item2, zoomLevel);

                        string regionKey = null;
                        if (result != null)
                            regionKey = PointsCacheManager.Write(coordinate.Item1, coordinate.Item2, coordinate.Item3, dataSetID, result.Points);

                        callback(result == null ? new List<PointBase>() : result.Points, regionKey);                         
                    }
                    catch (Exception exception)
                    { //TODO: log exception
                        //if a error ocurs, the created entry must be deleted from the cache
                        PointsCacheManager.Remove(coordinate.Item1, coordinate.Item2, coordinate.Item3, dataSetID);
                    }
                });
            }

            foreach (var pointsZonePair in serverCachedRegions)
            {
                callback(pointsZonePair.Item1, pointsZonePair.Item2);
            }


        }

        public List<string> RequestPointsRegionsKeys(Pair topLeftCorner,
                                                     Pair bottomRightCorner,
                                                     int zoomLevel,
                                                     string username,
                                                     string dataSet)
        {
            int dataSetID = this.userRepository.GetDatasetID(username, dataSet);
            if (dataSetID == -1) throw new ApplicationException($"User do not have a dataset with name {dataSet}");

            var topLeftIndexes = this.dataPointsRegionSource.GetRegionIndexes(topLeftCorner.Item1,
                                                                                    topLeftCorner.Item2,
                                                                                    zoomLevel);
            var bottomRightIndexes = this.dataPointsRegionSource.GetRegionIndexes(bottomRightCorner.Item1,
                                                                                    bottomRightCorner.Item2,
                                                                                    zoomLevel);

            return PointsCacheManager.GetKeys(topLeftIndexes, bottomRightIndexes, zoomLevel, dataSetID);
        }

        public Point RequestPointDetails(string dataSet, string username, int zoomLevel, PointBase basicPoint)
        {
            int dataSetID = this.userRepository.GetDatasetID(username, dataSet);
            if (dataSetID == -1) throw new ApplicationException($"User do not have a dataset with name {dataSet}");

            //TODO: ignore zoom level, delete it in further updates (*)

            return this.dataPointsRepository.GetPointDetails(dataSetID, basicPoint);
        }

        public bool InsertColorPalette(string username, ColorMap colorMap)
        {
            return userRepository.CreateColorMap(username, colorMap);
        }


        public IEnumerable<Tuple<string, ColorMap>> GetColorPaletes(ColorMapFilters filter, string filterValue, int pageIndex = 0, int itemsPerPage = 10)
        {
            //TODO: handle errors or do more checks if needed
            return this.userRepository.GetColorMapsFiltered(filter, filterValue, pageIndex, itemsPerPage);
        }

        public IEnumerable<PointsDataSetBase> GetDataSets(DataSetsFilters filter, string filterValue, int pageIndex = 0, int itemsPerPage = 10)
        {
            //TODO: handle errors or do more checks if needed
            return this.userRepository.GetDataSetsFiltered(filter, filterValue, pageIndex, itemsPerPage);
        }


        public string GetColorPaletteSerialization(string username, string paletteName)
        {
            return this.userRepository.GetColorMapSerialization(username, paletteName);
        }

    }
}
