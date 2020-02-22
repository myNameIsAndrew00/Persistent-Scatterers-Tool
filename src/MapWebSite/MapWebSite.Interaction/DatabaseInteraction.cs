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
    /// Provides methods for interacting with the databases
    /// </summary>
    public class DatabaseInteractionHandler
    { 

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
        /// Create an empty dataset in the database. Created dataset will wave status Pending 
        /// </summary>
        /// <param name="datasetName">Name of the dataset</param>
        /// <param name="username">The username of the user which create the dataset</param>
        /// <returns>A boolean which indicates if the insert was succesufully</returns>
        public bool CreateDataSet(string datasetName, string username)
        {
            int datasetId = this.userRepository.CreateUserPointsDataset(username, datasetName);
            return datasetId != -1;
        }

        public bool UpdateDatasetStatus(string datasetName, DatasetStatus status, string username)
        {
            return this.userRepository.UpdateDatasetStatus(datasetName, status, username);
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
            //todo: cache the header
            PointsDataSetHeader datasetHeader = this.userRepository.GetDatasetHeader(username, dataSet);
            if (datasetHeader == null) throw new ApplicationException($"User do not have a dataset with name {dataSet}");

            //set the requested latitude and longitude coordinates of corners in the available limits of the dataset
            var topLeftIndexes = this.dataPointsRegionSource.GetRegionIndexes(
                                                    topLeftCorner.Item1 < (datasetHeader.MaximumLatitude ?? 90m) ? topLeftCorner.Item1 : datasetHeader.MaximumLatitude ?? 90m,
                                                    topLeftCorner.Item2 > (datasetHeader.MinimumLongitude ?? -180m) ? topLeftCorner.Item2 : datasetHeader.MinimumLongitude ?? -180m,
                                                    zoomLevel);

            var bottomRightIndexes = this.dataPointsRegionSource.GetRegionIndexes(
                                                    bottomRightCorner.Item1 > (datasetHeader.MinimumLatitude ?? -90m) ? bottomRightCorner.Item1 : (datasetHeader.MinimumLatitude ?? -90m),
                                                    bottomRightCorner.Item2 < (datasetHeader.MaximumLongitude ?? 180m) ? bottomRightCorner.Item2 : (datasetHeader.MaximumLongitude ?? 180m),
                                                    zoomLevel);

            var serverCachedRegions = PointsCacheManager.Get(topLeftIndexes,
                                                         bottomRightIndexes,
                                                         zoomLevel,
                                                         datasetHeader.ID,
                                                         cachedRegions,
                                                         out List<Coordinates> requiredRegions);

            if (requiredRegions != null)
            {
                Parallel.ForEach(requiredRegions, coordinate =>
                {
                    try
                    {
                        var result = this.dataPointsRepository.GetRegion(datasetHeader.ID, coordinate.Item1, coordinate.Item2, zoomLevel);

                        string regionKey = null;
                        if (result != null)
                            regionKey = PointsCacheManager.Write(coordinate.Item1, coordinate.Item2, coordinate.Item3, datasetHeader.ID, result.Points);

                        callback(result == null ? new List<PointBase>() : result.Points, regionKey);                         
                    }
                    catch (Exception exception)
                    { //TODO: log exception
                        //if a error ocurs, the created entry must be deleted from the cache
                        PointsCacheManager.Remove(coordinate.Item1, coordinate.Item2, coordinate.Item3, datasetHeader.ID);
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
            //todo: cache the header
            PointsDataSetHeader dataSetHeader = this.userRepository.GetDatasetHeader(username, dataSet);
            if (dataSetHeader == null) throw new ApplicationException($"User do not have a dataset with name {dataSet}");
  
            //set the requested latitude and longitude coordinates of corners in the available limits of the dataset
            var topLeftIndexes = this.dataPointsRegionSource.GetRegionIndexes(
                                                    topLeftCorner.Item1 < (dataSetHeader.MaximumLatitude ?? 90m) ? topLeftCorner.Item1 : dataSetHeader.MaximumLatitude ?? 90m,
                                                    topLeftCorner.Item2 > (dataSetHeader.MinimumLongitude ?? -180m) ? topLeftCorner.Item2 : dataSetHeader.MinimumLongitude ?? -180m,                                                    
                                                    zoomLevel);
           
            var bottomRightIndexes = this.dataPointsRegionSource.GetRegionIndexes(
                                                    bottomRightCorner.Item1 > (dataSetHeader.MinimumLatitude ?? -90m) ? bottomRightCorner.Item1 : (dataSetHeader.MinimumLatitude ?? -90m),
                                                    bottomRightCorner.Item2 < (dataSetHeader.MaximumLongitude ?? 180m) ? bottomRightCorner.Item2 : (dataSetHeader.MaximumLongitude ?? 180m),
                                                    zoomLevel);

            return PointsCacheManager.GetKeys(topLeftIndexes, bottomRightIndexes, zoomLevel, dataSetHeader.ID);
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

        public IEnumerable<PointsDataSetHeader> GetDataSets(DataSetsFilters filter, string filterValue, int pageIndex = 0, int itemsPerPage = 10)
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
