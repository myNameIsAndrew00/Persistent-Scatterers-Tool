using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace MapWebSite.Types
{
    ///<summary>
    ///Decorate with this attribute every class which is a user defined type in database <br></br>
    ///This decoration can be used to generate queries, code or database objects
    ///</summary>
    public class UserDefinedTypeAttribute : Attribute
    {
        public UserDefinedTypeAttribute()
        {
        }
    }

    ///<summary>
    ///Decorate with this attribute every member which is a user defined type column in user defined type classes <br></br>
    ///This decoration can be used to generate queries, code or database objects
    ///</summary>
    public class UserDefinedTypeColumnAttribute : Attribute
    {
        public string NameInDatabase { get; } = null;

        public bool UseIfNull { get; } = true;

        public UserDefinedTypeColumnAttribute(string nameInDatabase, bool useIfNull = true)
        {
            this.NameInDatabase = nameInDatabase;
            this.UseIfNull = useIfNull;
        }
    }


    public static class UserDefinedTypeAttributeExtensions
    {

        public static string GetUserDefinedNameInDatabase<T>(this T @enum) where T : Enum
        {
            var info = typeof(T).GetMember(@enum.ToString());

            var attributeValue = (UserDefinedTypeColumnAttribute)info[0].GetCustomAttribute(typeof(UserDefinedTypeColumnAttribute), false);
 
            if (attributeValue == null)
                throw new ArgumentException("Argument is not a valid object. It must be decorated with UserDefinedTypeColumn Attribute");

            return attributeValue.NameInDatabase;
        }

        /// <summary>
        /// If user defind column names are used as attributes inside an enum, use this method to retrieve all the names 
        /// </summary>
        /// <param name="enum"></param>
        /// <returns></returns>
        public static string[] GetUserDefinedColumnsNames(Type @enum) 
        {
            List<string> result = new List<string>();
            foreach(var userDefinedType in Enum.GetValues(@enum))
            {              
                var info = @enum.GetMember(userDefinedType.ToString());

                var attributeValue = (UserDefinedTypeColumnAttribute) info[0].GetCustomAttribute(typeof(UserDefinedTypeColumnAttribute), false);

                if (attributeValue == null)
                    throw new ArgumentException("Argument is not a valid object. It must be decorated with UserDefinedTypeColumn Attribute");

                result.Add(attributeValue.NameInDatabase);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Use this extension method to map an object with a DataTable. <br></br>
        /// DataTable columns will be the properties of the object which are decorated with UserDefinedTypeColumn attribute
        /// </summary>
        /// <param name="Object"></param>
        /// <returns></returns>
        public static DataTable GetDataTableFromProperties(this object Object)
        {
            if (Object.GetType().GetCustomAttributes(typeof(UserDefinedTypeAttribute), true) == null)
                throw new ArgumentException("Argument is not a valid object. It must be decorated with UserDefinedType Attribute");

            DataTable dt = new DataTable();
            var properties = Object.GetType().GetProperties();


            foreach (var property in properties)
            {
                UserDefinedTypeColumnAttribute attribute =
                     property.GetCustomAttribute(typeof(UserDefinedTypeColumnAttribute)) as UserDefinedTypeColumnAttribute;

                if (attribute != null) dt.Columns.Add(property.Name, property.PropertyType);
            }


            return dt;
        }
    }
}
