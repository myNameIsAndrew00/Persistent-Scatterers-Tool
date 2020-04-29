using MapWebSite.Core;
using MapWebSite.Core.Database;
using MapWebSite.Model;
using MapWebSite.SQLAccess;
using SqlKata;
using SqlKata.Compilers;
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
        private static class Tables {
            public static readonly string Users = "Users";
            public static readonly string Datasets = "DataSets";
            public static readonly string UsersAllowedDatasets = "UsersAllowedDatasets";
        }


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

            var userTrialPassword = Core.Helper.HashData(Encoding.UTF8.GetBytes(password), storedSalt);

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


        public int CreateUserPointsDataset(string username, string datasetName, PointsSource pointsSource)
        {
            try
            {
                return Convert.ToInt32(SqlExecutionInstance.ExecuteScalar(new SqlCommand("InsertPointsDataset")
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                },
                                                    new SqlParameter[]{
                                                         new SqlParameter("username", username),
                                                         new SqlParameter("dataset_name", datasetName),
                                                         new SqlParameter("source_name", pointsSource.ToString())
                                                    },
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
                return Convert.ToInt32(SqlExecutionInstance.ExecuteScalar(new SqlCommand("GetUserPointsDatasetID")
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

        public PointsDataSetHeader GetDatasetHeader(string username, string datasetName)
        {
            return getDatasetHeader(username, datasetName, 0);
        }


        public PointsDataSetHeader GetDatasetHeader(int datasetId)
        {
            return getDatasetHeader(string.Empty, string.Empty, datasetId);
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
                                                    new SqlParameter("last_name", user.LastName),
                                                    new SqlParameter("email", user.Email),
                                                    new SqlParameter("secure_stamp", user.SecurityStamp)},
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
                return parseColorMapDataset(colorMapsResult.Tables[0].Rows);
            }

        }


        public IEnumerable<Tuple<string, ColorMap>> GetColorMapsFiltered(IEnumerable<Tuple<ColorMapFilters, string>> filters, int pageIndex, int itemsPerPage)
        {

            Query query = new Query("dbo.ColorPalettes as CP")
                                .Select(new string[]{
                                       "U.username",
                                       "CP.palette_name",
                                       "CP.palette_serialization",
                                       "CP.status_mask"
                                })
                                .Join("dbo.Users as U", "CP.user_id", "U.user_id");

            Func<ColorMapFilters, string> getColumnName = (filter) =>
            {
                switch (filter)
                {
                    case ColorMapFilters.ColorMapName:
                        return "CP.palette_name";
                    case ColorMapFilters.Username:
                        return "U.username";
                    default:
                        return null;
                }
            };

            if (filters != null)
                foreach (var filter in filters)
                {
                    string columnName = getColumnName(filter.Item1);
                    if (!string.IsNullOrEmpty(columnName))
                        query = query.WhereLike(columnName, $"%{filter.Item2}%");
                }

            query = query.OrderByDesc("CP.creation_date")
                .Limit(itemsPerPage).Offset(pageIndex * itemsPerPage);

            SqlResult queryResult = new SqlServerCompiler().Compile(query);

            using (var colorMapsResult = SqlExecutionInstance.ExecuteQuery(new SqlCommand(queryResult.ToString())
            { CommandType = CommandType.Text }, null, new SqlConnection(this.connectionString)))
            {
                return parseColorMapDataset(colorMapsResult.Tables[0].Rows);
            }

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
            if (string.IsNullOrEmpty(username)) return null;

            return this.getUser(username, string.Empty);
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
                                                    new SqlParameter("secure_stamp", user.SecurityStamp),
                                                    new SqlParameter("email", user.Email) },
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

        public IEnumerable<PointsDataSetHeader> GetDataSetsFiltered(string username, DataSetsFilters filter, string filterValue, int pageIndex, int itemsPerPage)
        {
            using (var datasetsResult = SqlExecutionInstance.ExecuteQuery(new SqlCommand("GetDataSetsFiltered")
            { CommandType = CommandType.StoredProcedure },
                                               new SqlParameter[]
                                               {
                                                    new SqlParameter("@username", username),
                                                    new SqlParameter("@filter_id",(int)filter),
                                                    new SqlParameter("@filter_value",filterValue),
                                                    new SqlParameter("@page_index",pageIndex),
                                                    new SqlParameter("@items_per_page",itemsPerPage)
                                               },
                                               new SqlConnection(this.connectionString)))
            {
                return parseDataPointsDataset(datasetsResult.Tables[0].Rows);
            }

        }

        public IEnumerable<PointsDataSetHeader> GetDataSetsFiltered(string username, IEnumerable<Tuple<DataSetsFilters, string>> filters, int pageIndex, int itemsPerPage)
        {
            IList<UserRoles> roles = this.GetUserRoles(username);

            Query query = new Query("dbo.DataSets as DS")
                              .Select(new string[]{
                                        "U.username",
                                        "DS.dataset_name",
                                        "DS.data_set_id as dataset_id",
                                        "DS.status_id",
                                        "DS.source_name"
                              })
                              .Join("dbo.Users as U", "DS.user_id", "U.user_id")
                              .LeftJoin("dbo.UsersAllowedDatasets as UAD", "UAD.dataset_id", "DS.data_set_id");

            Func<DataSetsFilters, string> getColumnName = (filter) =>
            {
                switch (filter)
                {
                    case DataSetsFilters.DataSetName:
                        return "DS.dataset_name";
                    case DataSetsFilters.Username:
                        return "U.username";
                    case DataSetsFilters.Source:
                        return "DS.source_name";
                    default:
                        return null;
                }
            };

            if (filters != null)
                foreach (var filter in filters)
                {
                    string columnName = getColumnName(filter.Item1);
                    if (!string.IsNullOrEmpty(columnName))
                        query = query.WhereLike(columnName, $"%{filter.Item2}%");
                }

            query.WhereRaw($"(UAD.user_id = (select top 1 user_id from Users as _U where _U.username  = ?) { (roles.Contains(UserRoles.Administrator) ? "OR 1 = 1" : string.Empty) })", username);

            query = query.OrderByDesc("DS.data_set_id")
                .Limit(itemsPerPage).Offset(pageIndex * itemsPerPage);

            SqlResult queryResult = new SqlServerCompiler().Compile(query);

            using (var colorMapsResult = SqlExecutionInstance.ExecuteQuery(new SqlCommand(queryResult.ToString())
            { CommandType = CommandType.Text }, null, new SqlConnection(this.connectionString)))
            {
                return parseDataPointsDataset(colorMapsResult.Tables[0].Rows);
            }

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

        public bool UpdateDatasetLimits(string datasetName, string username, decimal? minimumLatitude, decimal? minimumLongitude, decimal? maximumLatitude, decimal? maximumLongitude)
        {
            try
            {
                SqlExecutionInstance.ExecuteNonQuery(new SqlCommand("UpdatePointsDatasetLimits")
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                },
                                                   new SqlParameter[]{
                                                        new SqlParameter("datasetName", datasetName),
                                                        new SqlParameter("username",username),
                                                        new SqlParameter("minimum_latitude", minimumLatitude),
                                                        new SqlParameter("minimum_longitude", minimumLongitude),
                                                        new SqlParameter("maximum_latitude", maximumLatitude),
                                                        new SqlParameter("maximum_longitude", maximumLongitude)
                                                   },
                                                   new SqlConnection(this.connectionString));
            }
            catch (Exception exception)
            {
                return false;
            }
            return true;
        }


        public bool UpdateDatasetRepresentationLimits(string datasetName,
                                 string username,
                                 decimal? minimumHeight,
                                 decimal? maximumHeight,
                                 decimal? minimumDeformationRate,
                                 decimal? maximumDeformationRate,
                                 decimal? minimumStdDev,
                                 decimal? maximumStdDev)
        {
            try
            {
                SqlExecutionInstance.ExecuteNonQuery(new SqlCommand("UpdatePointsDatasetLimits")
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                },
                                                   new SqlParameter[]{
                                                        new SqlParameter("datasetName", datasetName),
                                                        new SqlParameter("username",username),
                                                        new SqlParameter("minimum_height", minimumHeight),
                                                        new SqlParameter("minimum_def_rate", minimumDeformationRate),
                                                        new SqlParameter("minimum_std_dev", minimumStdDev),
                                                        new SqlParameter("maximum_height", maximumHeight),
                                                        new SqlParameter("maximum_def_rate", maximumDeformationRate),
                                                        new SqlParameter("maximum_std_dev", maximumStdDev)
                                                   },
                                                   new SqlConnection(this.connectionString));
            }
            catch (Exception exception)
            {
                return false;
            }
            return true;
        }

        public User GetUserByEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return null;

            return this.getUser(string.Empty, email);
        }

        public bool SetEmail(string username, string email)
        {
            try
            {
                SqlExecutionInstance.ExecuteNonQuery(new SqlCommand("SetUserEmail")
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                },
                                                 new SqlParameter[]{
                                                        new SqlParameter("email", email),
                                                        new SqlParameter("username",username)
                                                 },
                                                 new SqlConnection(this.connectionString));
            }
            catch (Exception exception)
            {
                //todo: log exception
                return false;
            }
            return true;
        }

        public bool SetEmailConfirmed(string username, bool confirmed)
        {
            try
            {
                try
                {
                    SqlExecutionInstance.ExecuteNonQuery(new SqlCommand("SetUserEmailConfirmed")
                    {
                        CommandType = System.Data.CommandType.StoredProcedure
                    },
                                                     new SqlParameter[]{
                                                        new SqlParameter("email_confirmed", confirmed),
                                                        new SqlParameter("username",username)
                                                     },
                                                     new SqlConnection(this.connectionString));
                }
                catch (Exception exception)
                {
                    //todo: log exception
                    return false;
                }
                return true;
            }
            catch (Exception exception)
            {
                //todo: log exception
                return false;
            }
            return true;
        }

        public int RaiseToGeoserverDataset(int datasetId, string apiUrl)
        {
            try
            {
                return Convert.ToInt32(SqlExecutionInstance.ExecuteScalar(new SqlCommand("InsertGeoserverPointsDataset")
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                },
                                                    new SqlParameter[]{
                                                         new SqlParameter("geoserver_api_url", apiUrl),
                                                         new SqlParameter("data_set_id", datasetId)
                                                    },
                                                    new SqlConnection(this.connectionString)));

            }
            catch (Exception exception)
            {
                //TODO: log exception
                return -1;
            }
        }

        public int InsertGeoserverColorMap(int geoserverDatasetId, int paletteId)
        {
            try
            {
                return Convert.ToInt32(SqlExecutionInstance.ExecuteScalar(new SqlCommand("InsertGeoserverColorPalette")
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                },
                                                    new SqlParameter[]{
                                                         new SqlParameter("geoserver_data_set_id", geoserverDatasetId),
                                                         new SqlParameter("palette_id", paletteId)
                                                    },
                                                    new SqlConnection(this.connectionString)));

            }
            catch (Exception exception)
            {
                //TODO: log exception
                return -1;
            }
        }

        public int InsertGeoserverColorMap(int geoserverDatasetId, string paletteName, string paletteUsername)
        {
            try
            {
                return Convert.ToInt32(SqlExecutionInstance.ExecuteScalar(new SqlCommand("InsertGeoserverColorPaletteByName")
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                },
                                                    new SqlParameter[]{
                                                         new SqlParameter("geoserver_data_set_id", geoserverDatasetId),
                                                         new SqlParameter("palette_name", paletteName),
                                                         new SqlParameter("username", paletteUsername)
                                                    },
                                                    new SqlConnection(this.connectionString)));

            }
            catch (Exception exception)
            {
                //TODO: log exception
                return -1;
            }
        }



        public IEnumerable<Tuple<string, ColorMap>> GetGeoserverColorMaps(int geoserverDatasetId)
        {
            using (var colorMapsResult = SqlExecutionInstance.ExecuteQuery(new SqlCommand("GetGeoserverColorPalettes")
            { CommandType = System.Data.CommandType.StoredProcedure },
                                             new SqlParameter[]
                                             {
                                                    new SqlParameter("@geoserver_dataset_id",geoserverDatasetId)
                                             },
                                             new SqlConnection(this.connectionString)))
            {
                return parseColorMapDataset(colorMapsResult.Tables[0].Rows);
            }
        }

        public int GetGeoserverDatasetID(int datasetId)
        {
            try
            {
                return Convert.ToInt32(SqlExecutionInstance.ExecuteScalar(new SqlCommand("GetUserGeoserverPointsDataasetID")
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                },
                                                    new SqlParameter[]{
                                                         new SqlParameter("dataset_id", datasetId)
                                                    },
                                                    new SqlConnection(this.connectionString)));

            }
            catch (Exception exception)
            {
                //TODO: log exception
                return -1;
            }
        }


        public bool AddPointsDatasetToUser(string datasetName, string datasetUser, string username)
        {
            try
            {
                SqlExecutionInstance.ExecuteNonQuery(new SqlCommand("InsertDatapointsToUser")
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                },
                                                 new SqlParameter[]{
                                                        new SqlParameter("dataset_name", datasetName),
                                                        new SqlParameter("dataset_user", datasetUser),
                                                        new SqlParameter("username", username),
                                                 },
                                                 new SqlConnection(this.connectionString));
            }
            catch (Exception exception)
            {
                //todo: log exception
                return false;
            }
            return true;
        }

        public bool RemovePointsDatasetFromUser(string datasetName, string datasetUser, string username)
        {
            try
            {
                SqlExecutionInstance.ExecuteNonQuery(new SqlCommand("RemoveDatapointsFromUser")
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                },
                                                 new SqlParameter[]{
                                                        new SqlParameter("dataset_name", datasetName),
                                                        new SqlParameter("dataset_user", datasetUser),
                                                        new SqlParameter("username", username),
                                                 },
                                                 new SqlConnection(this.connectionString));
            }
            catch (Exception exception)
            {
                //todo: log exception
                return false;
            }
            return true;
        }


        public IEnumerable<User> GetUsersFiltered(IEnumerable<Tuple<UserFilters, string>> filters, int pageIndex, int itemsPerPage)
        {
            Query query = new Query("dbo.Users as U")
                              .Select(new string[]{
                                       "U.username",
                                        "UD.first_name",
                                        "UD.last_name",
                                        "UD.timestamp",
                                        "UD.email",
                                        "UD.email_confirmed"
                              })
                              .Join("dbo.UsersDetails as UD", "UD.user_id", "U.user_id");

            Func<UserFilters, string> getColumnName = (filter) =>
            {
                switch (filter)
                {
                    case UserFilters.Email:
                        return "UD.email";
                    case UserFilters.FirstName:
                        return "UD.first_name";
                    case UserFilters.LastName:
                        return "UD.last_name";
                    case UserFilters.Username:
                        return "U.username";
                    default:
                        return null;
                }
            };

            if (filters != null)
                foreach (var filter in filters)
                {
                    string columnName = getColumnName(filter.Item1);
                    if (!string.IsNullOrEmpty(columnName))
                        query = query.WhereLike(columnName, $"%{filter.Item2}%");
                }

            query = query.OrderByDesc("UD.user_id")
                .Limit(itemsPerPage).Offset(pageIndex * itemsPerPage);

            SqlResult queryResult = new SqlServerCompiler().Compile(query);

            using (var usersResult = SqlExecutionInstance.ExecuteQuery(new SqlCommand(queryResult.ToString())
            { CommandType = CommandType.Text }, null, new SqlConnection(this.connectionString)))
            {
                return parseUserDataset(usersResult.Tables[0].Rows);
            }

        }

        public int GetUsersCount()
        {
            Query query = new Query("Users").AsCount("user_id");

            SqlResult queryResult = new SqlServerCompiler().Compile(query);

            return Convert.ToInt32(
                SqlExecutionInstance.ExecuteScalar(new SqlCommand(queryResult.ToString())
                {
                    CommandType = CommandType.Text
                }, 
                null, 
                new SqlConnection(this.connectionString)));
        }

        public int GetDatasetsCount()
        {
            Query query = new Query(Tables.Datasets).AsCount("data_set_id");

            SqlResult queryResult = new SqlServerCompiler().Compile(query);

            return Convert.ToInt32(
                SqlExecutionInstance.ExecuteScalar(new SqlCommand(queryResult.ToString())
                {
                    CommandType = CommandType.Text
                },
                null,
                new SqlConnection(this.connectionString)));
        }

        public int GetUserAssociatedDatasetsCount(string username)
        {
            IList<UserRoles> roles = this.GetUserRoles(username);

            Query query = new Query($"{Tables.Datasets} as D")
                                    .AsCount()
                                    .LeftJoin($"{Tables.UsersAllowedDatasets} as UAD", "D.data_set_id", "UAD.dataset_id")
                                    .WhereRaw($"(UAD.user_id = (select top 1 user_id from Users as _U where _U.username  = ?) { (roles.Contains(UserRoles.Administrator) ? "OR 1 = 1" : string.Empty) })", username);
            
            SqlResult queryResult = new SqlServerCompiler().Compile(query);

            return Convert.ToInt32(
                SqlExecutionInstance.ExecuteScalar(new SqlCommand(queryResult.ToString())
                {
                    CommandType = CommandType.Text
                },
                null,
                new SqlConnection(this.connectionString)));
        }

        #region Private

        private PointsDataSetHeader getDatasetHeader(string username, string datasetName, int datasetId)
        {
            using (var userCredentialsInfo = SqlExecutionInstance.ExecuteQuery(new SqlCommand("GetUserPointsDataset")
            { CommandType = CommandType.StoredProcedure },
                                                   new SqlParameter[]
                                                   {
                                                           new SqlParameter("username", username),
                                                           new SqlParameter("dataset_name", datasetName),
                                                           new SqlParameter("dataset_id", datasetId),
                                                   },
                                                   new SqlConnection(this.connectionString)))
            {
                return parseDataSetHeaderDataset(userCredentialsInfo.Tables[0].Rows);
            };
        }

        private User getUser(string username, string email)
        {
            using (var userCredentialsInfo = SqlExecutionInstance.ExecuteQuery(new SqlCommand("GetUser") { CommandType = CommandType.StoredProcedure },
                                              new SqlParameter[]
                                              {
                                                    new SqlParameter("username",username),
                                                    new SqlParameter("email",  email)
                                              },
                                              new SqlConnection(this.connectionString)))
            {
                if (userCredentialsInfo.Tables[0].Rows.Count == 0) return null;

                return this.parseUserDataset(userCredentialsInfo.Tables[0].Rows).FirstOrDefault();
            };
        }

        private IEnumerable<User> parseUserDataset(DataRowCollection rows)
        {
            List<User> result = new List<User>();

            foreach (DataRow row in rows)
            {
                result.Add(new User()
                {
                    Username = (string)row["username"],
                    FirstName = (string)row["first_name"],
                    LastName = (string)row["last_name"],
                    SecurityStamp = row["timestamp"] is DBNull ? null : (string)row["timestamp"],
                    Email = row["email"] is DBNull ? null : (string)row["email"],
                    ConfirmedEmail = row["email_confirmed"] is DBNull ? false : (bool)row["email_confirmed"],
                    PasswordHash = null
                });
            }

            return result;
        }

        private PointsDataSetHeader parseDataSetHeaderDataset(DataRowCollection rows)
        {
            if (rows.Count == 0) return null;
            var resultRow = rows[0];

            return new PointsDataSetHeader()
            {
                Username = (string)resultRow["username"],
                Name = (string)resultRow["dataset_name"],
                ID = (int)resultRow["data_set_id"],
                MaximumLatitude = resultRow["maximum_latitude"] is DBNull ? null : (decimal?)resultRow["maximum_latitude"],
                MaximumLongitude = resultRow["maximum_longitude"] is DBNull ? null : (decimal?)resultRow["maximum_longitude"],
                MinimumLatitude = resultRow["minimum_latitude"] is DBNull ? null : (decimal?)resultRow["minimum_latitude"],
                MinimumLongitude = resultRow["minimum_longitude"] is DBNull ? null : (decimal?)resultRow["minimum_longitude"],
                MinimumHeight = resultRow["minimum_height"] is DBNull ? null : (decimal?)resultRow["minimum_height"],
                MaximumHeight = resultRow["maximum_height"] is DBNull ? null : (decimal?)resultRow["maximum_height"],
                MinimumDeformationRate = resultRow["minimum_def_rate"] is DBNull ? null : (decimal?)resultRow["minimum_def_rate"],
                MaximumDeformationRate = resultRow["maximum_def_rate"] is DBNull ? null : (decimal?)resultRow["maximum_def_rate"],
                MinimumStdDev = resultRow["minimum_std_dev"] is DBNull ? null : (decimal?)resultRow["minimum_std_dev"],
                MaximumStdDev = resultRow["maximum_std_dev"] is DBNull ? null : (decimal?)resultRow["maximum_std_dev"],

                Status = (DatasetStatus)((int)resultRow["data_set_id"]),
                PointsSource = (PointsSource)Enum.Parse(typeof(PointsSource), (string)resultRow["source_name"])
            };

        }
        private IEnumerable<PointsDataSetHeader> parseDataPointsDataset(DataRowCollection rows)
        {
            List<PointsDataSetHeader> result = new List<PointsDataSetHeader>();

            foreach (DataRow row in rows)
            {
                result.Add(new PointsDataSetHeader()
                {
                    Username = (string)row["username"],
                    Name = (string)row["dataset_name"],
                    ID = (int)row["dataset_id"],
                    Status = row["status_id"] == DBNull.Value ? DatasetStatus.None : (DatasetStatus)((int)row["status_id"]),
                    PointsSource = (PointsSource)Enum.Parse(typeof(PointsSource), (string)row["source_name"])
                });

            }

            return result;
        }

        private IEnumerable<Tuple<string, ColorMap>> parseColorMapDataset(DataRowCollection rows)
        {
            List<Tuple<string, ColorMap>> result = new List<Tuple<string, ColorMap>>();

            foreach (DataRow row in rows)
                result.Add(new Tuple<string, ColorMap>(
                    (string)row["username"],
                    new ColorMap()
                    {
                        Name = (string)row["palette_name"],
                        Intervals = new List<Interval>().JSONDeserialize((string)row["palette_serialization"]),
                        StatusMask = (int)row["status_mask"]
                    }));

            return result;
        }





        #endregion
    }
}
