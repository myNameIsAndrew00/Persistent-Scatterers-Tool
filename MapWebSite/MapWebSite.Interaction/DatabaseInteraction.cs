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

namespace MapWebSite.Interaction
{
    using Pair = Tuple<decimal, decimal>;
    using Coordinates = Tuple<
               Tuple<decimal, decimal>,
               Tuple<decimal, decimal>>
               ;

    /// <summary>
    /// Provides methods for interacting with the database 
    /// </summary>
    public class DatabaseInteractionHandler
    {

        private readonly IUserRepository userRepository;

        private readonly IDataPointsRepository dataPointsRepository;

        public DatabaseInteractionHandler()
        {
            userRepository = new SQLUserRepository();
            dataPointsRepository = CassandraDataPointsRepository.Instance;
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
        /// Returns data points which are inside a specific area of screen for a zoom level. Limits unit of measure with latitude/longitude.
        /// </summary>
        /// <param name="leftMargin"></param>
        /// <param name="rightMargin"></param>
        /// <param name="username"></param>
        /// <param name="dataSet"></param>
        /// <param name="zoomLevel"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public void RequestPoints(Pair leftMargin, 
                                                     Pair rightMargin, 
                                                     string username, 
                                                     string dataSet, 
                                                     int zoomLevel,
                                                     BasicPoint.BasicInfoOptionalField optionalField,
                                                     Action<IEnumerable<BasicPoint>> callback)
        {
            int dataSetID = this.userRepository.GetDatasetID(username, dataSet);
            if (dataSetID == -1) throw new ApplicationException($"User do not have a dataset with name {dataSet}");

            var points = PointsCacheManager.Get(leftMargin, rightMargin, dataSetID, out List<Coordinates> requiredCoordinates);

            if (requiredCoordinates != null)
            {              
                Parallel.ForEach(requiredCoordinates, coordinate =>
                {
                    try
                    {
                        var result = this.dataPointsRepository.GetDataPointsBasicInfo(dataSetID, 0, coordinate.Item1, coordinate.Item2, optionalField);
                        PointsCacheManager.Write(coordinate.Item1, coordinate.Item2, dataSetID, result);

                        //for(int i  = 0; i < result.Count() / 200; i++)
                        //    callback(result.Skip(i * 200).Take(200));
                        callback(result.Take(1000));
                    }
                    catch { //TODO: log exception
                    }
                });
            }
            
            //TODO: do not send a big bulk of data to client. Send it in packages.
            callback(points.Take(20000));
//            for (int i = 0; i < points.Count() / 200; i++)
//                    callback(points.Skip(i * 200).Take(200));

        }

        public Point RequestPointDetails(string dataSet, string username, int zoomLevel, BasicPoint basicPoint)
        {
            int dataSetID = this.userRepository.GetDatasetID(username, dataSet);
            if (dataSetID == -1) throw new ApplicationException($"User do not have a dataset with name {dataSet}");

            //TODO: ignore zoom level
            
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

        public IEnumerable<Tuple<string, string>> GetDataSets(DataSetsFilters filter, string filterValue, int pageIndex = 0, int itemsPerPage = 10)
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
