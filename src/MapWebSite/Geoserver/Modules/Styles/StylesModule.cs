using MapWebSite.Core;
using MapWebSite.GeoserverAPI.Modules.Styles;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace GeoserverAPI.Modules
{
    /// <summary>
    /// Represents a module which can be used to send/receive a style to Geoserver
    /// </summary>
    internal class StylesModule : IGeoserverModule
    {
        private StylesBuilder stylesBuilder = null;

        public StylesModule(StylesBuilder stylesBuilder)
        {
            this.stylesBuilder = stylesBuilder;
        }

        public HttpContent GetContent()
        {
            throw new NotImplementedException();
        }

        public string GetEndpoint()
        {
            return Endpoints.Styles.GetEnumString();
        }
    }
}
