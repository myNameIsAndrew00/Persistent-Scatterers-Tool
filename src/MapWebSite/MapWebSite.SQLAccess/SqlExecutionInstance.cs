using System;
using System.Data;
using System.Data.SqlClient;


namespace MapWebSite.SQLAccess
{
    /// <summary>
    /// Use this class to handle sql server query executions
    /// </summary>
    public static class SqlExecutionInstance
    {

        public static void ExecuteNonQuery(IDbCommand Command, IDbDataParameter[] Parameters, IDbConnection Connection)
        {

            Command.Connection = Connection;

            if (Parameters != null)
                foreach (var parameter in Parameters)
                    Command.Parameters.Add(parameter);

            try
            {
                using (Connection)
                using (Command)
                {
                    Connection.Open();
                    Command.ExecuteNonQuery();
                }
            }
            catch (Exception Exception)
            {
                throw Exception;
            }
            finally
            {
                Connection.Close();
            }
        }

        public static DataSet ExecuteQuery(IDbCommand Command, IDbDataParameter[] Parameters, IDbConnection Connection,
            Func<IDbCommand, IDbDataAdapter> adapterCreator = null)
        {
            if (adapterCreator == null)
                adapterCreator = command => new SqlDataAdapter(command as SqlCommand);

            DataSet dataSet = new DataSet();

            Command.Connection = Connection;
            if (Parameters != null)
                foreach (var parameter in Parameters)
                    Command.Parameters.Add(parameter);

            try
            {
                using (Connection)
                using (Command)
                {
                    Connection.Open();
                    IDbDataAdapter dataAdapter = adapterCreator(Command);
                    dataAdapter.Fill(dataSet);

                    (dataAdapter as IDisposable)?.Dispose();
                }
            }
            catch (Exception Exception)
            {
                throw Exception;
            }
            finally
            {
                Connection.Close();
            }

            return dataSet;
        }

        public static object ExecuteScalar(IDbCommand Command, IDbDataParameter[] Parameters, IDbConnection Connection)
        {
            Command.Connection = Connection;
            if (Parameters != null)
                foreach (var parameter in Parameters)
                    Command.Parameters.Add(parameter);

            try
            {
                using (Connection)
                using (Command)
                {
                    Connection.Open();
                    return Command.ExecuteScalar();
                }
            }
            catch (Exception Exception)
            {
                throw Exception;
            }
            finally
            {
                Connection.Close();
            }
        }


    }

}