using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MapWebSite.GeoserverAPI.Entities.Symbolizers
{
    /// <summary>
    /// Use this class to specify the rules used for points layers over a rule.
    /// </summary>
    public class PointSymbolizer : Symbolizer
    {
        [XmlElement]
        public Geometry Geometry { get; set; }


        [XmlElement]
        public Graphic Graphic { get; set; }
    }
}
