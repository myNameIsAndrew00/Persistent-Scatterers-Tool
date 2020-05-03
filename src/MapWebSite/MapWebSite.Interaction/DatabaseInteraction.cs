using MapWebSite.Core.Database;
using MapWebSite.Core.DataPoints;
using MapWebSite.Model;
using MapWebSite.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using MapWebSite.GeoserverAPI;
using MapWebSite.GeoserverAPI.Modules.Styles;
using System.Threading.Tasks;
using System.Linq;
using MapWebSite.GeoserverAPI.Modules.Layers;
using MapWebSite.GeoserverAPI.Entities;

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
        /// <summary>
        /// An enum which handles codes for CreateDataset function
        /// </summary>
        public enum CreateDatasetResultCode
        {
            Ok,
            GeoserverError,
            DatasetError
        }

        private readonly IUserRepository userRepository;

        private readonly IDataPointsRepository dataPointsRepository;

        private readonly IDataPointsRegionsSource dataPointsRegionSource;

        private readonly Func<GeoserverClient> geoserverClient = () => 
              new GeoserverClient(
                  ConfigurationManager.AppSettings["GeoserverApiUrl"],
                  ConfigurationManager.AppSettings["GeoserverUser"],
                  ConfigurationManager.AppSettings["GeoserverPassword"]
                  );
        

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
        /// <returns>A boolean which indicates if the insert was succesufully and a return message</returns>
        public CreateDatasetResultCode CreateDataSet(string datasetName, string username, PointsSource pointsSource, string apiUrl = null)
        {
            /** error strings: 
             * Dataset - caused by dataset insertions
             * Geoserver - caused by geoserver 
             **/

            int datasetId = this.userRepository.CreateUserPointsDataset(username, datasetName, pointsSource);

            if (pointsSource == PointsSource.Geoserver)
            { 
                LayersBuilder builder = new LayersBuilder
                {
                    LayerName = datasetName,
                    SingleLayer = true
                };

                GeoserverClient client = geoserverClient();

                if (client.Get<Layer>(new ModulesFactory().CreateLayerModule(builder)) == null)
                    return CreateDatasetResultCode.GeoserverError;

                //if dataset already exists in database
                if (datasetId == -1) datasetId = this.userRepository.GetDatasetID(username, datasetName);
                 
                datasetId = this.userRepository.RaiseToGeoserverDataset(datasetId, apiUrl);
            }

            return datasetId != -1 ? CreateDatasetResultCode.Ok : CreateDatasetResultCode.DatasetError;
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

        /// <summary>
        /// Use this method to associate a style with a layer in geoserver or to validat
        /// </summary>
        /// <param name="datasetId"></param>
        /// <param name="paletteName"></param>
        /// <param name="paletteUsername"></param>
        /// <returns></returns>
        public bool ValidateOrSetPaletteToGeoserverLayer(string datasetName, string datasetUsername, string paletteName, string paletteUsername)
        {
            /**
             * Steps: 1. check if palette is already asociated. If yes, return true 
             *        2. if the palette is not associated, retrieve it from db and associate it with layer.
             *        3. if insert succeed, count the palette in the database and return true
             *        4. if insert fail, return false
             */

            try
            {

                if (string.IsNullOrEmpty(paletteName) || string.IsNullOrEmpty(paletteUsername)) return false;

                int datasetId = userRepository.GetDatasetID( datasetUsername, datasetName);
                int geoserverId = userRepository.GetGeoserverDatasetID(datasetId);
               
                if (geoserverId == -1) return false;

                var userPalettes = userRepository.GetGeoserverColorMaps(geoserverId);

                var selectedPalette = userPalettes.Where(p => p.Item1 == paletteUsername && p.Item2.Name == paletteName).FirstOrDefault();
                if (selectedPalette != null) return true;

                GeoserverClient client = geoserverClient();
                ModulesFactory modulesFactory = new ModulesFactory();

                LayersBuilder builder = new LayersBuilder();
                builder.LayerName = userRepository.GetDatasetHeader(datasetId).Name;      
                builder.SingleLayer = true;

                builder.Styles = userPalettes.Select(p => p.Item1 + '_' + p.Item2.Name).ToList();
                builder.Styles.Add(paletteUsername + '_' + paletteName);

                return client.Put(modulesFactory.CreateLayerModule(builder)) ?
                    userRepository.InsertGeoserverColorMap(
                        geoserverId,
                        paletteName,
                        paletteUsername
                    ) != -1
                    : false;

            }
            catch(Exception exception)
            {
                return false;
            }

        }


        public bool InsertColorPalette(string username, ColorMap colorMap)
        {
            GeoserverClient client = geoserverClient();

            StylesBuilder stylesBuilder = 
                new StylesBuilder(username + '_' + colorMap.Name, colorMap.Name);

            foreach (var intervalRule in colorMap.GetRules())
                stylesBuilder.AddRule(intervalRule);

            if (!client.Post(new ModulesFactory().CreateStylesModule(stylesBuilder)))
                return false;

            return userRepository.CreateColorMap(username, colorMap);
        }


        public IEnumerable<Tuple<string, ColorMap>> GetColorPaletes(IEnumerable<Tuple<ColorMapFilters,string>> filters, int pageIndex = 0, int itemsPerPage = 10)
        {
            //TODO: handle errors or do more checks if needed
            return this.userRepository.GetColorMapsFiltered(filters, pageIndex, itemsPerPage);
        }

        /// <summary>
        /// Returns datasets available for a user
        /// </summary>
        /// <param name="username">Username of the user which require data</param>
        /// <param name="ignoreUsername">A parameter which can be set to ignore the user association with required datasets</param>
        /// <param name="filters">Filter applied for searching</param>
        /// <param name="pageIndex">Index of the page requested</param>
        /// <param name="itemsPerPage">Items count per page</param>
        /// <returns>An array of data sets headers</returns>
        public IEnumerable<PointsDataSetHeader> GetDataSets(string username, bool ignoreUsername, IEnumerable<Tuple<DataSetsFilters,string>> filters, int pageIndex = 0, int itemsPerPage = 10)
        {
            //TODO: handle errors or do more checks if needed
            return this.userRepository.GetDataSetsFiltered(username, ignoreUsername, filters, pageIndex, itemsPerPage);
        }

        public bool ChangeUserAssociatedDataset(string username ,string datasetName, string datasetUsername, bool add)
        {
            //TODO: handle errors or do more checks if needed
            return add ? this.userRepository.AddPointsDatasetToUser(datasetName, datasetUsername, username)
                       : this.userRepository.RemovePointsDatasetFromUser(datasetName, datasetUsername, username);
        }


        public IEnumerable<User> GetUsers(IEnumerable<Tuple<UserFilters, string>> filters, int pageIndex = 0, int itemsPerPage = 10)
        {
            return this.userRepository.GetUsersFiltered(filters, pageIndex, itemsPerPage);
        }

        public int GetUsersCount()
        {
            return this.userRepository.GetUsersCount();           
        }

        public int GetUsersAssociatedDatasetsCount(string username)
        {
            return this.userRepository.GetUserAssociatedDatasetsCount(username);
        }


        public PointsDataSetHeader GetDataSet(string username, string datasetName)
        {
            return this.userRepository.GetDatasetHeader(username, datasetName);
        }

        public string GetColorPaletteSerialization(string username, string paletteName)
        {
            return this.userRepository.GetColorMapSerialization(username, paletteName);
        }

        public User GetUser(string username, bool useEmail)
        {
            return useEmail ? this.userRepository.GetUserByEmail(username) : this.userRepository.GetUser(username);
        }

    }
}
