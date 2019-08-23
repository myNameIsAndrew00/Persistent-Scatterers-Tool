using MapWebSite.Core;
using MapWebSite.Interaction;
using MapWebSite.Model; 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MapWebSite.Controllers
{
    [Filters.SiteAuthenticationFilter]
    public class HomeController : Controller
    { 
        public ActionResult Index()
        {

            return View();
        }

        [HttpGet]
        public ActionResult RequestSettingsLayerContent(string settingsPageName)
        {
            switch (settingsPageName)
            {
                case "create-palette-color":
                    return View((string)"Settings Content/ColorPicker");
                case "upload-points":
                    return View((string)"Settings Content/UploadPoints");
                default:
                    return View((string)"Settings Content/ColorPicker");
            }
        }
          
        public ActionResult SaveColorsPalette(ColorMap colorMap)
        {
            //TODO: save the color map to the database
            return View();
        }


        [HttpGet] 
        public JsonResult RequestDataPoints(decimal latitudeFrom, decimal longitudeFrom, decimal latitudeTo, decimal longitudeTo, int zoomLevel)
        {
            DatabaseInteractionHandler databaseInteractionHandler = new DatabaseInteractionHandler();
            //TODO: change the usern and the dataset name
            IEnumerable<BasicPoint> pointsData = databaseInteractionHandler.RequestPoints(new Tuple<decimal, decimal>(latitudeFrom, longitudeFrom),
                                      new Tuple<decimal, decimal>(latitudeTo, longitudeTo),
                                      "woofwoof", //this will be changed and customized for current user
                                      "mainTest", //this will be changed and customized for current user
                                      zoomLevel % 20);

            return Json(new { data = pointsData.DataContractJSONSerialize() }, JsonRequestBehavior.AllowGet);
        }
            
        [HttpGet]
        public JsonResult RequestPointDetails(decimal latitude, decimal longitude, int identifier, int zoomLevel)
        {
            //TODO: change the user and the dataset name
            DatabaseInteractionHandler databaseInteractionHandler = new DatabaseInteractionHandler();
            
            var point = databaseInteractionHandler.RequestPointDetails("mainTest",//this will be changed and customized for current user
                                                                       "woofwoof",//this will be changed and customized for current user
                                                                       zoomLevel% 20,
                                                                       new BasicPoint()
                                                                       {
                                                                           Latitude = latitude,
                                                                           Longitude = longitude,
                                                                           Number = identifier
                                                                       });
        
            return Json(new { data = point.JSONSerialize() }, JsonRequestBehavior.AllowGet);
        }
    }
}