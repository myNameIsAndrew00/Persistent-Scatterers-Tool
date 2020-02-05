
using MapWebSite.Model;
using System;
using System.Collections.Generic;

namespace MapWebSite.Core.DataPoints
{
     /// <summary>
     /// Provide methods to generate points datasets
     /// </summary>
    public interface IDataPointsSource
    {

        /// <summary>
        /// Creates a dataset or a list of datasets if the dataset is too large
        /// </summary>
        /// <param name="datasetName">The name of dataset</param>
        /// <param name="coordinateSystem">The coordinate system reference of dataset</param>
        /// <returns></returns>
        IEnumerable<PointsDataSet> CreateDataSet(string datasetName, CoordinateSystem coordinateSystem);

    }

    public enum CoordinateSystem
    {
        Default,
        UTM
    }
}
