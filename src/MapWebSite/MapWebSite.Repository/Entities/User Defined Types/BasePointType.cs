using MapWebSite.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Repository.Entities
{
    [UserDefinedType]
    public class BasePointType
    {

        [UserDefinedTypeColumn("number")]
        public int point_number { get; set; }

        public decimal reference_image_x { get; set; }

        public decimal reference_image_y { get; set; }


        [UserDefinedTypeColumn("longitude")]
        public decimal longitude { get; set; }


        [UserDefinedTypeColumn("latitude")]
        public decimal latitude { get; set; }


        [UserDefinedTypeColumn("height")]
        public decimal height { get; set; }


        [UserDefinedTypeColumn("deformation_rate")]
        public decimal deformation_rate { get; set; }


        [UserDefinedTypeColumn("standard_deviation")]
        public decimal standard_deviation { get; set; }


        [UserDefinedTypeColumn("estimated_height")]
        public decimal estimated_height { get; set; }


        [UserDefinedTypeColumn("estimated_deformation_rate")]
        public decimal estimated_deformation_rate { get; set; }


        [UserDefinedTypeColumn("observations")]
        public string observations { get; set; }

    }
}
