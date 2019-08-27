using MapWebSite.Model;
using MapWebSite.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("MapWebSite.Tests")]

namespace MapWebSite.Repository.Entities
{
    [UserDefinedType]
    internal class PointType
    {

        [UserDefinedTypeColumn("dataset_id")]
        public int dataset_id { get; set; }


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


        [UserDefinedTypeColumn("displacements")]
        public IEnumerable<PointDisplacementType> displacements { get; set; }


        public static IEnumerable<PointType> GetPoints(PointsDataSet pointsDatasetModel)
        {
            ConcurrentBag<PointType> result = new ConcurrentBag<PointType>();

            Parallel.ForEach(pointsDatasetModel.Points, (pointModel) =>
            {
                var point = new PointType()
                {
                    deformation_rate = pointModel.DeformationRate,
                    latitude = pointModel.Latitude,
                    longitude = pointModel.Longitude,
                    estimated_deformation_rate = pointModel.EstimatedDeformationRate,
                    standard_deviation = pointModel.StandardDeviation,
                    estimated_height = pointModel.EstimatedHeight,
                    height = pointModel.Height,
                    observations = pointModel.Observations,
                    point_number = pointModel.Number,
                    reference_image_x = pointModel.ReferenceImageX,
                    reference_image_y = pointModel.ReferenceImageY,
                    dataset_id = pointsDatasetModel.ID,
                    displacements = new ConcurrentBag<PointDisplacementType>()
                };

                ConcurrentBag<PointDisplacementType> displacements = point.displacements as ConcurrentBag<PointDisplacementType>;

                if (pointModel.Displacements != null)
                Parallel.ForEach(pointModel.Displacements, (displacement) =>
                {
                    displacements.Add(new PointDisplacementType()
                    {
                        days_from_reference = displacement.DaysFromReference,
                        date = displacement.Date,
                        jd = displacement.JD,
                        value = displacement.Value,
                    });
                });
                 

                result.Add(point);
            });

            return result;
        }
         
    }

}
