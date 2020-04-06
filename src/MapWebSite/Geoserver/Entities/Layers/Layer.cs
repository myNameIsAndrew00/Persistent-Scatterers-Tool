using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MapWebSite.GeoserverAPI.Entities
{
    [XmlRoot("layer")]
    public class Layer
    {
        public enum LayerType
        {
            Vector,
            Tile
        }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("path")]
        public string Path { get; set; }

        [XmlIgnore]
        public LayerType LayerTypeValue { get; set; }

        [XmlElement("type")]
        public string Type => LayerTypeValue.ToString().ToUpper();

        [XmlElement("defaultStyle")]
        public LayerStyle DefaultStyle { get; set; }

        [XmlArray("styles")]
        [XmlArrayItem("style")]
        public List<LayerStyle> Styles { get; set; }
    }
}
