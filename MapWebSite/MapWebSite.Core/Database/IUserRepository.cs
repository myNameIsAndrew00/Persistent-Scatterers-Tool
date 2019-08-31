using MapWebSite.Model;
using System;
using System.Collections.Generic;

namespace MapWebSite.Core.Database
{
    public enum ColorMapFilters
    {
        ColorMapName = 1,
        Username = 2,
        None = -1
    }

    public interface IUserRepository
    {
        bool InsertUser(User user);

        bool CheckUser(string username, string password);

        int CreateUserPointsDataset(string username, string datasetName);

        bool CreateColorMap(string username, ColorMap colorMap);

        IEnumerable<string> GetColorMaps(string username);

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
    }
}
