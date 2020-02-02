using MapWebSite.Model;
using System;
using System.Collections.Generic;

namespace MapWebSite.Core.Database
{
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
        //todo: add State = 3
    }

    /// <summary>
    /// Interface for a repository which contains users and other data (palettes and points datasets headers)
    /// </summary>
    public interface IUserRepository
    {
        bool InsertUser(User user);

        bool UpdateUser(User user);

        User GetUser(string username);

        IList<UserRoles> GetUserRoles(string username);

        bool CheckUser(string username, string password);

        byte[] GetUserHashedPassword(string username);

        int CreateUserPointsDataset(string username, string datasetName);

        bool CreateColorMap(string username, ColorMap colorMap);

        /// <summary>
        /// Get all the color maps for a user
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        IEnumerable<string> GetColorMapsNames(string username);

        /// <summary>
        /// Get the id of a dataset
        /// </summary>
        /// <param name="username">The owner of dataset</param>
        /// <param name="datasetName">The name of dataset</param>
        /// <returns></returns>
        int GetDatasetID(string username, string datasetName);

        /// <summary>
        /// Get the header of a dataset
        /// </summary>
        /// <param name="username">The owner of dataset</param>
        /// <param name="datasetName">The name of dataset</param>
        /// <returns></returns>
        PointsDataSetHeader GetDatasetHeader(string username, string datasetName);

        /// <summary>
        /// Provides a method to request color palettes (and their 'creators') using a filter 
        /// </summary>
        /// <param name="filter">Filter which will be applied</param>
        /// <param name="filterValue">Filter value</param>
        /// <param name="pageIndex">Index of the page which must be returned</param>
        /// <param name="itemsPerPage">Items contained in a page</param>
        /// <returns>List of tuples, first item of tuple represents user username and the second item, its color map</returns>
        IEnumerable<Tuple<string, ColorMap>> GetColorMapsFiltered(ColorMapFilters filter, string filterValue, int pageIndex, int itemsPerPage);

        string GetColorMapSerialization(string username, string paletteName);

        IEnumerable<PointsDataSetHeader> GetDataSetsFiltered(DataSetsFilters filter, string filterValue, int pageIndex, int itemsPerPage);

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
    }
}
