﻿using MapWebSite.HtmlHelpers;
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
        [HttpPost]
        public HttpResponseMessage UploadGeoserverLayer([FromBody] JObject data)
        {
            DatabaseInteractionHandler handler = new DatabaseInteractionHandler();


            var insertResult = handler.CreateDataSet(
                                     datasetName: data["name"].ToObject<string>(),
                                     username: RouteConfig.CurrentUser.Username,
                                     pointsSource: PointsSource.Geoserver,
                                     apiUrl: string.Empty);
                

            if(insertResult == DatabaseInteractionHandler.CreateDatasetResultCode.Ok)
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
            return new HttpResponseMessage()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(MessageBoxBuilder.Create(
                    insertResult == DatabaseInteractionHandler.CreateDatasetResultCode.GeoserverError ? 
                        TextDictionary.OverlayCDGeoserverFailTitle :
                        TextDictionary.OverlayCDFailTitle,
                    insertResult == DatabaseInteractionHandler.CreateDatasetResultCode.GeoserverError ? 
                        string.Format(TextDictionary.OverlayCDGeoserverFailText, data["name"].ToObject<string>()) :
                        TextDictionary.OverlayCDFailText))
            };
        }


    }
}