using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MapWebSite.GeoserverAPI.Entities.Graphics
{
    
    public class Stroke
    {
        [XmlElement]
        public Fill.GraphicFill GraphicFill { get; set; }

        [XmlElement]
        public Graphic GraphicStroke { get; set; }

        [XmlElement("CssParameter")]
        public List<CssParameter> CssParameterArray { get; set; }
    }

}
