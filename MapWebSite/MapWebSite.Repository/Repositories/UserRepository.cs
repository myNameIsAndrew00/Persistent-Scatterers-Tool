 
using MapWebSite.Repository.Entities;
using MapWebSite.SQLAccess;
using System;
using System.Data.SqlClient;

namespace MapWebSite.Repository
{
    public class UserRepository : BaseRepository
    {
        public bool InsertUser(DBUser user)
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
