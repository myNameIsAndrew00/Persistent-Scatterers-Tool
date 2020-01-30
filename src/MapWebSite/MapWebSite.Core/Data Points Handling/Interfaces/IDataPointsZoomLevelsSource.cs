using MapWebSite.Model;
using System;

namespace MapWebSite.Core.DataPoints
{ 
    [Obsolete("This mechanism is not longer needed")]
    public interface IDataPointsZoomLevelsSource
    {
        PointsDataSet[] CreateDataSetsZoomSets(PointsDataSet originalDataSet, int minZoomLevel, int maxZoomLevel);

    }
}
