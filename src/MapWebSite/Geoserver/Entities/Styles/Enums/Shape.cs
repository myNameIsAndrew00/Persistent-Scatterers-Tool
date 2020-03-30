using MapWebSite.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace MapWebSite.GeoserverAPI.Entities
{
    /// <summary>
    /// Represents a shape which can be used inside a Graphic object
    /// </summary>
    public enum Shape
    {
        [EnumString("circle")]
        Circle, 

        [EnumString("square")]
        Square, 

        [EnumString("triangle")]
        Triangle, 

        [EnumString("star")]
        Star, 

        [EnumString("cross")]
        Cross,

        [EnumString("x")]
        X
    }
}
