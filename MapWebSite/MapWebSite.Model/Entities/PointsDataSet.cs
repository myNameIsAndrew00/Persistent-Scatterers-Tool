using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Model
{
    public class PointsDataSet
    {
        public string Name { get; set; }

        public IEnumerable<Point> Points { get; set; }

    }

    public class Point
    {
        public int Number { get; set; }

        public float ReferenceImageX { get; set; }

        public float ReferenceImageY { get; set; }

        public float EastingProjectionCoordinate { get; set; }

        public float NorthingProjectionCoordinate { get; set; }

        public float Height { get; set; }

        public float DeformationRate { get; set; }

        public float StandardDeviation { get; set; }

        public float EstimatedHeight { get; set; }

        public float EstimatedDeformationRate { get; set; }

        public string Observations { get; set; }

        public List<Displacement> Displacements { get; set; }
    }

    public class Displacement
    {
        public DateTime Date { get; set; }

        public float JD { get; set; }

        public float DaysFromReference { get; set; }

        public float Value { get; set; }
    }

}
