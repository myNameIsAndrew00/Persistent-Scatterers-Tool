
using MapWebSite.Model;
using System;

namespace MapWebSite.Core.DataPoints
{
     /// <summary>
     /// Provide methods to generate points datasets
     /// </summary>
    public interface IDataPointsSource
    {

        PointsDataSet CreateDataSet(string datasetName, CoordinateSystem coordinateSystem);

    }

    public enum CoordinateSystem
    {
        Default,
        UTM
    }
}
