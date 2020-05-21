using MapWebSite.GeoserverAPI.Entities;
using MapWebSite.GeoserverAPI.Entities.Graphics;
using MapWebSite.GeoserverAPI.Entities.Symbolizers;
using MapWebSite.GeoserverAPI.Interfaces;
using System;
using System.Collections.Generic;

namespace MapWebSite.Model
{

    public struct Interval
    {
        public string Color { get; set; }

        public decimal Left { get; set; }

        public decimal Right { get; set; }

    }

    /// <summary>
    /// Model used for a color palette
    /// </summary>
    public class ColorMap : IRulesProvider
    {

        [Flags]
        public enum ColorMapStatus
        {
            Uploaded = 1,
            CassandraRequested = 2,
            CassandraUploaded = 4
        }

        public string Name { get; set; }

        public int StatusMask { get; set; }

        public List<Interval> Intervals { get; set; }

        /// <summary>
        /// This property represents the main criteria used for drawing points colors
        /// </summary>
        public string MainColorCriteria { get; set; } = "Height";


        #region Geoserver API Interface

        public IEnumerable<Rule> GetRules()
        {
            foreach (var interval in Intervals)
                yield return new Rule
                {
                    Name = $"{Name}_interval{interval.Left}_{interval.Right}",
                    Title = $"{Name}_interval{interval.Left}_{interval.Right}",
                    Filter = new Filter
                    {
                        FilterItems = new List<Filter.FilterItem>()
                        {
                            new Filter.FilterItem
                            {
                                Type = Filter.FilterItemType.And,
                                FilterItems = new List<Filter.FilterItem>()
                                {
                                    new Filter.FilterItem
                                    {
                                        PropertyName = this.MainColorCriteria,
                                        Type = Filter.FilterItemType.PropertyIsGreaterThanOrEqualTo,
                                        Literal = interval.Left.ToString()
                                    },
                                    new Filter.FilterItem
                                    {
                                        PropertyName = this.MainColorCriteria,
                                        Type = Filter.FilterItemType.PropertyIsLessThan,
                                        Literal = interval.Right.ToString()
                                    }
                                }
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
                                Size = 6,
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
                                                Value = interval.Color
                                            }
                                        },

                                    }
                                }
                            }
                        }
                    }
                };
        }

        #endregion

    }
}
