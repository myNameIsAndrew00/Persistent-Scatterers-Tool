using MapWebSite.Core;

namespace MapWebSite.Repository.Entities 
{
    [UserDefinedType]
    internal class PointType 
    {
        public int point_number { get; set; }

        public float reference_image_x { get; set; }

        public float reference_image_y { get; set; }

        public float easting_projection_coordinate { get; set; }

        public float northing_projection_coordinate { get; set; }

        public float height { get; set; }

        public float deformation_rate { get; set; }

        public float standard_deviation { get; set; }

        public float estimated_height { get; set; }

        public float estimated_deformation_rate { get; set; }

        public string observations { get; set; }
    }
}
