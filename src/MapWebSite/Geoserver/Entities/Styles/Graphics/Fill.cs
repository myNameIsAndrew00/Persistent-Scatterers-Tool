using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MapWebSite.GeoserverAPI.Entities.Graphics
{
    /// <summary>
    /// Use this object to specify the color which will be used to fill a graphic object
    /// </summary>
    public class Fill
    {
        public class GraphicFill
        {
            public Graphic Graphic { get; set; }
        }

        [XmlElement("GraphicFill")]
        public GraphicFill GraphicFillObject { get; set; }

        [XmlElement("CssParameter")]
        public List<CssParameter> CssParameterArray { get; set; }
    }
}
