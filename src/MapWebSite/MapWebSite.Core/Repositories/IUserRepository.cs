using MapWebSite.Model;
using System;
using System.Collections.Generic;

namespace MapWebSite.Core.Database
{

    public enum UserFilters
    {
        None = -1,
        Username = 1,
        FirstName = 2,
        LastName = 3,
        Email = 4
    }

    public enum ColorMapFilters
    {
        None = -1,
        ColorMapName = 1,
        Username = 2,
    }

    public enum DataSetsFilters
    {
        None = -1,
        DataSetName = 1,
        Username = 2,
        Source = 3

        //todo: add State = 4
    }

    /// <summary>
    /// Interface for a repository which contains users and other data (palettes and points datasets headers)
    /// </summary>
    public interface IUserRepository
    {
        #region User related
        bool InsertUser(User user);

        bool UpdateUser(User user);

        User GetUser(string username);

        /// <summary>
        /// Retrieve a list of users based a set of filters
        /// </summary>
        /// <param name="filters">Filters which will be used to filter the result</param>
        /// <param name="pageIndex">Index of the page which must be returned</param>
        /// <param name="itemsPerPage">Represents how many items area available per page</param>
        /// <returns>A list of users</returns>
        IEnumerable<User> GetUsersFiltered(IEnumerable<Tuple<UserFilters, string>> filters, int pageIndex, int itemsPerPage);

        int GetUsersCount();

        User GetUserByEmail(string email);

        bool SetEmail(string username, string email);

        bool SetEmailConfirmed(string username, bool confirmed);

        IList<UserRoles> GetUserRoles(string username);

        bool CheckUser(string username, string password);

        byte[] GetUserHashedPassword(string username);


        #endregion

        //todo: move this methods in other repository, for datapoints headers
        #region Data points related

        /// <summary>
        /// Returns the number of datasets available for a user
        /// </summary>
        /// <param name="username">Username of the user</param>
        /// <returns>An integer representing available datasets count</returns>
        int GetUserAssociatedDatasetsCount(string username);

        /// <summary>
        /// Use this method to associate a points dataset with a user
        /// </summary>
        /// <param name="datasetName">Name of the dataset</param>
        /// <param name="datasetUser">User which created the dataset</param>
        /// <param name="username">The user which will be associated with the dataset</param>
        /// <returns>Returns true if operation succeed</returns>
        bool AddPointsDatasetToUser(string datasetName, string datasetUser, string username);

        /// <summary>
        /// Use this method to remove a associated points dataset from a user
        /// </summary>
        /// <param name="datasetName">Name of the dataset</param>
        /// <param name="datasetUser">User which created the dataset</param>
        /// <param name="username">The user which will be associated with the dataset</param>
        /// <returns>Returns true if operation succeed</returns>
        bool RemovePointsDatasetFromUser(string datasetName, string datasetUser, string username);

        /// <summary>
        /// Use this method to create a points dataset
        /// </summary>
        /// <param name="username">Username of user which creates the dataset</param>
        /// <param name="datasetName">The name of the dataset</param>
        /// <param name="pointsSource">Source type for dataset</param>
        /// <returns>Returns the id of the dataset which was inserted or -1 if operation fails </returns>
        int CreateUserPointsDataset(string username, string datasetName, PointsSource pointsSource);


        /// <summary>
        /// Use this method to update a dataset to be used as a geoserver source
        /// </summary>
        /// <param name="datasetId">Id of the dataset</param> 
        /// <param name="apiUrl">Api endpoint used to request data</param>
        /// <returns>Returns the id of the dataset which was inserted or -1 if operation fails</returns>
        int RaiseToGeoserverDataset(int datasetId, string apiUrl);


        /// <summary>
        /// Get the id of a dataset
        /// </summary>
        /// <param name="username">The owner of dataset</param>
        /// <param name="datasetName">The name of dataset</param>
        /// <returns></returns>
        int GetDatasetID(string username, string datasetName);



        /// <summary>
        /// Get the geoserver id of a dataset 
        /// </summary> 
        /// <param name="datasetId">The id of the dataset</param>
        /// <returns></returns>
        int GetGeoserverDatasetID(int datasetId);

        /// <summary>
        /// Get the header of a dataset
        /// </summary>
        /// <param name="username">The owner of dataset</param>
        /// <param name="datasetName">The name of dataset</param>
        /// <returns></returns>
        PointsDataSetHeader GetDatasetHeader(string username, string datasetName);

        /// <summary>
        /// Get the header of a dataset
        /// </summary>
        /// <param name="datasetId">Id of the dataset</param> 
        /// <returns></returns>
        PointsDataSetHeader GetDatasetHeader(int datasetId);

        IEnumerable<PointsDataSetHeader> GetDataSetsFiltered(string username, DataSetsFilters filter, string filterValue, int pageIndex, int itemsPerPage);

        IEnumerable<PointsDataSetHeader> GetDataSetsFiltered(string username, IEnumerable<Tuple<DataSetsFilters, string>> filters, int pageIndex, int itemsPerPage);

        int GetDatasetsCount();
        /// <summary>
        /// Provides a method to request color palettes (and their 'creators') using a filter 
        /// </summary>
        /// <param name="filter">Filter which will be applied</param>
        /// <param name="filterValue">Filter value</param>
        /// <param name="pageIndex">Index of the page which must be returned</param>
        /// <param name="itemsPerPage">Items contained in a page</param>
        /// <returns>List of tuples, first item of tuple represents user username and the second item, its color map</returns>
        IEnumerable<Tuple<string, ColorMap>> GetColorMapsFiltered(ColorMapFilters filter, string filterValue, int pageIndex, int itemsPerPage);

        /// <summary>
        /// Provides a method to request color palettes using multiple filters 
        /// </summary>
        /// <param name="filters">Filters which will be applied</param> 
        /// <param name="pageIndex">Index of the page which must be returned</param>
        /// <param name="itemsPerPage">Items contained in a page</param>
        /// <returns>List of tuples, first item of tuple represents user username and the second item, its color map</returns>
        IEnumerable<Tuple<string, ColorMap>> GetColorMapsFiltered(IEnumerable<Tuple<ColorMapFilters, string>> filters, int pageIndex, int itemsPerPage);


        /// <summary>
        /// Retrieve color maps which were been loaded in geoserver using the application. 
        /// </summary>
        /// <param name="geoserverDatasetId">The id of the geoserver dataset</param>
        /// <returns>List of tuples, first item of tuple represents user username and the second item, its color map</returns>
        IEnumerable<Tuple<string, ColorMap>> GetGeoserverColorMaps(int geoserverDatasetId);


        string GetColorMapSerialization(string username, string paletteName);

        /// <summary>
        /// Use this method to associate a geoserver dataset with a color palette
        /// </summary>
        /// <param name="geoserverDatasetId">Id of the geoserver dataset</param>
        /// <param name="paletteId">Id of color palette. Color palette must be uploaded to geoserver first.</param>
        /// <returns></returns>
        int InsertGeoserverColorMap(int geoserverDatasetId, int paletteId);


        /// <summary>
        /// Use this method to associate a geoserver dataset with a color palette
        /// </summary>
        /// <param name="geoserverDatasetId">Id of the geoserver dataset</param>
        /// <param name="paletteName">Name of the palette. Color palette must be uploaded to geoserver first.</param>
        /// <param name="paletteUsername">Username of user which uploaded the color palette. Color palette must be uploaded to geoserver first.</param>
        /// <returns></returns>
        int InsertGeoserverColorMap(int geoserverDatasetId, string paletteName, string paletteUsername);

        bool CreateColorMap(string username, ColorMap colorMap);

        /// <summary>
        /// Get all the color maps for a user
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        IEnumerable<string> GetColorMapsNames(string username);

        /// <summary>
        /// Use this method to update the status of an existing dataset
        /// </summary>
        /// <param name="datasetName">The name of the dataset which must change the status</param>
        /// <param name="status">The new status of the dataset</param>
        /// <param name="username">The owner of the dataset </param>
        /// <returns>A boolean which indicates if the update was successfully</returns>
        bool UpdateDatasetStatus(string datasetName, DatasetStatus status, string username);

        /// <summary>
        /// Use this method to update the limits ( latitude and longitude ) of an existing dataset
        /// </summary>
        /// <param name="datasetName">The name of the dataset</param>
        /// <param name="username">The owner of the dataset</param>
        /// <param name="minimumLatitude">Minimum latitude value</param>
        /// <param name="minimumLongitude">Minimum longitude value</param>
        /// <param name="maximumLatitude">Maximum latitude value</param>
        /// <param name="maximumLongitude">Maximum longitude value</param>
        /// <returns>A boolean which indicates if the update was successfully</returns>
        bool UpdateDatasetLimits(string datasetName,
                                 string username,
                                 decimal? minimumLatitude,
                                 decimal? minimumLongitude,
                                 decimal? maximumLatitude,
                                 decimal? maximumLongitude);

        /// <summary>
        /// Use this method to update the representation limits ( height, deformation rate, standard deviation ) of an existing dataset
        /// </summary>
        /// <param name="datasetName">The name of the dataset</param>
        /// <param name="username">The owner of the dataset</param>
        /// <param name="minimumLatitude">Minimum latitude value</param>
        /// <param name="minimumLongitude">Minimum longitude value</param>
        /// <param name="maximumLatitude">Maximum latitude value</param>
        /// <param name="maximumLongitude">Maximum longitude value</param>
        /// <param name="maximumLongitude">Maximum longitude value</param>
        /// <param name="maximumLongitude">Maximum longitude value</param>
        /// <returns>A boolean which indicates if the update was successfully</returns>
        bool UpdateDatasetRepresentationLimits(string datasetName,
                                 string username,
                                 decimal? minimumHeight,
                                 decimal? maximumHeight,
                                 decimal? minimumDeformationRate,
                                 decimal? maximumDeformationRate,
                                 decimal? minimumStdDev,
                                 decimal? maximumStdDev);

        #endregion
    }
}
