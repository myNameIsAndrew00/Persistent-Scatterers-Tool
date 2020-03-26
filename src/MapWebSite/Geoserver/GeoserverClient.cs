using GeoserverAPI.Modules;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GeoserverAPI
{
    /// <summary>
    /// Represents a class to handle interaction with Geoserver API
    /// </summary>
    public class GeoserverClient
    {

        private string serverUrl = null;

        /// <summary>
        /// Requires the geoserver url 
        /// </summary>
        /// <param name="serverUrl">Url to the server where the requests will be made</param>
        public GeoserverClient(string serverUrl)
        {
            this.serverUrl = serverUrl;
        }

        public async Task<bool> CreateRequest(IGeoserverModule module)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(serverUrl);

                await client.PostAsync(
                    module.GetEndpoint(), 
                    module.GetContent());
            }

            return true;

        } 
    }
}
