using MapWebSite.GeoserverAPI.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MapWebSite.GeoserverAPI.Interfaces
{
    /// <summary>
    /// Provides methods to generate a Rule based on object
    /// </summary>
    interface IRuleProvider
    {
        Rule GetRule();
    }
}
