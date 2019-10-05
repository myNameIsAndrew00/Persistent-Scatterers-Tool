﻿using MapWebSite.Model;
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
        Username = 2
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

        int GetDatasetID(string username, string datasetName);

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

        IEnumerable<Tuple<string, string>> GetDataSetsFiltered(DataSetsFilters filter, string filterValue, int pageIndex, int itemsPerPage);
    }
}