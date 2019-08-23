
using MapWebSite.Model;
using System;

namespace MapWebSite.Core.DataPoints
{

    [Obsolete("This interface will be replaced by one writen in C++")]
    public interface IDataPointsSource
    {
        PointsDataSet CreateDataSet(string datasetName);

    }
}
