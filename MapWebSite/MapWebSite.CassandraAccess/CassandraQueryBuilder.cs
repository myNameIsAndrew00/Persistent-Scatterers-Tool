using MapWebSite.Core;
using System;
using System.Collections.Generic; 
using System.Reflection;
using System.Text; 

namespace MapWebSite.CassandraAccess
{
    public class CassandraQueryBuilder
    {
        public enum QueryTypes
        {
            Insert,
            InsertFromType,
            Select
        }

        public QueryTypes QueryType { get; set; } = CassandraQueryBuilder.QueryTypes.Insert;

        public string TableName { get; set; }

        public string[] Parameters { get; set; }

        public Type Type { get; set; }

        /// <summary>
        /// Use this method to call Build methods acording to QueryType property
        /// </summary>
        /// <returns></returns>
        public string Build()
        {
            switch (this.QueryType)
            {
                case QueryTypes.Insert:
                    return BuildInsertQuery();
                case QueryTypes.InsertFromType:
                    return BuildInsertQueryFromType();
                case QueryTypes.Select:
                    //TO DO: throw exception
                    throw new NotImplementedException();
            }

            return null;
        }

        /// <summary>
        /// Use this method to generate a query string.<br></br>
        /// Properties used: TableName, Parameters
        /// </summary>
        /// <returns></returns>
        public string BuildInsertQuery()
        {

            if (string.IsNullOrEmpty(TableName)) throw new ArgumentNullException("Table name is not set");

            if (Parameters?.Length < 1) throw new ArgumentNullException("Parameters are not set or insuficient number of parameters");

            return buildInsertQuery(this.TableName, this.Parameters);

        }

        /// <summary>
        /// Use this method to build an insert query based on a class decorated with UserDefinedType Attribute.<br></br>
        /// Properties used: TableName, Type
        /// </summary>
        /// <returns></returns>
        public string BuildInsertQueryFromType()
        {
            if (string.IsNullOrEmpty(TableName)) throw new ArgumentNullException("Table name is not set");

            if (this.Type == null) throw new ArgumentNullException("Type property was not set.");

            if (Attribute.GetCustomAttribute(Type, typeof(UserDefinedTypeAttribute)) == null)
                throw new InvalidOperationException("Type property is not decorated with UserDefinedType attribute");

            List<string> columns = new List<string>();

            PropertyInfo[] properties = Type.GetProperties();
            for (int index = 0; index < properties.Length; index++)
            {
                UserDefinedTypeColumnAttribute attribute =
                    properties[index].GetCustomAttribute(typeof(UserDefinedTypeColumnAttribute)) as UserDefinedTypeColumnAttribute;

                if (attribute != null) columns.Add(attribute.NameInDatabase);
            }

            return buildInsertQuery(this.TableName, columns.ToArray());
        }



        #region Private

        private string buildInsertQuery(string tableName, string[] parameters )
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.AppendFormat("insert into {0}({1}", tableName, parameters[0]);

            for (int i = 1; i < parameters.Length; i++)
                queryBuilder.AppendFormat(",{0}", parameters[i]);

            queryBuilder.AppendFormat(") values (:{0}", parameters[0]);

            for (int i = 1; i < parameters.Length; i++)
                queryBuilder.AppendFormat(",:{0}", parameters[ i ]);

            queryBuilder.Append(')');

            return queryBuilder.ToString();
        }

        #endregion
    }
}
