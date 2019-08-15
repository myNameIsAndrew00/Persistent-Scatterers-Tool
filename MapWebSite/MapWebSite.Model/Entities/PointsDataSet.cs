using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MapWebSite.Model
{


    public class PointsDataSet
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public long ZoomLevel { get; set; }

        public IEnumerable<Point> Points { get; set; }

    }

    [DataContract]
    [JsonObject(MemberSerialization.OptIn)]
    public class Point  
    {
        public int Number { get; set; }

        public decimal ReferenceImageX { get; set; }

        public decimal ReferenceImageY { get; set; }

        [DataMember]
        [JsonProperty]
        public decimal Longitude { get; set; }

        [DataMember]
        [JsonProperty]
        public decimal Latitude { get; set; }

        public decimal Height { get; set; }

        public decimal DeformationRate { get; set; }

        public decimal StandardDeviation { get; set; }

        public decimal EstimatedHeight { get; set; }

        public decimal EstimatedDeformationRate { get; set; }

        public string Observations { get; set; }

        public List<Displacement> Displacements { get; set; }

   


        public class Displacement
        {
            public DateTime Date { get; set; }

            public decimal JD { get; set; }

            public decimal DaysFromReference { get; set; }

            public decimal Value { get; set; }
        }
    }

   

}
