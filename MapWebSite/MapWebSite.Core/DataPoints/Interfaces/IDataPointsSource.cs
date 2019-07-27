
using MapWebSite.Model;
using System.IO;

namespace MapWebSite.Core.DataPoints
{
    public interface IDataPointsSource
    {
        PointsDataSet CreateDataSet(string datasetName);
        
    }
}
