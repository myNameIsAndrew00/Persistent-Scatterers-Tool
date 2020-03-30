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

namespace MapWebSite.Tests.Core
{
    [TestClass]

    public class GeoserverAPI
    {
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
                MaxScaleDenominator = 10,
                MinScaleDenominator = 10,
                Name = "Rulename",
                Title = "Ruletitle",
                FilterItems = new List<Filter.FilterItem>()
                    {
                        new Filter.FilterItem
                        {
                            PropertyName = "Height",
                            Type = Filter.FilterItemType.PropertyIsGreaterThanOrEqualTo,
                            Literal = "0"
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
                FilterItems = new List<Filter.FilterItem>()
                    {
                        new Filter.FilterItem
                        {
                            PropertyName = "Height",
                            Type = Filter.FilterItemType.PropertyIsGreaterThanOrEqualTo,
                            Literal = "0"
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


            var result = geoserverClient.CreateRequest(modulesFactory.CreateStylesModule(builder)).Result;
        }
    }
}
