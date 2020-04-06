using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Types
{
    public class EnumStringAttribute : Attribute
    {
        public string String { get; set; }
        public EnumStringAttribute(string stringValue)
        {
            this.String = stringValue;
        }
    }

    public static class EnumStringAttributeExtensions
    {


        /// <summary>
        /// Get the string which decorate an enum. Decoration its mate with Types.EnumStringAttribute.
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>        
        public static string GetEnumString(this Enum enumValue)
        {
            var type = enumValue.GetType();
            var info = type.GetMember(enumValue.ToString());

            var enumStringAttribute = info[0].GetCustomAttribute(typeof(Types.EnumStringAttribute), false);

            return (enumStringAttribute as Types.EnumStringAttribute)?.String;
        }

    }
}
