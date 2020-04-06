using MapWebSite.GeoserverAPI.Modules;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.GeoserverAPI
{
    /// <summary>
    /// Represents a class to handle interaction with Geoserver API
    /// </summary>
    public class GeoserverClient
    {

        private string serverUrl = null;
        private string serviceCredentials = null;

        /// <summary>
        /// Requires the geoserver url 
        /// </summary>
        /// <param name="serverUrl">Url to the server where the requests will be made</param>
        public GeoserverClient(string serverUrl, string username, string password)
        {
            this.serverUrl = serverUrl;
            this.serviceCredentials = Convert.ToBase64String(
                Encoding
                .ASCII
                .GetBytes(username + ":" + password));

        }

        /// <summary>
        /// Creates a POST request using a specific module
        /// </summary>
        /// <param name="module">Module used to provide request body content</param>
        /// <returns>A boolean which indicates if the request succeed</returns>
        public async Task<bool> Post(IGeoserverModule module)
        {
            return await this.createRequest(module, HttpMethod.Post);
        }

        /// <summary>
        /// Creates a PUT request using a specific module
        /// </summary>
        /// <param name="module">Module used to provide request body content</param>
        /// <returns></returns>
        public async Task<bool> Put(IGeoserverModule module)
        {
            return await this.createRequest(module, HttpMethod.Put);
        }

        #region Private

        private async Task<bool> createRequest(IGeoserverModule module, HttpMethod method)
        {
            string[] encodings = new[]
           {
                "application/vnd.ogc.sld+xml",
                "application/xml",
                "application/json"
            };

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(serverUrl);
                foreach (var encoding in encodings)
                    client.DefaultRequestHeaders
                          .Accept
                          .Add(new MediaTypeWithQualityHeaderValue(encoding));

                //todo: add more headers here

                using (HttpRequestMessage request = new HttpRequestMessage(method, module.GetEndpoint()))
                {
                    request.Content = module.GetContent();
                    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", this.serviceCredentials);

                    HttpResponseMessage response = await client.SendAsync(request);

                    return response.IsSuccessStatusCode;
                }
            }             
        }


        #endregion

    }
}
