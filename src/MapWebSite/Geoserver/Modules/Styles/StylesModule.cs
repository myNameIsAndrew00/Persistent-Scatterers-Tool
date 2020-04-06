using MapWebSite.GeoserverAPI.Modules.Styles;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using MapWebSite.Types;

namespace MapWebSite.GeoserverAPI.Modules
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
            StringContent content = new StringContent(stylesBuilder.ToXml(),
                Encoding.UTF8,
                "application/vnd.ogc.sld+xml");
           
            return content;
        }



        public string GetEndpoint()
        {
            return string.Format(Endpoints.Styles.GetEnumString(), this.stylesBuilder.Name);
        }

      
    }
}
