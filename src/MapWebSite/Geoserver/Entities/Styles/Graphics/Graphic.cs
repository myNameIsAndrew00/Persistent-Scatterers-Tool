using MapWebSite.Core;
using MapWebSite.GeoserverAPI.Entities.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MapWebSite.GeoserverAPI.Entities
{
    /// <summary>
    /// An object which describes the visual of a Rule.
    /// This graphic doesn't contains a mark. MarkGraphic can be used to have this behaviour.
    /// </summary>
    public class Graphic
    {
        public class Mark
        {
            [XmlIgnore]
            public Shape WellKnownNameProperty { get; set; }

            public string WellKnownName => WellKnownNameProperty.GetEnumString();

            [XmlElement]
            public Fill Fill { get; set; }

            [XmlElement]
            public Stroke Stroke { get; set; }
        }
        public class ExternalGraphic
        {
            public string OnlineResource { get; set; }

            public string Format { get; set; }
        }

        [XmlElement("Mark", Order = 1)]
        public Mark MarkObject { get; set; }

        [XmlElement("ExternalGraphic", Order = 2)]
        public ExternalGraphic ExternalGraphicObject { get; set; }

        [XmlElement(Order = 3)]
        public double Opacity { get; set; } = 1;

        [XmlElement(Order = 4)]
        public double Size { get; set; }

        [XmlElement(Order = 5)]
        public double Rotation { get; set; }

        public bool ShouldSerializeOpacity => Opacity != 1;
        public bool ShouldSerializeSize => Opacity != 0;
        public bool ShouldSerializeRotation => Opacity != 0;
 
    }  
}
