using MapWebSite.Model;
using System;

namespace MapWebSite.Core.DataPoints
{ 
    public interface IDataPointsZoomLevelsGenerator
    {
        PointsDataSet[] CreateDataSetsZoomSets(PointsDataSet originalDataSet, int minZoomLevel, int maxZoomLevel);
    }
}
