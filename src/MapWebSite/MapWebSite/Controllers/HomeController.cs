using MapWebSite.Core;
using MapWebSite.HtmlHelpers;
using MapWebSite.Domain;
using MapWebSite.Model; 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;
using System.Net.Http;

namespace MapWebSite.Controllers
{
    [System.Web.Mvc.Authorize]
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }


        [System.Web.Mvc.HttpGet]
        public ActionResult RequestSettingsLayerContent(SettingsController.Page settingsPage)
        {
            switch (settingsPage)
            {
                case SettingsController.Page.ColorPicker:
                    return View((string)"Settings Content/ColorPicker");
                case SettingsController.Page.UploadPoints:
                    return View((string)"Settings Content/UploadPoints");
                case SettingsController.Page.Account:
                    return View((string)"Settings Content/AccountSettings");
                case SettingsController.Page.About:
                    return View((string)"Settings Content/About");
                default:
                    return View((string)"Settings Content/ColorPicker");
            }
        }

     

        [System.Web.Mvc.HttpGet]
        public ActionResult Logout()
        {
            var AuthenticationManager = HttpContext.GetOwinContext().Authentication;
            AuthenticationManager.SignOut();

            return RedirectToAction("Index", "Login");
        }
    }


    [System.Web.Http.Authorize]
    public class HomeApiController : ApiController
    {
        [System.Web.Http.HttpGet]
        public HttpResponseMessage RequestPointDetails(decimal latitude,
                                             decimal longitude,
                                             int identifier,
                                             decimal zoomLevel,
                                             string username,
                                             string datasetName)
        {
            DatabaseInteractionHandler databaseInteractionHandler = new DatabaseInteractionHandler();

            //*zoomLevel is not required anymore
            var point = databaseInteractionHandler.RequestPointDetails(datasetName,
                                                                       username,
                                                                       0,
                                                                       new PointBase()
                                                                       {
                                                                           Latitude = latitude,
                                                                           Longitude = longitude,
                                                                           Number = identifier
                                                                       });
            var response = new HttpResponseMessage();
            response.Content = new StringContent( point.JSONSerialize() );
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            return response;
             
        }


        [System.Web.Http.HttpGet]
        public HttpResponseMessage RequestRegionsKeys(decimal latitudeFrom,
                                     decimal longitudeFrom,
                                     decimal latitudeTo,
                                     decimal longitudeTo,
                                     int zoomLevel,
                                     string username,
                                     string datasetName)
        {
            DatabaseInteractionHandler databaseInteractionHandler = new DatabaseInteractionHandler();
            var keys = databaseInteractionHandler.RequestPointsRegionsKeys(
                                      new Tuple<decimal, decimal>(latitudeFrom, longitudeFrom),
                                      new Tuple<decimal, decimal>(latitudeTo, longitudeTo),
                                      zoomLevel,
                                      username,
                                      datasetName);
            
            var response = new HttpResponseMessage();
            response.Content = new StringContent(keys.JSONSerialize());
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            return response;
        }
    }
}