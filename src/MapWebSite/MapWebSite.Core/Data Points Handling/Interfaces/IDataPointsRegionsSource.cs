using MapWebSite.Model;
using System;
using System.Collections.Generic;

namespace MapWebSite.Core.DataPoints
{
    /// <summary>
    /// Provide methods to generate regions of points, with predefined scales (based on an existing points dataset)
    /// </summary>
    public interface IDataPointsRegionsSource
    {
        /// <summary>
        /// Use this method to add 'zoom' regions to a dataset of points. An invalid set of points can generate undefined regions.       
        /// </summary>
        /// <param name="pointsDataset">The dataset which contains the points used to generate regions</param>
        /// <param name="sectionIndex">Use this parameter as a indexer for the sections. Can be used if the dataset is contained in multiple objects</param>
        /// <returns>Returns true if regions were generated with success</returns>
        bool GenerateRegions(PointsDataSet pointsDataset, int sectionIndex = 0);

        /// <summary>
        /// Use this method to generate 'zoom' regions for a dataset of points.
        /// </summary>
        /// <param name="dataPoints">The dataset which contains the points used to generate regions</param>
        /// <returns></returns>
        IEnumerable<PointsRegionsLevel> GenerateRegions(IEnumerable<PointBase> dataPoints, int sectionIndex = 0);

        /// <summary>
        /// Use this method to map a latitude/longitude coordinate to a top-left corner of a region
        /// </summary>
        /// <param name="latitude">The latitude of the point</param>
        /// <param name="longitude">The longitude of the point</param>
        /// <param name="zoomLevel">The zoom level of the region</param>
        /// <returns>A tuple which contains region top-left corner latitude in Item1 and longitude in Item2</returns>
        Tuple<decimal, decimal> MapCoordinateToRegionCorner(decimal latitude, decimal longitude, int zoomLevel);

        /// <summary>
        /// Use this method to get the index (row and column) of a region (which contains a described by latitude and longitude)
        /// </summary>
        /// <param name="latitude">The latitude of the point</param>
        /// <param name="longitude">The longitude of the point</param>
        /// <param name="zoomLevel">The zoom level of the region</param>
        /// <returns>A tuple which contains row value in Item1 and column value in Item2</returns>
        Tuple<int, int> GetRegionIndexes(decimal latitude, decimal longitude, int zoomLevel);
    }

}
