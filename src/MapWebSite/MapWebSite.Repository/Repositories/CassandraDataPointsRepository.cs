using Cassandra;
using MapWebSite.CassandraAccess;
using MapWebSite.Core.Database;
using MapWebSite.Model;
using MapWebSite.Repository.Entities;
using System;
using Dasync.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MapWebSite.Repository
{
    using BuilderTuple = Tuple<string, string, CassandraQueryBuilder.Clauses>;

    public class CassandraDataPointsRepository : CassandraBaseRepository, IDataPointsRepository
    {
        private CassandraExecutionInstance executionInstance = null;

        public static CassandraDataPointsRepository Instance { get; set; }

        public static void Initialise()
        {
            Instance = new CassandraDataPointsRepository();
        }

        private CassandraDataPointsRepository()
        {
            try
            {
                this.executionInstance = new CassandraExecutionInstance(this.server, this.keyspace);
                executionInstance.UserDefinedTypeMappings.Define(UdtMap.For<PointDisplacementType>("points_displacements"));
                executionInstance.UserDefinedTypeMappings.Define(UdtMap.For<BasePointType>("base_point"));
            }
            catch (Exception exception)
            {
                //TODO: log exception
            }
        }

        public async Task<bool> InsertPointsDataset(PointsDataSet dataSet)
        {
            bool success = await this.insertPointsDataset(dataSet);

            if (!success) return false;

            foreach (var regionsLevel in dataSet.PointsRegions)
            {
                if (regionsLevel.Regions.Count() == 0) continue;

                success = await this.insertRegion(regionsLevel, dataSet.ID);
                if (!success) return false;
            }


            //TODO: clear the data if not success
            return true;
        }

        public IEnumerable<PointBase> GetBasePoints(int dataSetID, Tuple<decimal, decimal> from, Tuple<decimal, decimal> to)
        {

            var dataRows = selectBasicPointsDataset(dataSetID, from, to);

            return convertRowSetToBasicPointList(dataRows);
        }


        public Point GetPointDetails(int dataSetID, PointBase basicPoint)
        {
            var dataRows = selectPointDetails(dataSetID, basicPoint);

            return convertRowSetToPointList(dataRows).FirstOrDefault();
        }

        public IEnumerable<PointsRegion> GetRegions(int datasetId, Tuple<int, int> from, Tuple<int, int> to, int zoomLevel)
        {
            var dataRows = selectRegions(datasetId, from, to, zoomLevel);

            return convertRowSetToPointsRegions(dataRows);
        }


        public PointsRegion GetRegion(int datasetId, int row, int column, int zoomLevel)
        {
            var dataRows = selectRegion(datasetId, row, column, zoomLevel);

            return convertRowSetToPointsRegions(dataRows)?.FirstOrDefault();
        }

        #region Private

        private async Task<bool> insertRegion(PointsRegionsLevel regionLevel, int datasetId)
        {
            IEnumerable<SectionedPointsRegionType> regionTypes = PointsRegionType.GetRegions(regionLevel, datasetId);

            CassandraQueryBuilder queryBuilder = new CassandraQueryBuilder();
            queryBuilder.TableName = "points_region_zoom_" + regionLevel.ZoomLevel;
            queryBuilder.Type = typeof(SectionedPointsRegionType);
            queryBuilder.QueryType = CassandraQueryBuilder.QueryTypes.InsertFromType;

            this.executionInstance.PrepareQuery(queryBuilder);

            try
            {
                ConcurrentQueue<Exception> exceptions = new ConcurrentQueue<Exception>();

                await regionTypes.ParallelForEachAsync(async regionType =>
                {
                    try
                    {
                        await executionInstance.ExecuteNonQuery(new
                        {
                            regionType.dataset_id,
                            regionType.column,
                            regionType.row,
                            regionType.points,
                            regionType.section
                        });
                    }
                    catch (Exception exception)
                    {
                        exceptions.Enqueue(exception);
                    }
                });
                if (exceptions.Count > 0) throw new Exception("Exceptions catched in tasks", exceptions.First());
            }
            catch (Exception exception)
            {
                return false;
                //todo: log exception
            }

            return true;
        }


        private async Task<bool> insertPointsDataset(PointsDataSet pointsDataset)
        {
            IEnumerable<PointType> pointTypes = PointType.GetPoints(pointsDataset);

            CassandraQueryBuilder queryBuilder = new CassandraQueryBuilder();
            queryBuilder.TableName = "points_by_dataset";
            queryBuilder.Type = typeof(PointType);
            queryBuilder.QueryType = CassandraQueryBuilder.QueryTypes.InsertFromType;
            // if (pointsDataset.ZoomLevel != 0) queryBuilder.IgnoredColumnNames = new List<string>() { "displacements" };



            this.executionInstance.PrepareQuery(queryBuilder);

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


            return true;
        }

        private List<Row> selectBasicPointsDataset(int dataSetID, Tuple<decimal, decimal> from, Tuple<decimal, decimal> to)
        {
            CassandraQueryBuilder queryBuilder = new CassandraQueryBuilder()
            {
                QueryType = CassandraQueryBuilder.QueryTypes.Select
            };
            queryBuilder.TableName = "points_by_dataset";
            queryBuilder.SelectColumnNames = new List<string>() { "latitude",
                                                                  "longitude",
                                                                  "number",
                                                                  "height",
                                                                  "deformation_rate",
                                                                  "standard_deviation",
                                                                  "estimated_height",
                                                                  "estimated_deformation_rate" };
            queryBuilder.ClausesList.Add(new BuilderTuple("dataSetID", "dataset_id", CassandraQueryBuilder.Clauses.Equals));
            queryBuilder.ClausesList.Add(new BuilderTuple("leftLatitude", "latitude", CassandraQueryBuilder.Clauses.GreaterOrEqual));
            queryBuilder.ClausesList.Add(new BuilderTuple("leftLongitude", "longitude", CassandraQueryBuilder.Clauses.GreaterOrEqual));
            queryBuilder.ClausesList.Add(new BuilderTuple("rightLatitude", "latitude", CassandraQueryBuilder.Clauses.LessOrEqual));
            queryBuilder.ClausesList.Add(new BuilderTuple("rightLongitude", "longitude", CassandraQueryBuilder.Clauses.LessOrEqual));



            executionInstance.PrepareQuery(queryBuilder);

            return executionInstance.ExecuteQuery(new
            {
                dataSetID,
                leftLatitude = from.Item1,
                leftLongitude = from.Item2,
                rightLatitude = to.Item1,
                rightLongitude = to.Item2
            }).Result;

        }

        private List<Row> selectPointDetails(int dataSetID, PointBase basicPoint)
        {
            CassandraQueryBuilder queryBuilder = new CassandraQueryBuilder()
            {
                QueryType = CassandraQueryBuilder.QueryTypes.Select
            };

            //update: do not use latitude and longitude to search for points details

            queryBuilder.TableName = "points_by_dataset";
            queryBuilder.ClausesList.Add(new BuilderTuple("dataSetID", "dataset_id", CassandraQueryBuilder.Clauses.Equals));
            queryBuilder.ClausesList.Add(new BuilderTuple("number", "number", CassandraQueryBuilder.Clauses.Equals));

            executionInstance.UserDefinedTypeMappings.Define(UdtMap.For<PointDisplacementType>("points_displacements"));

            var preparedStatement = executionInstance.GetPreparedStatement("points_by_dataset", queryBuilder);

            var result = executionInstance.ExecuteQuery(new
            {
                dataSetID,
                number = basicPoint.Number
            }, preparedStatement).Result;

            return result;
        }
 
        private List<Row> selectRegion(int datasetId, int row, int column, int zoomLevel)
        {
            var preparedStatement = executionInstance.GetPreparedStatement("points_region_zoom_" + zoomLevel);

            if (preparedStatement == null)
            {
                CassandraQueryBuilder queryBuilder = new CassandraQueryBuilder()
                {
                    QueryType = CassandraQueryBuilder.QueryTypes.Select
                };

                queryBuilder.TableName = "points_region_zoom_" + zoomLevel;
                queryBuilder.SelectColumnNames = new List<string>() { "row",
                                                                  "column",
                                                                  "points" };
                queryBuilder.ClausesList.Add(new BuilderTuple("datasetId", "dataset_id", CassandraQueryBuilder.Clauses.Equals));
                queryBuilder.ClausesList.Add(new BuilderTuple("row", "row", CassandraQueryBuilder.Clauses.Equals));
                queryBuilder.ClausesList.Add(new BuilderTuple("column", "column", CassandraQueryBuilder.Clauses.Equals));

                preparedStatement = executionInstance.GetPreparedStatement("points_region_zoom_" + zoomLevel, queryBuilder);            
            }
         

            var result = executionInstance.ExecuteQuery(new
            {
                datasetId,
                row,
                column
            },
            preparedStatement).Result;

 
            return result;
        }

        private List<Row> selectRegions(int datasetId, Tuple<int, int> from, Tuple<int, int> to, int zoomLevel)
        {

            CassandraQueryBuilder queryBuilder = new CassandraQueryBuilder()
            {
                QueryType = CassandraQueryBuilder.QueryTypes.Select
            };

            queryBuilder.TableName = "points_region_zoom_" + zoomLevel;
            queryBuilder.SelectColumnNames = new List<string>() { "row",
                                                                  "column",
                                                                  "points" };
            queryBuilder.ClausesList.Add(new BuilderTuple("datasetId", "dataset_id", CassandraQueryBuilder.Clauses.Equals));
            queryBuilder.ClausesList.Add(new BuilderTuple("fromRow", "row", CassandraQueryBuilder.Clauses.GreaterOrEqual));
            queryBuilder.ClausesList.Add(new BuilderTuple("fromColumn", "column", CassandraQueryBuilder.Clauses.GreaterOrEqual));
            queryBuilder.ClausesList.Add(new BuilderTuple("toRow", "row", CassandraQueryBuilder.Clauses.LessOrEqual));
            queryBuilder.ClausesList.Add(new BuilderTuple("toColumn", "column", CassandraQueryBuilder.Clauses.LessOrEqual));

            executionInstance.PrepareQuery(queryBuilder);

            return executionInstance.ExecuteQuery(new
            {
                datasetId,
                fromRow = from.Item1,
                fromColumn = from.Item2,
                toRow = to.Item1,
                toColumn = to.Item2
            }).Result;

        }


        private IEnumerable<PointsRegion> convertRowSetToPointsRegions(List<Row> rowSet)
        {
            ConcurrentDictionary<string, PointsRegion> result = new ConcurrentDictionary<string, PointsRegion>();

            Parallel.ForEach(rowSet, row =>
            {
                if (result.ContainsKey($"{row["row"]}_{row["column"]}"))
                {
                    var points = convertBasePoints(row["points"] as BasePointType[]);

                    foreach (var point in points)
                        (result[$"{row["row"]}_{row["column"]}"].Points as ConcurrentBag<PointBase>).Add(
                               point
                        );
                }
                else
                    result.TryAdd($"{row["row"]}_{row["column"]}", new PointsRegion()
                    {
                        Column = Convert.ToInt32(row["column"]),
                        Row = Convert.ToInt32(row["row"]),
                        Points = convertBasePoints(row["points"] as BasePointType[])
                    });
            });

            return result.Values;
        }


        private IEnumerable<PointBase> convertRowSetToBasicPointList(List<Row> rowSet)
        {

            ConcurrentBag<PointBase> result = new ConcurrentBag<PointBase>();
            Parallel.ForEach(rowSet, row =>
            {
                result.Add(new PointBase()
                {
                    Latitude = Convert.ToDecimal(row["latitude"]),
                    Longitude = Convert.ToDecimal(row["longitude"]),
                    Number = Convert.ToInt32(row["number"]),
                    DeformationRate = Convert.ToDecimal(row["deformation_rate"]),
                    EstimatedDeformationRate = Convert.ToDecimal(row["estimated_deformation_rate"]),
                    EstimatedHeight = Convert.ToDecimal(row["estimated_height"]),
                    StandardDeviation = Convert.ToDecimal(row["standard_deviation"]),
                    Height = Convert.ToDecimal(row["height"])
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


        private IEnumerable<PointBase> convertBasePoints(BasePointType[] basePoints)
        {
            ConcurrentBag<PointBase> result = new ConcurrentBag<PointBase>();

            foreach (var basePoint in basePoints)
                result.Add(new PointBase()
                {
                    DeformationRate = basePoint.deformation_rate,
                    EstimatedDeformationRate = basePoint.estimated_deformation_rate,
                    Height = basePoint.height,
                    EstimatedHeight = basePoint.estimated_height,
                    Latitude = basePoint.latitude,
                    Longitude = basePoint.longitude,
                    Number = basePoint.number,
                    Observations = basePoint.observations,
                    StandardDeviation = basePoint.standard_deviation
                });

            return result;
        }


        #endregion
    }
}
