using Cassandra;
using MapWebSite.CassandraAccess;
using MapWebSite.Core;
using MapWebSite.Core.Database;
using MapWebSite.Model;
using MapWebSite.Repository;
using MapWebSite.Repository.Entities;
using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MapWebSite.Repository
{
    public class CassandraDataPointsRepository : CassandraBaseRepository, IDataPointsRepository
    {
        public async Task<bool> InsertPointsDataset(PointsDataSet pointsDataset)
        {
            IEnumerable<PointType> pointTypes = PointType.GetPoints(pointsDataset);

            CassandraQueryBuilder queryBuilder = new CassandraQueryBuilder();
            queryBuilder.TableName = "points_by_dataset";
            queryBuilder.Type = typeof(PointType);
            queryBuilder.QueryType = CassandraQueryBuilder.QueryTypes.InsertFromType;

            CassandraExecutionInstance executionInstance = new CassandraExecutionInstance(this.server,
                                                                                this.keyspace);
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
            catch(Exception exception)
            {
                //TODO: log exception
            }
           
            return true;
        }
    }
}
