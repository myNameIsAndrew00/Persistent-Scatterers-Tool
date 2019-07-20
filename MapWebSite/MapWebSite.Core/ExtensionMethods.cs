using System; 
using System.Data;
 
namespace MapWebSite.Core
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Use this extension method to map an object with a DataTable.
        /// DataTable columns will be the properties of the object
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
                dt.Columns.Add(property.Name, property.PropertyType);

            return dt;
        }
    }
}
