using MapWebSite.Types;
using Newtonsoft.Json;
using ServiceStack.Serialization;
using System; 
using System.Data;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;

namespace MapWebSite.Core
{
    public static class ExtensionMethods
    {
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

                if (attribute != null)  dt.Columns.Add(property.Name, property.PropertyType);
            }
         

            return dt;
        }

        public static string JSONSerialize<T>(this T ObjectToBeSerialized, bool UseQuotesForColumns = true)
        {
            if (UseQuotesForColumns)  return JsonConvert.SerializeObject(ObjectToBeSerialized);

            JsonSerializer serializer = new JsonSerializer();
            using (var stringWriter = new StringWriter())
            {
                using (var writer = new JsonTextWriter(stringWriter))
                {
                    writer.QuoteName = false;
                    writer.QuoteChar = '\'';
                    serializer.Serialize(writer, ObjectToBeSerialized);
                }

                return stringWriter.ToString();
            }
        }
   
        public static T JSONDeserialize<T>(this T reference, string JSONstring)
        {
            JsonSerializer serializer = new JsonSerializer();

            using (var stringReader = new StringReader(JSONstring))
            using (var reader = new JsonTextReader(stringReader))
            {
                return serializer.Deserialize<T>(reader);
            }
        }


        public static string DataContractJSONSerialize<T>(this T ObjectToBeSerialized)
        {
            JsonDataContractSerializer serializer = new JsonDataContractSerializer();
            return serializer.SerializeToString<T>(ObjectToBeSerialized);
        }

        public static string GetEnumString(this Enum enumValue)
        {
            var type = enumValue.GetType();
            var info = type.GetMember(enumValue.ToString());

            var enumStringAttribute = info[0].GetCustomAttribute(typeof(EnumStringAttribute), false);

            return (enumStringAttribute as EnumStringAttribute)?.String;
        }


    }
}
