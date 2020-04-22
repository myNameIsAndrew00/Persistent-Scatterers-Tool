using MapWebSite.GeoserverAPI.Modules;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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
        /// Requires the geoserver url, username and password for authentication
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
        /// Creates a async POST request using a specific module
        /// </summary>
        /// <param name="module">Module used to provide request body content</param>
        /// <returns>A boolean which indicates if the request succeed</returns>
        public async Task<bool> PostAsync(IGeoserverModule module)
        {
            //todo: implement async
            return false;
        }

        /// <summary>
        /// Creates a POST request using a specific module
        /// </summary>
        /// <param name="module">Module used to provide request body content</param>
        /// <returns>A boolean which indicates if the request succeed</returns>
        public bool Post(IGeoserverModule module)
        {
            return this.createRequest(module, HttpMethod.Post)?.IsSuccessStatusCode ?? false;
        }

        /// <summary>
        /// Creates a PUT request using a specific module
        /// </summary>
        /// <param name="module">Module used to provide request body content</param>
        /// <returns></returns>
        public async Task<bool> PutAsync(IGeoserverModule module)
        {
            //todo: implement
            return false;
        }

        /// <summary>
        /// Creates a PUT request using a specific module
        /// </summary>
        /// <param name="module">Module used to provide request body content</param>
        /// <returns></returns>
        public bool Put(IGeoserverModule module)
        {
            return this.createRequest(module, HttpMethod.Put)?.IsSuccessStatusCode ?? false;
        }

        /// <summary>
        /// Creates a GET request using a specific module
        /// </summary>
        /// <param name="module">Module used to provide request body content</param>
        /// <returns></returns>
        public T Get<T>(IGeoserverModule module)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));

                return (T)serializer.Deserialize(this.createRequest(module, HttpMethod.Get)
                                                      .Content
                                                      .ReadAsStreamAsync()
                                                      .Result);
            }
            catch(Exception exception)
            {
                return default;
            }
        }

        /// <summary>
        /// Creates a GET request using a specific module
        /// </summary>
        /// <param name="module">Module used to provide request body content</param>
        /// <returns></returns>
        public async Task<bool> GetAsync(IGeoserverModule module)
        {
            return false;
        }
 
        #region Private

        private HttpResponseMessage createRequest(IGeoserverModule module, HttpMethod method)
        {
            string[] encodings = new[]
            {
                "application/vnd.ogc.sld+xml",
                "application/xml"
            };

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(serverUrl);
                foreach (var encoding in encodings)
                    client.DefaultRequestHeaders
                          .Accept
                          .Add(new MediaTypeWithQualityHeaderValue(encoding));

                //todo: add more headers here
                try
                {
                    using (HttpRequestMessage request = new HttpRequestMessage(method, module.GetEndpoint()))
                    {
                        if(method != HttpMethod.Get) request.Content = module.GetContent();
                        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", this.serviceCredentials);

                        return client.SendAsync(request).Result;
                         
                    }
                }
                catch(Exception exception)
                {
                    return null;
                }
            }             
        }


        #endregion

    }
}
