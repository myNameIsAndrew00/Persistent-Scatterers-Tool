using Cassandra;
using System;
using System.Collections.Concurrent;
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


        //use this object to handle the access to the connection
        private object connectingLock = new object();

        //a boolean value which handle if a connection attempt is in progress
        private bool connecting = false;

        //a variables which stores prepared statements to optimise queries execution
        public ConcurrentDictionary<string, PreparedStatement> CachedStatements { get; private set; } = new ConcurrentDictionary<string, PreparedStatement>();

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
         
         
        public async Task ExecuteNonQuery(dynamic parameters, PreparedStatement statement = null)
        {
            if (string.IsNullOrEmpty(this.query)) throw new ArgumentNullException("Query is not set. Use the Prepare Query method to set the query first");
            if (this.currentSession == null) createConnection();

            try
            {
                if(statement == null) statement = await currentSession.PrepareAsync(this.query);
                var boundStatement = statement.Bind(parameters);
            
                await currentSession.ExecuteAsync(boundStatement);
            }
            catch(Exception exception)
            {
                //todo: log exception
            }
        }

        public List<Row> ExecuteQuery(dynamic parameters, PreparedStatement statement = null)
        {
            if (string.IsNullOrEmpty(this.query)) throw new ArgumentNullException("Query is not set. Use the Prepare Query method to set the query first");
            if (this.currentSession == null) createConnection();

            try
            {
                if(statement == null) statement = currentSession.Prepare(this.query);

                var boundStatement = statement.Bind(parameters);

                RowSet result = currentSession.Execute(boundStatement);

                return result.GetRows().ToList();
            }
            catch (Exception exception)
            {
                //todo: log exception
                return new List<Row>();
            }
        }


        public void PrepareQuery(CassandraQueryBuilder builder)
        {
            this.query = builder.Build();
        }

        /// <summary>
        /// Creates and caches a prepared statement. It can be used to optimise queries execution time
        /// </summary>
        /// <param name="builder">The builder which builds the query</param>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task<PreparedStatement> GetPreparedStatement(string key, CassandraQueryBuilder builder = null)
        {
            if (this.CachedStatements.ContainsKey(key)) return this.CachedStatements[key];

            if (builder == null) return null;

            PreparedStatement statement = await this.currentSession.PrepareAsync(builder.Build());
            this.CachedStatements.TryAdd(key, statement);

            return statement;
        }


        private void createConnection()
        {
            int attempts = 0;

            lock (connectingLock)
            {
                if (connecting) return;
                connecting = true;
            }

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
                    if (attempts >= maxAttempts) break;
                }                
            }
            
            lock (connectingLock) connecting = false;
        }

    }
}
