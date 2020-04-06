using MapWebSite.GeoserverAPI.Modules.Styles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MapWebSite.GeoserverAPI.Entities;
using MapWebSite.GeoserverAPI.Entities.Symbolizers;
using MapWebSite.GeoserverAPI.Entities.Graphics;
using MapWebSite.GeoserverAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapWebSite.Model;
using MapWebSite.GeoserverAPI.Modules.Layers;

namespace MapWebSite.Tests.Core
{
    [TestClass]

    public class GeoserverAPI
    {

        [TestMethod]
        public void LayersBuilderTest()
        {
            LayersBuilder layersBuilder = new LayersBuilder();
            layersBuilder.LayerName = "constanta_labeled";
            layersBuilder.Workspace = "constanta";
            layersBuilder.Styles = new List<string>()
            {
                "population"
            };

            string value = layersBuilder.ToXml();
        }

        [TestMethod]
        public void StylesBuilderTest()
        {

            string desiredResult =
                    @"< StyledLayerDescriptor version= ""1.0.0""
                xsi: schemaLocation = ""http://www.opengis.net/sld StyledLayerDescriptor.xsd""
                xmlns = ""http://www.opengis.net/sld""
                xmlns: ogc = ""http://www.opengis.net/ogc""
                xmlns: xlink = ""http://www.w3.org/1999/xlink""
                xmlns: xsi = ""http://www.w3.org/2001/XMLSchema-instance"">
                       <NamedLayer>
                         <Name>Simple point</Name>
                            <UserStyle>
                              <Title>GeoServer SLD Cook Book: Simple point</Title>
                                 <FeatureTypeStyle>
                                   <Rule>
                                     <PointSymbolizer>
                                       <Graphic>
                                         <Mark>
                                           <WellKnownName> circle </WellKnownName>
                                           <Fill>
                                             <CssParameter name=""fill"">#FF0000</CssParameter>
                                           </Fill>
                                         </Mark>       
                                         <Size>6</Size>        
                                       </Graphic>       
                                     </PointSymbolizer>        
                                   </Rule>        
                          </FeatureTypeStyle>        
                        </UserStyle>        
                      </NamedLayer>        
                    </StyledLayerDescriptor>";


            StylesBuilder builder = new StylesBuilder("style", "titlestyle");

            builder.AddRule(new Rule
            {
                Abstract = "abstract", 
                Name = "Rulename",
                Title = "Ruletitle",
                Filter = new Filter
                {
                    FilterItems = new List<Filter.FilterItem>()
                    {
                        new Filter.FilterItem
                        {
                            PropertyName = "Height",
                            Type = Filter.FilterItemType.PropertyIsGreaterThanOrEqualTo,
                            Literal = "0"
                        }
                    }
                }
                ,
                PointSymbolizers = new List<PointSymbolizer>
                {
                    new PointSymbolizer
                    {                      
                        Graphic = new Graphic
                        { 
                            MarkObject = new Graphic.Mark()
                            {
                                WellKnownNameProperty = Shape.Circle,
                                Fill = new Fill
                                {
                                    CssParameterArray = new List<CssParameter>()
                                    {
                                        new CssParameter
                                        {
                                            Name = "fill",
                                            Value = "#FF0000"
                                        },
                                        new CssParameter
                                        {
                                            Name = "fill-opacity",
                                            Value = "1"
                                        }
                                    },

                                }
                            }
                        }
                    }
                }
            });

            string result = builder.ToXml();

        }

        [TestMethod]
        public void ColorMapStyle() {
            ColorMap colorMap = new ColorMap()
            {
                Name = "rada",
                Intervals = new List<Interval>()
                {
                    new Interval()
                    {
                        Color = "#FF0000",
                        Left = -100,
                        Right = 0
                    },
                    new Interval()
                    {
                        Color = "#00FF00",
                        Left = 0,
                        Right = 100
                    }
                }
            };


            StylesBuilder builder = new StylesBuilder("colormap", "colormap");

            foreach (var rule in colorMap.GetRules())
                builder.AddRule(rule);
            
            ModulesFactory modulesFactory = new ModulesFactory();


            //    GeoserverClient geoserverClient = new GeoserverClient("http://localhost:8080", "admin", "geoserver");

            var str = builder.ToXml();
        //    var result = geoserverClient.CreateRequest(modulesFactory.CreateStylesModule(builder)).Result;

        }

        [TestMethod]
        public void GeoserverServiceTest()
        {
            StylesBuilder builder = new StylesBuilder("style", "titlestyle");

            builder.AddRule(new Rule
            {
                Abstract = "abstract",
                MaxScaleDenominator = 10,
                MinScaleDenominator = 10,
                Name = "Rulename",
                Title = "Ruletitle",
                Filter = new Filter
                {
                    FilterItems = new List<Filter.FilterItem>()
                    {
                        new Filter.FilterItem
                        {
                            PropertyName = "Height",
                            Type = Filter.FilterItemType.PropertyIsGreaterThanOrEqualTo,
                            Literal = "0"
                        }
                    }
                }
                ,
                PointSymbolizers = new List<PointSymbolizer>
                {
                    new PointSymbolizer
                    {
                        Graphic = new Graphic
                        {
                            MarkObject = new Graphic.Mark()
                            {
                                WellKnownNameProperty = Shape.Circle,
                                Fill = new Fill
                                {
                                    CssParameterArray = new List<CssParameter>()
                                    {
                                        new CssParameter
                                        {
                                            Name = "fill",
                                            Value = "#FF0000"
                                        },
                                        new CssParameter
                                        {
                                            Name = "fill-opacity",
                                            Value = "1"
                                        }
                                    },

                                }
                            }
                        }
                    }
                }
            });

            ModulesFactory modulesFactory = new ModulesFactory();

            GeoserverClient geoserverClient = new GeoserverClient("http://localhost:8080", "admin", "geoserver");


            var result = geoserverClient.Post(modulesFactory.CreateStylesModule(builder)).Result;
        }

        [TestMethod]
        public void GeoserverServiceLayerTest()
        {
            ModulesFactory modulesFactory = new ModulesFactory();

            LayersBuilder builder = new LayersBuilder();
            builder.LayerName = "constanta_labeled";
            builder.Workspace = "constanta";
            builder.SingleLayer = true;
            builder.Styles = new List<string>()
            {
                "population"
            };


            GeoserverClient geoserverClient = new GeoserverClient("http://localhost:8080", "admin", "geoserver");


            var result = geoserverClient.Put(modulesFactory.CreateLayerModule(builder)).Result;

        }
    }
}
