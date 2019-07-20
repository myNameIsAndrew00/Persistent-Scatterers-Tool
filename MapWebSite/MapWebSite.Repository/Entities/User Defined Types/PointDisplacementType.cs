using MapWebSite.Core;
using System;
 

namespace MapWebSite.Repository.Entities
{
    [UserDefinedType]
    public class PointDisplacementType 
    {
        public int point_number { get; set; }

        public DateTime displacement_date { get; set; }

        public float displacement_JD { get; set; }

        public float days_from_reference { get; set; }

        public float displacement_value { get; set; }
    }
}
