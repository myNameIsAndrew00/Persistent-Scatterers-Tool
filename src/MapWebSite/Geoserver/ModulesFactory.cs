using MapWebSite.GeoserverAPI.Modules;
using MapWebSite.GeoserverAPI.Modules.Styles;
using System;
using System.Collections.Generic;
using System.Text;

namespace MapWebSite.GeoserverAPI
{
    /// <summary>
    /// Use this class to create modules which must be provided to GeoserverClient
    /// </summary>
    public sealed class ModulesFactory
    {
        public IGeoserverModule CreateStylesModule(StylesBuilder stylesBuilder)
        {
            return new StylesModule(stylesBuilder);
        }
    }
}
