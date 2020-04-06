using MapWebSite.GeoserverAPI.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MapWebSite.GeoserverAPI.Modules.Styles
{
    /// <summary>
    /// Provide methods to build a style which can be send to the Geoserver
    /// </summary>   

    public sealed class StylesBuilder
    {


        public string Name { get; }

        private readonly string title;

        private List<Rule> rules = new List<Rule>();

        public StylesBuilder(string name, string title)
        {
            this.Name = name;
            this.title = title;
        }

        public StylesBuilder AddRule(Rule rule)
        {
            this.rules.Add(rule);
            return this;
        }

        public string ToXml()
        {
           
            StyledLayerDescriptor styledLayerDescriptor = new StyledLayerDescriptor
            {
                NamedLayerObject = new StyledLayerDescriptor.NamedLayer
                {
                    Name = this.Name
                }
            }
            ;

            styledLayerDescriptor.NamedLayerObject.UserStyleObject = new StyledLayerDescriptor.NamedLayer.UserStyle
            {
                Name = this.Name,
                Title = this.title,
                FeatureTypeStyleObject = new StyledLayerDescriptor.NamedLayer.UserStyle.FeatureTypeStyle
                {
                    Rules = this.rules
                }
            };

            XmlSerializer serializer = new XmlSerializer(typeof(StyledLayerDescriptor));
             

            using (var stream = new StringWriter())
            using (var xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true,
                Encoding = Encoding.UTF8,
               
            }))
            {             
                serializer.Serialize(xmlWriter,
                                     styledLayerDescriptor,
                                      new XmlSerializerNamespaces(                                         
                                        new XmlQualifiedName[] {
                                            new XmlQualifiedName("xlink", Namespaces.XLINK),
                                            new XmlQualifiedName("ogc", Namespaces.OGC),
                                            new XmlQualifiedName("schemaLocation", Namespaces.SCHEMA_LOCATION ),                                                                                   
                                        }));
            

                return stream.ToString();
            }
              
        }
    }
}
