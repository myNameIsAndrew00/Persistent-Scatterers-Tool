using System.Collections.Generic;

namespace MapWebSite.Model
{

    public struct Interval
    {
        public string Color { get; set; }

        public decimal Left { get; set; }

        public decimal Right { get; set; }

    }

    public class ColorMap
    {
        public string Name { get; set; }

        public List<Interval> Intervals { get; set; }
    }
}
