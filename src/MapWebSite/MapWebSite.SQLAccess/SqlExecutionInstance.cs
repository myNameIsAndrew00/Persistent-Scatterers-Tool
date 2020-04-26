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

        public static void ExecuteNonQuery(SqlCommand Command, SqlParameter[] Parameters, SqlConnection Connection)
        {

            Command.Connection = Connection;
            Command.Parameters.AddRange(Parameters);

            try
            {
                using (Connection)
                using (Command)
                {
                    Connection.Open();
                    Command.ExecuteNonQuery();
                }
            }
            catch (SqlException Exception)
            {
                throw Exception;
            }
            finally
            {
                Connection.Close();
            }
        }

        public static DataSet ExecuteQuery(SqlCommand Command, SqlParameter[] Parameters, SqlConnection Connection)
        {
            DataSet dataSet = new DataSet();

            Command.Connection = Connection;
            if (Parameters != null) Command.Parameters.AddRange(Parameters);

            try
            {
                using (Connection)
                using (Command)
                {
                    Connection.Open();
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(Command))
                        dataAdapter.Fill(dataSet);
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

        public static object ExecuteScalar(SqlCommand Command, SqlParameter[] Parameters, SqlConnection Connection)
        {
            Command.Connection = Connection;
            if (Parameters != null) Command.Parameters.AddRange(Parameters);

            try
            {
                using (Connection)
                using (Command)
                {
                    Connection.Open();
                    return Command.ExecuteScalar();
                }
            }
            catch (SqlException Exception)
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