using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Core
{
    public class EnumStringAttribute : Attribute
    {
        public string String { get; set; }
        public EnumStringAttribute(string stringValue)
        {
            this.String = stringValue;
        }
    }
}
