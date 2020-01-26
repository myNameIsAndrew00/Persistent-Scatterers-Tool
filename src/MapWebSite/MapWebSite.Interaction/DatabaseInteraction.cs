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
    using Coordinates = Tuple<
               Tuple<decimal, decimal>,
               Tuple<decimal, decimal>>;

    using PointsZonePair = Tuple<IEnumerable<PointBase>, string, bool>;

  
   
    /// <summary>
    /// Provides methods for interacting with the database 
    /// </summary>
    public class DatabaseInteractionHandler
    {
        private readonly int pointsPerBlock = 1000;

        private readonly IUserRepository userRepository;

        private readonly IDataPointsRepository dataPointsRepository;

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

            IDataPointsZoomLevelsGenerator zoomGenerator = new SquareMeanPZGenerator();

            PointsDataSet[] zoomedDataSets = zoomGenerator.CreateDataSetsZoomSets(pointsDataSet, 3, 19);

            return await this.dataPointsRepository.InsertPointsDatasets(pointsDataSet, zoomedDataSets);
        }

        /// <summary>
        /// Returns data points which are inside a specific area of screen for a zoom level. Limits unit of measure with latitude/longitude.
        /// </summary>
        /// <param name="leftMargin"></param>
        /// <param name="rightMargin"></param>
        /// <param name="username"></param>
        /// <param name="dataSet"></param>
        /// <param name="regionsPointsCount">Contains a list of regions which can be ignored for processing if the 
        /// number of points inside that region is valid. Could be optimized</param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public void RequestPoints(Pair leftMargin, 
                                                     Pair rightMargin, 
                                                     string username, 
                                                     string dataSet, 
                                                     Dictionary<string,int> regionsPointsCount,
                                                     Action<IEnumerable<PointBase>,Tuple<string,int>,bool> callback)
        {
            Action<IEnumerable<PointBase>, string, bool> triggerCallback = (points, regionKey, filled) =>
            {
                callback(null, new Tuple<string, int>(regionKey, points.Count()),filled);
                callback(points.Take(1000), null, filled);
                //for (int i = 0; i < points.Count() / pointsPerBlock; i++)
                //    callback(points.Skip(i * pointsPerBlock).Take(pointsPerBlock), null, filled);
            };

            //todo: cache the datasetId and request maximum/minimum lat/long for optimizations
            int dataSetID = this.userRepository.GetDatasetID(username, dataSet);
            if (dataSetID == -1) throw new ApplicationException($"User do not have a dataset with name {dataSet}");

            var pointsZonePairs = PointsCacheManager.Get(leftMargin, 
                                                         rightMargin, 
                                                         dataSetID,
                                                         regionsPointsCount,
                                                         out List<Coordinates> requiredCoordinates);
         
            if (requiredCoordinates != null)
            {              
                Parallel.ForEach(requiredCoordinates, coordinate =>
                {
                    try
                    {
                        var result = this.dataPointsRepository.GetDataPointsBasicInfo(dataSetID, 0, coordinate.Item1, coordinate.Item2);

                        string regionKey = PointsCacheManager.Write(coordinate.Item1, coordinate.Item2, dataSetID, result);

                        triggerCallback(result,regionKey, true); 
                    }
                    catch { //TODO: log exception
                        //if a error ocurs, the created entry must be deleted from the cache
                        PointsCacheManager.Remove(coordinate.Item1, coordinate.Item2, dataSetID);
                    }
                });
            }
             
            foreach(var pointsZonePair in pointsZonePairs)
            {
                triggerCallback(pointsZonePair.Item1, pointsZonePair.Item2, pointsZonePair.Item3);
            }
         

        }

        public Point RequestPointDetails(string dataSet, string username, int zoomLevel, PointBase basicPoint)
        {
            int dataSetID = this.userRepository.GetDatasetID(username, dataSet);
            if (dataSetID == -1) throw new ApplicationException($"User do not have a dataset with name {dataSet}");

            //TODO: ignore zoom level, delete it in further updates (*)
            
            return this.dataPointsRepository.GetPointDetails(dataSetID, zoomLevel, basicPoint);
        }

        public bool InsertColorPalette(string username, ColorMap colorMap)
        {
            return userRepository.CreateColorMap(username, colorMap);
        }

        
        public IEnumerable<Tuple<string,ColorMap>> GetColorPaletes(ColorMapFilters filter, string filterValue, int pageIndex = 0, int itemsPerPage = 10)
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
