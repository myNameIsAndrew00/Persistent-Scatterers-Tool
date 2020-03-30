using MapWebSite.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace MapWebSite.GeoserverAPI
{
    internal enum Endpoints
    {
            [EnumString("/geoserver/rest/about/manifests")]
            Manifests,
            
            [EnumString("/geoserver/rest/styles?name={0}")]
            Styles

    }
}
