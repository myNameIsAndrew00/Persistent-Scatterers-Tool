using MapWebSite.Model;
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
        public int number { get; set; }

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


        public static explicit operator BasePointType(PointBase point)
        {
            return new BasePointType()
            {
                deformation_rate = point.DeformationRate,
                estimated_deformation_rate = point.EstimatedDeformationRate,
                estimated_height = point.EstimatedHeight,
                height = point.Height,
                latitude = point.Latitude,
                longitude = point.Longitude,
                number = point.Number,
                observations = point.Observations,
                standard_deviation = point.StandardDeviation
            };
        }
    }
}
