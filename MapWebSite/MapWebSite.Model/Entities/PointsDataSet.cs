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
    [JsonObject(MemberSerialization.OptIn)]
    [DataContract] 
    public class BasicPoint
    {
        [DataMember] 
        [JsonProperty]
        public int Number { get; set; }

        [DataMember]
        [JsonProperty]
        public decimal Longitude { get; set; }

        [DataMember]
        [JsonProperty]
        public decimal Latitude { get; set; }

    }
    [JsonObject(MemberSerialization.OptIn)]
    [DataContract] 
    public class Point  : BasicPoint
    { 
        [JsonProperty]
        public decimal ReferenceImageX { get; set; }

        [JsonProperty]
        public decimal ReferenceImageY { get; set; }

        [JsonProperty]
        public decimal Height { get; set; }

        [JsonProperty]
        public decimal DeformationRate { get; set; }

        [JsonProperty]
        public decimal StandardDeviation { get; set; }

        [JsonProperty]
        public decimal EstimatedHeight { get; set; }

        [JsonProperty]
        public decimal EstimatedDeformationRate { get; set; }

        [JsonProperty]
        public string Observations { get; set; }

        [JsonProperty]
        public List<Displacement> Displacements { get; set; }


        [JsonObject(MemberSerialization.OptIn)]
        public class Displacement
        {
            [JsonProperty]
            public DateTime Date { get; set; }

            [JsonProperty]
            public decimal JD { get; set; }

            [JsonProperty]
            public decimal DaysFromReference { get; set; }

            [JsonProperty]
            public decimal Value { get; set; }
        }
    }

   

}
