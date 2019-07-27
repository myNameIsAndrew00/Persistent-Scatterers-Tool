
using FastMember;
using MapWebSite.Core;
using MapWebSite.Core.Database;
using MapWebSite.Model;
using MapWebSite.Repository.Entities;
using MapWebSite.SQLAccess;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace MapWebSite.Repository
{
    public class SQLDataPointsRepository : SQLBaseRepository, IDataPointsRepository
    {
        public bool InsertPointsDataset(PointsDataSet pointsDataset, string username)
        {
            try
            {
                DBPointsDataSet dataBasePointsDataset = (DBPointsDataSet)pointsDataset;

                DataTable pointsTable = new PointType().GetDataTableFromProperties();             
                DataTable pointsDisplacementsTable = new PointDisplacementType().GetDataTableFromProperties();

                loadTables(pointsTable, pointsDisplacementsTable, dataBasePointsDataset);
             
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



        #region Private

        private void loadTables(DataTable pointsTable, DataTable pointsDisplacementsTable, DBPointsDataSet dataBasePointsDataset)
        {
            Task[] populateTableTasks = new Task[2];

            populateTableTasks[0] = Task.Run(() =>
            {
                using (var objectReader = ObjectReader.Create(dataBasePointsDataset.Points))
                {
                    pointsTable.Load(objectReader);
                }
            });

            populateTableTasks[1] = Task.Run(() =>
            {
                using (var objectReader = ObjectReader.Create(dataBasePointsDataset.PointsDisplacements))
                {
                    pointsDisplacementsTable.Load(objectReader);
                }
            });

            Task.WaitAll(populateTableTasks);
        }

        #endregion
    }
}
