using MapWebSite.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace MapWebSite.GeoserverAPI
{
    /// <summary>
    /// Endpoints available for geoserver rest API
    /// </summary>
    internal enum Endpoints
    {
        [EnumString("/geoserver/rest/about/manifests")]
        Manifests,

        [EnumString("/geoserver/rest/styles?name={0}")]
        Styles,

        [EnumString("/geoserver/rest/layers")]
        Layers,
        [EnumString("/geoserver/rest/layers/{0}")]
        Layer,

        [EnumString("/geoserver/rest/workspaces/{0}/layers")]
        WorkspaceLayers,
        [EnumString("/geoserver/rest/workspaces/{0}/layers/{1}")]
        WorkspaceLayer  


    }
}
