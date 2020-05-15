using MapWebSite.Core.Database;
using MapWebSite.Model;
using MapWebSite.SQLAccess;
using SqlKata;
using SqlKata.Compilers;
using System;
using System.Collections.Generic;
using Npgsql;
using System.Threading.Tasks;
using System.Data;
using MapWebSite.Types;

namespace MapWebSite.Repository
{
    public class PostgreSQLDataPointsRepository : SQLBaseRepository, IDataPointsRepository
    {

        private static class Tables
        {
            public static readonly string metadataTableName = "tables_time_references";
           
            public enum MetadataTableColumns
            {
                [UserDefinedTypeColumn("number")]
                Number,

                [UserDefinedTypeColumn("referencex")]
                ReferenceX,

                [UserDefinedTypeColumn("referencey")]
                ReferenceY,

                [UserDefinedTypeColumn("latitude")]
                Latitude,

                [UserDefinedTypeColumn("longitude")]
                Longitude,

                [UserDefinedTypeColumn("height")]
                Height,

                [UserDefinedTypeColumn("deformati")]
                DeformationRate,

                [UserDefinedTypeColumn("standardde")]
                StandardDeviation,

                [UserDefinedTypeColumn("estimatedh")]
                EstimatedHeight,

                [UserDefinedTypeColumn("estimatedd")]
                EstimatedDeformationRate
            }

        }


        public PostgreSQLDataPointsRepository() : base("PostgreSQLDatabase") { }

        public IEnumerable<PointBase> GetBasePoints(int dataSetID, Tuple<decimal, decimal> from, Tuple<decimal, decimal> to)
        {
            throw new NotImplementedException();
        }

        public Point GetPointDetails(int dataSetID, PointBase basicPoint)
        {
            Query query = new Query(Tables.metadataTableName)
                            .Select("table_names")
                            .Where("data_set_id",dataSetID);
            SqlResult queryResult = new SqlServerCompiler().Compile(query);
            string tableName = SqlExecutionInstance.ExecuteScalar
                (new NpgsqlCommand(queryResult.ToString().Replace("[","").Replace("]","")),
                                                null,
                                                 new NpgsqlConnection(this.connectionString))?.ToString();

            if (tableName == null) return null;

            query = new Query(Tables.metadataTableName)
                            .Select("time_references")
                            .Where("data_set_id", dataSetID);
            queryResult = new SqlServerCompiler().Compile(query);
            long[] timeReferences = (long[])SqlExecutionInstance.ExecuteScalar
               (new NpgsqlCommand(queryResult.ToString().Replace("[", "").Replace("]", "")),
                                               null,
                                                new NpgsqlConnection(this.connectionString));

            if (timeReferences == null) return null;


            List<string> queryColumns = new List<string>();
            for (int i = 0; i < timeReferences.Length; i++)
                queryColumns.Add($"d_{i}");
            queryColumns.AddRange(
                    UserDefinedTypeAttributeExtensions.GetUserDefinedColumnsNames(typeof(Tables.MetadataTableColumns)));

            query = new Query(tableName)
                .Select(queryColumns.ToArray())
                .WhereRaw("geom && ST_Expand(ST_SetSRID(ST_MakePoint(?,?),4326),100)", basicPoint.Longitude, basicPoint.Latitude)
                .OrderByRaw("ST_SetSRID(ST_MakePoint(?,?),4326) <-> geom", basicPoint.Longitude, basicPoint.Latitude);
                

            queryResult = new SqlServerCompiler().Compile(query);

            using (var datasetResult = SqlExecutionInstance.ExecuteQuery(
                                                    new NpgsqlCommand(queryResult.ToString()
                                                                      .Replace("[", "")
                                                                      .Replace("]", "")
                                                                      + " limit 1"),
                                                    null,
                                                    new NpgsqlConnection(this.connectionString),
                                                    (command) =>
                                                    {
                                                        return new NpgsqlDataAdapter((NpgsqlCommand)command);
                                                    }))
            {
                return parsePointDetails(datasetResult.Tables[0].Rows, timeReferences);
            }
        }

    
        public PointsRegion GetRegion(int datasetId, int row, int column, int zoomLevel)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PointsRegion> GetRegions(int datasetId, Tuple<int, int> from, Tuple<int, int> to, int zoomLevel)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertPointsDataset(PointsDataSet originalDataSet)
        {
            throw new NotImplementedException();
        }

        #region Private

        private Point parsePointDetails(DataRowCollection rows, long[] timeReferences)
        {
            DataRow firstRow = rows[0];

            var result = new Point
            {
                Height = Convert.ToDecimal(firstRow["height"]),
                Latitude = Convert.ToDecimal(firstRow[Tables.MetadataTableColumns.Latitude.GetUserDefinedNameInDatabase()]),
                Longitude = Convert.ToDecimal(firstRow[Tables.MetadataTableColumns.Longitude.GetUserDefinedNameInDatabase()]),
                DeformationRate = Convert.ToDecimal(firstRow[Tables.MetadataTableColumns.DeformationRate.GetUserDefinedNameInDatabase()]),
                EstimatedDeformationRate = Convert.ToDecimal(firstRow[Tables.MetadataTableColumns.EstimatedDeformationRate.GetUserDefinedNameInDatabase()]),
                EstimatedHeight = Convert.ToDecimal(firstRow[Tables.MetadataTableColumns.EstimatedHeight.GetUserDefinedNameInDatabase()]),
                Number = Convert.ToInt32(firstRow[Tables.MetadataTableColumns.Number.GetUserDefinedNameInDatabase()]),
                ReferenceImageX = Convert.ToDecimal(firstRow[Tables.MetadataTableColumns.ReferenceX.GetUserDefinedNameInDatabase()]),
                ReferenceImageY = Convert.ToDecimal(firstRow[Tables.MetadataTableColumns.ReferenceY.GetUserDefinedNameInDatabase()]),
                StandardDeviation = Convert.ToDecimal(firstRow[Tables.MetadataTableColumns.StandardDeviation.GetUserDefinedNameInDatabase()]),
                Displacements = new List<Point.Displacement>()
            };

            for (int i = 0; i < timeReferences.Length; i++)
                result.Displacements.Add(new Point.Displacement()
                {
                    JD = timeReferences[i],
                    DaysFromReference = timeReferences[i],
                    Value = Convert.ToDecimal(firstRow[$"d_{i}"])
                });

            return result;

        }



        #endregion

    }
}
