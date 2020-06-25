using System;
using System.Collections.Generic;
using System.Text;

namespace MapWebSite.Model
{
    public class PointsDatasetOptionalData
    {
        public PointsSource HeaderType { get; set; }
    }

    public class GeoserverOptionalData : PointsDatasetOptionalData
    {

        public GeoserverOptionalData()
        {
            HeaderType = PointsSource.Geoserver;
        }

        public string ServerUrl { get; set; }

    }
}
