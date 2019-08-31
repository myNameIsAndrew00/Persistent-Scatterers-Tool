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

        public IEnumerable<string> GetColorMaps(string username)
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
    }
}
