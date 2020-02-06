using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Core
{
    /// <summary>
    /// Decorate with this attribute an enum to provide a string value
    /// </summary>
    public class EnumStringAttribute : Attribute
    {
        public string String { get; set; }
        public EnumStringAttribute(string stringValue)
        {
            this.String = stringValue;
        }
    }
}
