using MapWebSite.GeoserverAPI.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MapWebSite.GeoserverAPI.Interfaces
{
    /// <summary>
    /// Provides methods to generate styling rules based on object properties
    /// </summary>
    public interface IRulesProvider
    {
        IEnumerable<Rule> GetRules();
    }
}
