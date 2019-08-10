using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Model
{

    public struct Interval
    {
        public string Color { get; set; }

        public int Left { get; set; }

        public int Right { get; set; }

    }

    public class ColorMap
    {
        public string Name { get; set; }

        public List<Interval> Intervals { get; set; }
    }
}
