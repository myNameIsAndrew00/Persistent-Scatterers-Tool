using MapWebSite.Core;
using MapWebSite.Core.Database;
using MapWebSite.Model;
using MapWebSite.SQLAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;

namespace MapWebSite.Repository
{
    public class SQLUserRepository : SQLBaseRepository, IUserRepository
    {
        public bool CheckUser(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) return false;

            byte[] storedHashData = new byte[64];

            using (var userCredentialsInfo = SqlExecutionInstance.ExecuteQuery(new SqlCommand("GetUserPasswordInfo") { CommandType = System.Data.CommandType.StoredProcedure },
                                                new SqlParameter[]
                                                {
                                                    new SqlParameter("username",username)
                                                },
                                                new SqlConnection(this.connectionString)))
            {
                if (userCredentialsInfo.Tables[0].Rows.Count == 0) return false;
                storedHashData = (byte[])userCredentialsInfo.Tables[0].Rows[0]["hashed_password"];
            };

            byte[] storedSalt = new byte[32];
            byte[] storedHash = new byte[32];

            Array.Copy(storedHashData, storedHash, 32);
            Array.Copy(storedHashData, 32, storedSalt, 0, 32);

            var userTrialPassword = Helper.HashData(Encoding.UTF8.GetBytes(password), storedSalt);

            return storedHash.SequenceEqual(userTrialPassword);
        }

        public bool CreateColorMap(string username, ColorMap colorMap)
        { 
            try
            {
                SqlExecutionInstance.ExecuteNonQuery(new SqlCommand("InsertColorPalette")
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                },
                                                    new SqlParameter[]{
                                                    new SqlParameter("username", username),
                                                    new SqlParameter("palette_name", colorMap.Name),
                                                    new SqlParameter("palette_serialization", colorMap.Intervals.JSONSerialize())
                                                    },                                                  
                                                    new SqlConnection(this.connectionString));
            }
            catch (Exception exception)
            {
                //TODO: log exception
                return false;
            }
            return true;
        }

        public IEnumerable<string> GetColorMapsNames(string username)
        {
            List<string> result = new List<string>();

            using (var colorMapsResult = SqlExecutionInstance.ExecuteQuery(new SqlCommand("GetUserColorPalettes") { CommandType = System.Data.CommandType.StoredProcedure },
                                               new SqlParameter[]
                                               {
                                                    new SqlParameter("username",username)
                                               },
                                               new SqlConnection(this.connectionString)))
            {
                foreach (DataRow row in colorMapsResult.Tables[0].Rows)
                    result.Add((string)row["palette_name"]);
            }

            return result;
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

    
        public int GetDatasetID(string username, string datasetName)
        {
            try
            {
                return Convert.ToInt32(SqlExecutionInstance.ExecuteScalar(new SqlCommand("GetUserPointsDataset")
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
                return (int)SqlExecutionInstance.ExecuteScalar(new SqlCommand("InsertUser")
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                },
                                                    new SqlParameter[]{
                                                    new SqlParameter("hashed_password", user.PasswordHash),                                                    
                                                    new SqlParameter("username", user.Username),
                                                    new SqlParameter("first_name", user.FirstName),
                                                    new SqlParameter("last_name", user.LastName) },
                                                    new SqlConnection(this.connectionString))
                    == 1;
            }
            catch (Exception exception)
            {
                //TODO: log exception
                return false;
            }

        }

        public IEnumerable<Tuple<string, ColorMap>> GetColorMapsFiltered(ColorMapFilters filter, string filterValue, int pageIndex, int itemsPerPage)
        {
            List<Tuple<string, ColorMap>> result = new List<Tuple<string, ColorMap>>();

            using (var colorMapsResult = SqlExecutionInstance.ExecuteQuery(new SqlCommand("GetColorPalettesFiltered")
                                                                                { CommandType = System.Data.CommandType.StoredProcedure },
                                               new SqlParameter[]
                                               {
                                                    new SqlParameter("@filter_id",(int)filter),
                                                    new SqlParameter("@filter_value",filterValue),
                                                    new SqlParameter("@page_index",pageIndex),
                                                    new SqlParameter("@items_per_page",itemsPerPage)
                                               },
                                               new SqlConnection(this.connectionString)))
            {
                foreach (DataRow row in colorMapsResult.Tables[0].Rows)
                    result.Add(new Tuple<string, ColorMap>(
                        (string)row["username"],
                        new ColorMap()
                        {
                            Name = (string)row["palette_name"],
                            Intervals = new List<Interval>().JSONDeserialize((string)row["palette_serialization"])
                        }));                    
            }

            return result;
        }

        public string GetColorMapSerialization(string username, string paletteName)
        {
            try
            {
                return Convert.ToString(SqlExecutionInstance.ExecuteScalar(new SqlCommand("GetUserColorPalette")
                {
                    CommandType = CommandType.StoredProcedure
                },
                                                    new SqlParameter[]{
                                                         new SqlParameter("username", username),
                                                         new SqlParameter("palette_name", paletteName) },
                                                    new SqlConnection(this.connectionString)));

            }
            catch (Exception exception)
            {
                //TODO: log exception
                return string.Empty;
            }
        }

        public byte[] GetUserHashedPassword(string username)
        {
            if (string.IsNullOrEmpty(username)) return null;

            byte[] passwordHash = (byte[])SqlExecutionInstance.ExecuteScalar(new SqlCommand("GetUserPasswordInfo") { CommandType = CommandType.StoredProcedure },
                                              new SqlParameter[]
                                              {
                                                    new SqlParameter("username",username)
                                              },
                                              new SqlConnection(this.connectionString));

            return passwordHash;
        }

        public User GetUser(string username)
        {
            using (var userCredentialsInfo = SqlExecutionInstance.ExecuteQuery(new SqlCommand("GetUser") { CommandType = CommandType.StoredProcedure },
                                              new SqlParameter[]
                                              {
                                                    new SqlParameter("username",username)
                                              },
                                              new SqlConnection(this.connectionString)))
            {
                if (userCredentialsInfo.Tables[0].Rows.Count == 0) return null;
                var resultRow = userCredentialsInfo.Tables[0].Rows[0];

                return new User()
                {
                    Username = (string)resultRow["username"],
                    FirstName = (string)resultRow["first_name"],
                    LastName = (string)resultRow["last_name"],
                    SecurityStamp = resultRow["timestamp"] is DBNull ? null : (string)resultRow["timestamp"],
                    PasswordHash = null
                };

            };
        }

        public bool UpdateUser(User user)
        {
            try
            {
                SqlExecutionInstance.ExecuteNonQuery(new SqlCommand("UpdateUser")
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                },
                                                    new SqlParameter[]{
                                                    new SqlParameter("username", user.Username),
                                                    new SqlParameter("first_name", user.FirstName),
                                                    new SqlParameter("last_name", user.LastName),
                                                    new SqlParameter("secure_stamp", user.SecurityStamp) },
                                                    new SqlConnection(this.connectionString));
            }
            catch (Exception exception)
            {
                //TODO: log exception
                return false;
            }
            return true;
        }

        public IList<UserRoles> GetUserRoles(string username)
        {
            List<UserRoles> result = new List<UserRoles>();

            using (var colorMapsResult = SqlExecutionInstance.ExecuteQuery(new SqlCommand("GetUserRoles")
            { CommandType = CommandType.StoredProcedure },
                                               new SqlParameter[]
                                               {
                                                    new SqlParameter("@username",username)
                                               },
                                               new SqlConnection(this.connectionString)))
            {
                foreach (DataRow row in colorMapsResult.Tables[0].Rows)
                    result.Add(
                        (UserRoles)Enum.Parse(typeof(UserRoles), (string)row["role_name"])
                        );
            }

            return result;

        }

        public IEnumerable<PointsDataSetBase> GetDataSetsFiltered(DataSetsFilters filter, string filterValue, int pageIndex, int itemsPerPage)
        {
            List<PointsDataSetBase> result = new List<PointsDataSetBase>();

            using (var colorMapsResult = SqlExecutionInstance.ExecuteQuery(new SqlCommand("GetDataSetsFiltered")
            { CommandType = System.Data.CommandType.StoredProcedure },
                                               new SqlParameter[]
                                               {
                                                    new SqlParameter("@filter_id",(int)filter),
                                                    new SqlParameter("@filter_value",filterValue),
                                                    new SqlParameter("@page_index",pageIndex),
                                                    new SqlParameter("@items_per_page",itemsPerPage)
                                               },
                                               new SqlConnection(this.connectionString)))
            {
                foreach (DataRow row in colorMapsResult.Tables[0].Rows)
                {
                    result.Add(new PointsDataSetBase()
                    {
                        Username = (string)row["username"],
                        DatasetName = (string)row["dataset_name"],
                        ID = (int)row["dataset_id"],
                        Status = row["status_id"] == DBNull.Value ? DatasetStatus.None : (DatasetStatus) ((int)row["status_id"])
                    }); ;
                     
                }
            }

            return result;
        }

        public bool UpdateDatasetStatus(string datasetName, DatasetStatus status, string username)
        {
            try
            {
                SqlExecutionInstance.ExecuteNonQuery(new SqlCommand("UpdatePointsDatasetStatus")
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                },
                                                    new SqlParameter[]{
                                                        new SqlParameter("datasetName", datasetName),
                                                        new SqlParameter("statusId",(int)status),
                                                        new SqlParameter("username",username)
                                                    },
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
