using MapWebSite.Core;
using MapWebSite.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MapWebSite.GeoserverAPI.Entities
{
   
    public sealed class Filter
    {

        public enum FilterItemType
        {
            [EnumString("PropertyIsEqualTo")]
            PropertyIsEqualTo,

            [EnumString("PropertyIsNotEqualTo")]
            PropertyIsNotEqualTo,

            [EnumString("PropertyIsLessThan")]
            PropertyIsLessThan,

            [EnumString("PropertyIsLessThanOrEqualTo")]
            PropertyIsLessThanOrEqualTo,

            [EnumString("PropertyIsGreaterThan")]
            PropertyIsGreaterThan,

            [EnumString("PropertyIsGreaterThanOrEqualTo")]
            PropertyIsGreaterThanOrEqualTo
        }

        public class FilterItem : IXmlSerializable
        {
            [XmlIgnore]
            public FilterItemType Type { get; set; }
          
            public string PropertyName { get; set; }

            public string Literal { get; set; }

            public XmlSchema GetSchema()
            {
                return null;
            }

            public void ReadXml(XmlReader reader)
            {
                throw new NotImplementedException();
            }

            public void WriteXml(XmlWriter writer)
            {           
                writer.WriteStartElement(this.Type.GetEnumString(), Namespaces.OGC);
                writer.WriteElementString(nameof(this.PropertyName), Namespaces.OGC, PropertyName);               
                writer.WriteElementString(nameof(this.Literal), Namespaces.OGC, Literal);
                writer.WriteEndElement();
            }
        }

 
    }


}
