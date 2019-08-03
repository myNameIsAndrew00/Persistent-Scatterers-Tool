using Cassandra;
using System;
using System.Threading.Tasks;

namespace MapWebSite.CassandraAccess
{
    public class CassandraExecutionInstance : IDisposable
    {
        private ISession currentSession = null;

        private Cluster cluster = null;

        private string query = null;
         
        public UdtMappingDefinitions UserDefinedTypeMappings => currentSession?.UserDefinedTypes;
          
        public CassandraExecutionInstance(string server, string keyspace)
        {
            cluster = Cluster.Builder().AddContactPoint(server).Build();

            currentSession = cluster.Connect(keyspace); 
  
        }

        public void Dispose()
        {
            currentSession.Dispose();
            cluster.Dispose();
        }

        public async Task ExecuteNonQuery(dynamic anonymousType)
        {
            if (this.query == null) throw new ArgumentNullException("Query is not set. Use the Prepare Query method to set the query first");

            var statement = await currentSession.PrepareAsync(this.query);

            var boundStatement = statement.Bind(anonymousType);

            await currentSession.ExecuteAsync(boundStatement);
        }

        public void PrepareQuery(CassandraQueryBuilder builder)
        {
            this.query = builder.Build();
        }


        

    }
}
