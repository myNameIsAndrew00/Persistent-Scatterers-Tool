
using MapWebSite.Core;
using MapWebSite.Model;
using System.Collections.Generic;

namespace MapWebSite.Repository.Entities
{
    internal class DBPointsDataSet
    {
        public string Name { get; set; }

        public PointType[] Points { get; set; }

        public PointDisplacementType[] PointsDisplacements { get; set; }

        public static explicit operator DBPointsDataSet(PointsDataSet pointsDataSet)
        {          
            List<PointType> points = new List<PointType>();
            List<PointDisplacementType> pointsDisplacements = new List<PointDisplacementType>();

            foreach(var point in pointsDataSet.Points)
            {
                points.Add(new PointType()
                {
                    deformation_rate = point.DeformationRate,
                    easting_projection_coordinate = point.EastingProjectionCoordinate,
                    northing_projection_coordinate = point.NorthingProjectionCoordinate,
                    estimated_deformation_rate = point.EstimatedDeformationRate,
                    standard_deviation = point.StandardDeviation,
                    estimated_height = point.EstimatedHeight,
                    height = point.Height,
                    observations = point.Observations,
                    point_number = point.Number,
                    reference_image_x = point.ReferenceImageX,
                    reference_image_y = point.ReferenceImageY
                });

                foreach (var pointDisplacement in point.Displacements)
                    pointsDisplacements.Add(new PointDisplacementType()
                    {
                        days_from_reference = pointDisplacement.DaysFromReference,
                        displacement_date = pointDisplacement.Date,
                        displacement_JD = pointDisplacement.JD,
                        displacement_value = pointDisplacement.Value,
                        point_number = point.Number
                    });
            }

            return new DBPointsDataSet()
            {
                Name = pointsDataSet.Name,
                PointsDisplacements = pointsDisplacements.ToArray(),
                Points = points.ToArray()
            }; 
        }

    }

}
