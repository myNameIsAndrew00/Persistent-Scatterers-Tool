using MapWebSite.Types;
using Newtonsoft.Json; 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace MapWebSite.Core
{
    /// <summary>
    /// Contains methods which extends base data types
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Clone a IEnumerable container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listToClone">The container which must be cloned</param>
        /// <returns>A clone of the parameter</returns>
        public static IEnumerable<T> Clone<T>(this IEnumerable<T> listToClone) where T : ICloneable
        {
            List<T> result = new List<T>();

            foreach (var item in listToClone)
                result.Add((T)item.Clone());
            
            return result;
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
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));

            using (var stream = new MemoryStream()) {
                serializer.WriteObject(stream, ObjectToBeSerialized);
                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream))
                    return reader.ReadToEnd();             
             }

        }


        public static byte[] Concatenate(this byte[] firstArray, byte[] secondArray)
        {
            byte[] result = new byte[firstArray.Length + secondArray.Length];

            Array.Copy(firstArray, 0, result, 0, firstArray.Length);
            Array.Copy(secondArray, 0, result, firstArray.Length, secondArray.Length);

            return result;
        }

        /// <summary>
        /// Extension method for exponentiation of a decimal number 
        /// </summary>
        /// <param name="number">The number which must be expoentiate</param>
        /// <param name="pow">The power used for exponentiation</param>
        /// <returns></returns>
        public static decimal Pow(this decimal number, uint pow)
        {
            decimal A = 1m;
            BitArray e = new BitArray(BitConverter.GetBytes(pow));
            int t = e.Count;

            for (int i = t - 1; i >= 0; --i)
            {
                A *= A;
                if (e[i] == true)
                {
                    A *= number;
                }
            }
            return A;
        }
    }
}
