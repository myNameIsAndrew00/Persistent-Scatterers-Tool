using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Model
{
    public class PointsDataSet
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public IEnumerable<Point> Points { get; set; }

    }

    public class Point
    {
        public int Number { get; set; }

        public decimal ReferenceImageX { get; set; }

        public decimal ReferenceImageY { get; set; }

        public decimal Longitude { get; set; }

        public decimal Latitude { get; set; }

        public decimal Height { get; set; }

        public decimal DeformationRate { get; set; }

        public decimal StandardDeviation { get; set; }

        public decimal EstimatedHeight { get; set; }

        public decimal EstimatedDeformationRate { get; set; }

        public string Observations { get; set; }

        public List<Displacement> Displacements { get; set; }
    }

    public class Displacement
    {
        public DateTime Date { get; set; }

        public decimal JD { get; set; }

        public decimal DaysFromReference { get; set; }

        public decimal Value { get; set; }
    }

}
