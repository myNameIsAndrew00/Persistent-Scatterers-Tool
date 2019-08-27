using MapWebSite.Core;
using MapWebSite.Types;
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

        public enum Clauses
        {
            Equals = '=',
            Less = '<',
            LessOrEqual = '|',
            Greater = '>',
            GreaterOrEqual = ';'
        }


        public QueryTypes QueryType { get; set; } = CassandraQueryBuilder.QueryTypes.Insert;

        public string TableName { get; set; }

        public List<string> IgnoredColumnNames { get; set; } = null;

        public List<string> SelectColumnNames { get; set; } = null;

        public string[] Parameters { get; set; }

        public Type Type { get; set; }

        /// <summary>
        /// Item1: parameter name
        /// Item2: name in the table
        /// Item3: relationship
        /// </summary>
        public List<Tuple<string, string, Clauses>> ClausesList { get; set; } = new List<Tuple<string, string, Clauses>>();

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
                    return BuildSelectQuery();
            }

            return null;
        }
        
        /// <summary>
        /// Use this method to generate a select query string.<br></br>
        /// Properties used: TableName, ClausesList
        /// </summary>
        /// <returns></returns>
        public string BuildSelectQuery()
        {
            if (string.IsNullOrEmpty(TableName)) throw new ArgumentNullException("Table name is not set");


            return buildSelectQuery(this.TableName, this.ClausesList);

        }



        /// <summary>
        /// Use this method to generate a insert query string.<br></br>
        /// Properties used: TableName, Parameters, SelectColumnNames
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
        /// Properties used: TableName, Type, IgnoredColumnNames
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

                if (attribute != null)
                {
                    if (IgnoredColumnNames != null && IgnoredColumnNames.Exists(item => item == attribute.NameInDatabase))
                        continue;
                    columns.Add(attribute.NameInDatabase);
                }
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


        private string buildSelectQuery(string tableName, List<Tuple<string,string, Clauses>> clausesList)
        {
            StringBuilder queryBuilder = new StringBuilder();

            Action<Tuple<string, string, Clauses>> appendClause = delegate (Tuple<string, string, Clauses> clause)
            {
                queryBuilder.Append(clause.Item2);
                string symbol = Convert.ToString((char)clause.Item3);
                if (symbol == "|") symbol = "<=";
                if (symbol == ";") symbol = ">=";

                queryBuilder.AppendFormat(" {0} ",symbol);
                queryBuilder.AppendFormat(":{0} ", clause.Item1);
            };

            Action apendSelectColumns = delegate ()
            {
                queryBuilder.Append("select ");
                if (SelectColumnNames == null){
                    queryBuilder.Append('*');
                    return;
                }
                SelectColumnNames.ForEach(column =>
                    queryBuilder.AppendFormat(" {0},", column)); 
                queryBuilder.Remove(queryBuilder.Length - 1, 1);
            };

            apendSelectColumns();
            queryBuilder.AppendFormat(" from {0} ", tableName);

            if (clausesList.Count > 0) queryBuilder.Append("where ");

            for (int index = 0; index < clausesList.Count - 1; index++)
            {
                appendClause(clausesList[index]);
                queryBuilder.Append(" and ");
            }

            if (clausesList.Count > 0) appendClause(clausesList[clausesList.Count - 1]);

            queryBuilder.Append(" allow filtering");

            return queryBuilder.ToString();
        }



        #endregion
    }
}
