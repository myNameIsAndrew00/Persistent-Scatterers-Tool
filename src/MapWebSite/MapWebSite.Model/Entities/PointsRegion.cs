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
        public int ZoomLevel { get; set; }

        public IEnumerable<PointBase> Points { get; set; }
    }
}
