using System;
using System.Collections.Generic;
using System.Text;

namespace MapWebSite.Model
{
    /// <summary>
    /// Use this class as a model for a region of points
    /// </summary>
    public class PointsRegion
    {
        public IEnumerable<PointBase> Points { get; set; }

        public int Row { get; set; }

        public int Column { get; set; }
    }

    /// <summary>
    /// Use this class to model a list of regions which are at the same level
    /// </summary>
    public class PointsRegionsLevel
    {
        public int ZoomLevel { get; set; }

        public IEnumerable<PointsRegion> Regions { get; set; }
    }
}
