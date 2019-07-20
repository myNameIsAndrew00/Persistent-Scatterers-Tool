
using MapWebSite.Core;
using MapWebSite.Repository.Entities;
using MapWebSite.SQLAccess;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace MapWebSite.Repository
{
    public class DataPointsRepository : BaseRepository
    {
        public bool InsertPointsDataset(DBPointsDataSet pointsDataset, string username)
        {
            try
            {
                DataTable pointsTable = new PointType().GetDataTableFromProperties();
                DataTable pointsDisplacementsTable = new PointDisplacementType().GetDataTableFromProperties();

                Parallel.ForEach(pointsDataset.Points, (point) =>
                {
                    pointsTable.Rows.Add(new {
                        point.point_number,
                        point.reference_image_x,
                        point.reference_image_y,
                        point.easting_projection_coordinate,
                        point.northing_projection_coordinate,
                        point.height,
                        point.deformation_rate,
                        point.standard_deviation,
                        point.estimated_height,
                        point.estimated_deformation_rate,
                        point.observations
                    });
                });

                Parallel.ForEach(pointsDataset.PointsDisplacements, (pointDisplacement) =>
                {
                    pointsDisplacementsTable.Rows.Add(new {
                        pointDisplacement.point_number,
                        pointDisplacement.displacement_date,
                        pointDisplacement.displacement_JD,
                        pointDisplacement.days_from_reference,
                        pointDisplacement.displacement_value
                    });
                });

                SqlExecutionInstance.ExecuteNonQuery(new SqlCommand("InsertPointsDataset")
                                                    {
                                                        CommandType = CommandType.StoredProcedure
                                                    },
                                                    new SqlParameter[] {
                                                        new SqlParameter("username",username),
                                                        new SqlParameter("dataset_name", pointsDataset.Name),
                                                        new SqlParameter("dataset_points", pointsTable),
                                                        new SqlParameter("dataset_points_displacements", pointsDisplacementsTable)
                                                    },
                                                    new SqlConnection(this.connectionString));
            }
            catch(Exception exception)
            {
                //TODO: log exception;
                return false;
            }

            return true;
        }
    }
}
