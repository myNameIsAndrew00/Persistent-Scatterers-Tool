using System.Collections.Generic;

namespace MapWebSite.Model
{

    public struct Interval
    {
        public string Color { get; set; }

        public decimal Left { get; set; }

        public decimal Right { get; set; }

    }

    /// <summary>
    /// Model used for a color palette
    /// </summary>
    public class ColorMap
    {
        public string Name { get; set; }

        public List<Interval> Intervals { get; set; }
    }
}
