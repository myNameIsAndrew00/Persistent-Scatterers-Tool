using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MapWebSite.GeoserverAPI.Entities
{
    [XmlRoot("style")]
    public class LayerStyle
    {
        [XmlElement("name")]
        public string Name { get; set; }
    }
}
