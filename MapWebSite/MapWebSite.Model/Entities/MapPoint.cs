using System.Runtime.Serialization;


namespace MapWebSite.Model.Entities
{
    [DataContract]
    public class MapPoint
    {
        [DataMember]
        public double Longitude { get; set; }

        [DataMember]
        public double Latitude { get; set; }
    }
}
