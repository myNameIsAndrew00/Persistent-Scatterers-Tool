using MapWebSite.Model;

namespace MapWebSite.Core.DataPoints
{
    public interface IDataPointsZoomLevelsGenerator
    {
        PointsDataSet[] CreateDataSetsZoomSets(PointsDataSet originalDataSet, int minZoomLevel, int maxZoomLevel);
    }
}
