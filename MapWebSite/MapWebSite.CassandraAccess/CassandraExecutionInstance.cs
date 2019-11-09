using Cassandra;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MapWebSite.CassandraAccess
{
    public class CassandraExecutionInstance : IDisposable
    {
        private readonly int maxAttempts = 10;         

        private string keyspace = null;

        private string server = null;

        private ISession currentSession = null;

        private Cluster cluster = null;

        private string query = null;
         
        public UdtMappingDefinitions UserDefinedTypeMappings => currentSession?.UserDefinedTypes;
          
        public CassandraExecutionInstance(string server, string keyspace)
        {
            this.cluster = Cluster.Builder().AddContactPoint(server).Build();
            this.server = server;
            this.keyspace = keyspace;

            try
            {
                currentSession = cluster.Connect(this.keyspace);
            }
            catch { currentSession = null; }
        }

        public void Dispose()
        {
            currentSession.Dispose();
            cluster.Dispose();

            GC.SuppressFinalize(this);
        }

        public async Task ExecuteNonQuery(dynamic parameters)
        {
            if (string.IsNullOrEmpty(this.query)) throw new ArgumentNullException("Query is not set. Use the Prepare Query method to set the query first");
            if (this.currentSession == null) createConnection();

            try
            {
                var statement = await currentSession.PrepareAsync(this.query);
                var boundStatement = statement.Bind(parameters);
            
                await currentSession.ExecuteAsync(boundStatement);
            }
            catch
            {

            }
        }

        public List<Row> ExecuteQuery(dynamic parameters)
        {
            if (string.IsNullOrEmpty(this.query)) throw new ArgumentNullException("Query is not set. Use the Prepare Query method to set the query first");

            var statement = currentSession.Prepare(this.query);

            var boundStatement = statement.Bind(parameters);

            RowSet result = currentSession.Execute(boundStatement);

            return result.GetRows().ToList();
        
        }


        public void PrepareQuery(CassandraQueryBuilder builder)
        {
            this.query = builder.Build();
        }


        private void createConnection()
        {
            int attempts = 0;
            while (this.currentSession == null)
            {
                try
                {
                    this.currentSession = cluster.Connect(this.keyspace);
                }
                catch
                {
                    attempts++;
                    currentSession = null;
                    if (attempts >= maxAttempts) throw;
                }
            }
        }

    }
}
