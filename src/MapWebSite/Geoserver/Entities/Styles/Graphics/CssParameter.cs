using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MapWebSite.GeoserverAPI.Entities.Graphics
{
    /// <summary>
    /// Represents a visual style rule which can be applied to graphic objects
    /// </summary>
    public class CssParameter
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlText()]
        public string Value { get; set; }
    }
}
