using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace GeoserverAPI.Modules
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
