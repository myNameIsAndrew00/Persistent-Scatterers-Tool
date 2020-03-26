using MapWebSite.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeoserverAPI
{
    internal enum Endpoints
    {
            [EnumString("/about/manifests")]
            Manifests,
            
            [EnumString("/styles")]
            Styles

    }
}
