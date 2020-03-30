using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace MapWebSite.GeoserverAPI.Modules
{
    /// <summary>
    /// Provides methods to 
    /// </summary>
    public interface IGeoserverModule
    {
        string GetEndpoint();

        HttpContent GetContent();
 
    }
}
