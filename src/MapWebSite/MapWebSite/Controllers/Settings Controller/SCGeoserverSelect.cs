using MapWebSite.HtmlHelpers;
using MapWebSite.Domain;
using MapWebSite.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using MapWebSite.Resources.text;

namespace MapWebSite.Controllers
{
    /// <summary>
    /// Use this ApiController to return pages for the settings layer and to interact with it
    /// </summary> 
    [Authorize]
    public partial class SettingsController : ApiController
    {
        /// <summary>
        /// Use this method to insert a geoserver layer in databse
        /// </summary>
        /// <param name="data">An json object which contains: name (dataset name), apiUrl, defaultColorPaletteName, defaultColorPaletteUser </param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage UploadGeoserverLayer([FromBody] JObject data)
        {
            DomainInstance handler = new DomainInstance();


            var insertResult = handler.CreateDataSet(
                                     datasetName: data["name"].ToObject<string>(),
                                     username: RouteConfig.CurrentUser.Username,
                                     pointsSource: PointsSource.Geoserver,
                                     serviceUrl: data["apiUrl"].ToObject<string>(),
                                     colorPaletteName: data["defaultColorPaletteName"].ToObject<string>(),
                                     colorPaletteUser: data["defaultColorPaletteUser"].ToObject<string>());
                

            if(insertResult == Domain.ViewModel.CreateDatasetResultCode.Ok)
            {
                handler.UpdateDatasetStatus(data["name"].ToObject<string>(),
                                            DatasetStatus.Generated,
                                            RouteConfig.CurrentUser.Username);

                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(MessageBoxBuilder.Create(TextDictionary.OverlayMFSuccesTitle,
                                                                   TextDictionary.OverlayMFSuccesText,
                                                                   true))
                };
            }

            //Log a warning message
            Core.CoreContainers.LogsRepository.LogWarning($"Failed to insert geoserver layer ( ({ insertResult.ToString()} ) with data provided: {data}", Core.Database.Logs.LogTrigger.Controllers);

            return new HttpResponseMessage()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(MessageBoxBuilder.Create(
                    insertResult == Domain.ViewModel.CreateDatasetResultCode.GeoserverError ? 
                        TextDictionary.OverlayCDGeoserverFailTitle :
                        TextDictionary.OverlayCDFailTitle,
                    insertResult == Domain.ViewModel.CreateDatasetResultCode.GeoserverError ? 
                        string.Format(TextDictionary.OverlayCDGeoserverFailText, data["name"].ToObject<string>()) :
                        TextDictionary.OverlayCDFailText))
            };
        }


    }
}
