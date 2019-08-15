
using MapWebSite.Model;
 

namespace MapWebSite.Core.DataPoints
{
    public interface IDataPointsSource
    {
        PointsDataSet CreateDataSet(string datasetName);

    }
}
