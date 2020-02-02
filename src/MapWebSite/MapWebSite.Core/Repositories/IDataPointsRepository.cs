using MapWebSite.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MapWebSite.Core.Database
{
    /// <summary>
    /// Interface for a repository which contains points
    /// </summary>
    public interface IDataPointsRepository
    {      
        /// <summary>
        /// Use this method to insert a dataset of points in the repository. If dataset contains and zoom regions, those will be inserted too.
        /// </summary>
        /// <param name="originalDataSet">The model of the dataset which will be inserted</param>
        /// <returns>Returns true if the points was inserted with succes</returns>
        Task<bool> InsertPointsDataset(PointsDataSet originalDataSet);

        /// <summary>
        /// Get data points from database
        /// </summary>
        /// <param name="dataSetID">Dataset ID containing the points</param>
        /// <param name="zoomLevel">The zoom required</param>
        /// <param name="from">Latitude and longitude from which points must be start</param>
        /// <param name="to">Latitude and longitude from which points must be end</param>
        /// <returns></returns>
        IEnumerable<PointBase> GetBasePoints(int dataSetID, 
                                                        Tuple<decimal, decimal> from, 
                                                        Tuple<decimal, decimal> to);
        
        /// <summary>
        /// Returns details about a point
        /// </summary>
        /// <param name="dataSetID">The id of the point whom details are required</param>
        /// <param name="basicPoint">The point which not contains details about the coordinate</param>
        /// <returns>A model containng all the data about a point coordinate</returns>
        Point GetPointDetails(int dataSetID, PointBase basicPoint);

        /// <summary>
        /// Returns a list of regions with points (scaled to a certain zoom level)
        /// </summary>
        /// <param name="datasetId">The id of the dataset containing the regions</param>
        /// <param name="from">The line and column of the region from where the 'scan' will start. First item represents the line, second represents the column.</param>
        /// <param name="to">The line and column of the region from where the 'scan' will end. First item represents the line, second represents the column.</param>
        /// <param name="zoomLevel">The zoom level of the regions requestsed</param>
        /// <returns>A list containing desired regions</returns>
        IEnumerable<PointsRegion> GetRegions(int datasetId, Tuple<int,int> from, Tuple<int,int> to, int zoomLevel);

        /// <summary>
        /// Returns a region with the specified row and column at a zoom level
        /// </summary>
        /// <param name="datasetId">The id of the dataset containing the region</param>
        /// <param name="row">The line(row) of the region</param>
        /// <param name="column">The column of the region</param>
        /// <param name="zoomLevel">The zoom level of the region requested</param>
        /// <returns>Desired region</returns>
        PointsRegion GetRegion(int datasetId, int row, int column, int zoomLevel);
      
    }
}
