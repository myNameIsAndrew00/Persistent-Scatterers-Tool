using MapWebSite.Core;
using MapWebSite.Core.Database;
using MapWebSite.Model;
using MapWebSite.SQLAccess;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace MapWebSite.Repository
{
    public class SQLUserRepository : SQLBaseRepository, IUserRepository
    {
        public bool CheckUser(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) return false;

            byte[] storedHash = new byte[32];
            byte[] storedSalt = new byte[32];
            using (var userCredentialsInfo = SqlExecutionInstance.ExecuteQuery(new SqlCommand("GetUserPasswordInfo") { CommandType = System.Data.CommandType.StoredProcedure },
                                                new SqlParameter[]
                                                {
                                                    new SqlParameter("username",username)
                                                },
                                                new SqlConnection(this.connectionString)))
            {
                if (userCredentialsInfo.Tables[0].Rows.Count == 0) return false;

                storedHash = (byte[])userCredentialsInfo.Tables[0].Rows[0]["hashed_password"];
                storedSalt = (byte[])userCredentialsInfo.Tables[0].Rows[0]["password_salt"];
            };

            var userTrialPassword = Helper.HashData(Encoding.UTF8.GetBytes(password), storedSalt);

            return storedHash.SequenceEqual(userTrialPassword);
        }

        public int CreateUserPointsDataset(string username, string datasetName)
        {
            try
            {
                return Convert.ToInt32(SqlExecutionInstance.ExecuteScalar(new SqlCommand("InsertPointsDataset")
                                                    {
                                                        CommandType = System.Data.CommandType.StoredProcedure
                                                    },
                                                    new SqlParameter[]{
                                                         new SqlParameter("username", username),
                                                         new SqlParameter("dataset_name", datasetName) },
                                                    new SqlConnection(this.connectionString)));
                                              
            }
            catch (Exception exception)
            {
                //TODO: log exception
                return -1;
            } 
        }

        public bool InsertUser(User user)
        {
            try
            {
                SqlExecutionInstance.ExecuteNonQuery(new SqlCommand("InsertUser")
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                },
                                                    new SqlParameter[]{
                                                    new SqlParameter("hashed_password", user.PasswordHash),
                                                    new SqlParameter("password_salt", user.PasswordSalt),
                                                    new SqlParameter("username", user.Username),
                                                    new SqlParameter("first_name", user.FirstName),
                                                    new SqlParameter("last_name", user.LastName) },
                                                    new SqlConnection(this.connectionString));
            }
            catch (Exception exception)
            {
                //TODO: log exception
                return false;
            }
            return true;
        }
    }
}
