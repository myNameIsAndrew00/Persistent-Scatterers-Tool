using MapWebSite.GeoserverAPI.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MapWebSite.GeoserverAPI.Modules.Layers
{

    /// <summary>
    /// Provide methods to build a layer which can be send to the Geoserver
    /// </summary>   
    public class LayersBuilder
    {
        public bool SingleLayer { get; set; }

        public string LayerName { get; set; }

        public string Workspace { get; set; }

        public List<string> Styles { get; set; }

        public string ToXml()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Layer));

            Layer layer = new Layer()
            {
                Name = LayerName,
                Styles = new List<LayerStyle>(),
                LayerTypeValue = Layer.LayerType.Vector
            };

            foreach (var style in Styles)
                layer.Styles.Add(new LayerStyle { Name = style });

            using (var stream = new StringWriter())
            using (var xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings
            {
                Indent = true,
                NamespaceHandling = NamespaceHandling.OmitDuplicates,
                OmitXmlDeclaration = true,
                Encoding = Encoding.UTF8,
            }))
            {
                serializer.Serialize(xmlWriter,
                                     layer,
                                      new XmlSerializerNamespaces(
                                        new XmlQualifiedName[] {
                                            XmlQualifiedName.Empty
                                        }
                                        ));


                return stream.ToString();
            }

        }
    }
}
