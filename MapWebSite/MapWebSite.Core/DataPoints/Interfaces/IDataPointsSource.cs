
using MapWebSite.Model;
using System;

namespace MapWebSite.Core.DataPoints
{
     
    public interface IDataPointsSource
    {
        PointsDataSet CreateDataSet(string datasetName);

    }
}
