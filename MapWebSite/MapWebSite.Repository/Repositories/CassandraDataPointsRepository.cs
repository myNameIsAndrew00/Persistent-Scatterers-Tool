using Cassandra;
using MapWebSite.CassandraAccess;
using MapWebSite.Core.Database;
using MapWebSite.Model; 
using MapWebSite.Repository.Entities;
using System;
using System.Collections.Async;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapWebSite.Repository
{
    using BuilderTuple = Tuple<string, string, CassandraQueryBuilder.Clauses>;

    public class CassandraDataPointsRepository : CassandraBaseRepository, IDataPointsRepository
    {
        public async Task<bool> InsertPointsDatasets(PointsDataSet originalDataSet, PointsDataSet[] zoomedDatasets)
        {
            bool success = await this.insertPointsDataset(originalDataSet);

            if (!success) return false;

            foreach (var pointsDataset in zoomedDatasets)
            {
                success = await this.insertPointsDataset(pointsDataset);
                if (!success) return false;
            }

            //TODO: clear the data if not success
            return true;
        }

        public IEnumerable<BasicPoint> GetDataPointsBasicInfo(int dataSetID, int zoomLevel, Tuple<decimal, decimal> from, Tuple<decimal, decimal> to)
        {
            var dataRows = selectBasicPointsDataset(dataSetID, zoomLevel, from, to);

            return convertRowSetToBasicPointList(dataRows);            
        }


        public Point GetPointDetails(int dataSetID, int zoomLevel, BasicPoint basicPoint)
        {
            var dataRows = selectPointDetails(dataSetID, zoomLevel, basicPoint);

            return convertRowSetToPointList(dataRows).FirstOrDefault();
        }

     

        #region Private

        private async Task<bool> insertPointsDataset(PointsDataSet pointsDataset)
        {
            IEnumerable<PointType> pointTypes = PointType.GetPoints(pointsDataset);

            CassandraQueryBuilder queryBuilder = new CassandraQueryBuilder();
            queryBuilder.TableName = pointsDataset.ZoomLevel == 0 ? "points_by_dataset" : $"points_by_dataset_zoom_{pointsDataset.ZoomLevel}";
            queryBuilder.Type = typeof(PointType);
            queryBuilder.QueryType = CassandraQueryBuilder.QueryTypes.InsertFromType;
            if (pointsDataset.ZoomLevel != 0) queryBuilder.IgnoredColumnNames = new List<string>() { "displacements" };

            using (CassandraExecutionInstance executionInstance = new CassandraExecutionInstance(this.server,
                                                                                this.keyspace))
            {
                executionInstance.UserDefinedTypeMappings.Define(UdtMap.For<PointDisplacementType>("points_displacements"));

                executionInstance.PrepareQuery(queryBuilder);

                try
                {
                    await pointTypes.ParallelForEachAsync(async pointType =>
                    {
                        await executionInstance.ExecuteNonQuery(new
                        {
                            pointType.dataset_id,
                            number = pointType.point_number,
                            pointType.longitude,
                            pointType.latitude,
                            pointType.height,
                            pointType.deformation_rate,
                            pointType.standard_deviation,
                            pointType.estimated_height,
                            pointType.estimated_deformation_rate,
                            pointType.observations,
                            pointType.displacements
                        });
                    });
                }
                catch (Exception exception)
                {
                    //TODO: log exception
                }
            }

            return true;
        }

        private List<Row> selectBasicPointsDataset(int dataSetID, int zoomLevel, Tuple<decimal, decimal> from, Tuple<decimal, decimal> to)
        {
            CassandraQueryBuilder queryBuilder = new CassandraQueryBuilder()
            {
                QueryType = CassandraQueryBuilder.QueryTypes.Select
            };
            queryBuilder.TableName = zoomLevel == 0 ? "points_by_dataset" : $"points_by_dataset_zoom_{zoomLevel}";
            queryBuilder.SelectColumnNames = new List<string>() { "latitude", "longitude", "number" };
            queryBuilder.ClausesList.Add(new BuilderTuple("dataSetID", "dataset_id", CassandraQueryBuilder.Clauses.Equals));
            queryBuilder.ClausesList.Add(new BuilderTuple("leftLatitude", "latitude", CassandraQueryBuilder.Clauses.GreaterOrEqual));
            queryBuilder.ClausesList.Add(new BuilderTuple("leftLongitude", "longitude", CassandraQueryBuilder.Clauses.GreaterOrEqual));
            queryBuilder.ClausesList.Add(new BuilderTuple("rightLatitude", "latitude", CassandraQueryBuilder.Clauses.Less));
            queryBuilder.ClausesList.Add(new BuilderTuple("rightLongitude", "longitude", CassandraQueryBuilder.Clauses.Less));

            using (CassandraExecutionInstance executionInstance = new CassandraExecutionInstance(this.server,
                                                                                 this.keyspace))
            {
                executionInstance.UserDefinedTypeMappings.Define(UdtMap.For<PointDisplacementType>("points_displacements"));

                executionInstance.PrepareQuery(queryBuilder);

                return executionInstance.ExecuteQuery(new
                {
                    dataSetID,
                    leftLatitude = from.Item1,
                    leftLongitude = from.Item2,
                    rightLatitude = to.Item1,
                    rightLongitude = to.Item2
                });
            }
        }

        private List<Row> selectPointDetails(int dataSetID, int zoomLevel, BasicPoint basicPoint)
        {
            CassandraQueryBuilder queryBuilder = new CassandraQueryBuilder()
            {
                QueryType = CassandraQueryBuilder.QueryTypes.Select
            };
            queryBuilder.TableName = zoomLevel == 0 ? "points_by_dataset" : $"points_by_dataset_zoom_{zoomLevel}";
            queryBuilder.ClausesList.Add(new BuilderTuple("dataSetID", "dataset_id", CassandraQueryBuilder.Clauses.Equals));
            queryBuilder.ClausesList.Add(new BuilderTuple("latitude", "latitude", CassandraQueryBuilder.Clauses.Equals));
            queryBuilder.ClausesList.Add(new BuilderTuple("longitude", "longitude", CassandraQueryBuilder.Clauses.Equals));
            queryBuilder.ClausesList.Add(new BuilderTuple("number", "number", CassandraQueryBuilder.Clauses.Equals));
            using (CassandraExecutionInstance executionInstance = new CassandraExecutionInstance(this.server,
                                                                                 this.keyspace))
            {
                executionInstance.UserDefinedTypeMappings.Define(UdtMap.For<PointDisplacementType>("points_displacements"));

                executionInstance.PrepareQuery(queryBuilder);

                return executionInstance.ExecuteQuery(new
                {
                    dataSetID,
                    latitude = basicPoint.Latitude,
                    longitude = basicPoint.Longitude,
                    number = basicPoint.Number 
                });
            }
        }


        private IEnumerable<BasicPoint> convertRowSetToBasicPointList(List<Row> rowSet)
        {
            ConcurrentBag<BasicPoint> result = new ConcurrentBag<BasicPoint>();
            Parallel.ForEach(rowSet, row =>
            {
                result.Add(new BasicPoint()
                {                  
                    Latitude = Convert.ToDecimal(row["latitude"]),
                    Longitude = Convert.ToDecimal(row["longitude"]),
                    Number = Convert.ToInt32(row["number"])            
                });
            });

            return result;
        }

        private IEnumerable<Point> convertRowSetToPointList(List<Row> rowSet)
        {
        
            ConcurrentBag<Point> result = new ConcurrentBag<Point>();
            Parallel.ForEach(rowSet, row =>
            {
                result.Add(new Point()
                {
                    DeformationRate = Convert.ToDecimal(row["deformation_rate"]),
                    EstimatedDeformationRate = Convert.ToDecimal(row["estimated_deformation_rate"]),
                    Height = Convert.ToDecimal(row["height"]),
                    EstimatedHeight = Convert.ToDecimal(row["estimated_height"]),
                    Latitude = Convert.ToDecimal(row["latitude"]),
                    Longitude = Convert.ToDecimal(row["longitude"]),
                    Number = Convert.ToInt32(row["number"]),
                    Observations = row["observations"]?.ToString(),
                    StandardDeviation = Convert.ToDecimal(row["standard_deviation"]),
                    Displacements = row.GetColumn("displacements") == null ? null : convertDisplacements(row["displacements"] as PointDisplacementType[])
                });
            });

            return result;
        }

        private List<Point.Displacement> convertDisplacements(PointDisplacementType[] displacements)
        {
            List<Point.Displacement> result = new List<Point.Displacement>();
            foreach (var displacement in displacements)
                result.Add(new Point.Displacement()
                {
                    Date = displacement.date.DateTime,
                    DaysFromReference = displacement.days_from_reference,
                    JD = displacement.jd,
                    Value = displacement.value
                });

            return result;
        }

        #endregion
    }
}
