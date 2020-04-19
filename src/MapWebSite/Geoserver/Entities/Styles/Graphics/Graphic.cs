using MapWebSite.GeoserverAPI.Entities.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MapWebSite.Types;

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

            [XmlElement]
            public string WellKnownName
            {
                get
                {
                    return WellKnownNameProperty.GetEnumString();
                }
                set
                {
                    Enum.TryParse<Shape>(value, true, out Shape property);
                    WellKnownNameProperty = property;

                }
            }

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

        public bool ShouldSerializeOpacity()
        {
            return Opacity != 1;
        }

        public bool ShouldSerializeSize()
        {
            return Size != 0;
        }

        public bool ShouldSerializeRotation()
        {
            return Rotation != 0;
        }

    }
}
