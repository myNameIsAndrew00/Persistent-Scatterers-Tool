using MapWebSite.Core;
using MapWebSite.HtmlHelpers;
using MapWebSite.Interaction;
using MapWebSite.Model; 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace MapWebSite.Controllers
{
    [Authorize] 
 //   [Filters.SiteAuthenticationFilter]
    public class HomeController : Controller
    {

        Func<int, int> choseZoomLevel = delegate (int zoomLevel)
        {
            switch (zoomLevel)
            {
                case 20:
                case 19:
                case 18:
                case 17:
                    return 0;
                case 16:
                case 15:
                case 14:
                    return 16;
                case 13:
                case 12:
                case 11:
                    return 14;
                default:
                    return zoomLevel;
            }
        };

        public ActionResult Index()
        {
            var User = RouteConfig.CurrentUser;
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
     
        [HttpGet] 
        public JsonResult RequestDataPoints(decimal latitudeFrom, decimal longitudeFrom, decimal latitudeTo, decimal longitudeTo, int zoomLevel, string optionalField)
        {
            DatabaseInteractionHandler databaseInteractionHandler = new DatabaseInteractionHandler();
            //TODO: change the usern and the dataset name


            IEnumerable<BasicPoint> pointsData = databaseInteractionHandler.RequestPoints(new Tuple<decimal, decimal>(latitudeFrom, longitudeFrom),
                                      new Tuple<decimal, decimal>(latitudeTo, longitudeTo),
                                      "woofwoof", //this will be changed and customized for current user
                                      "mainTest", //this will be changed and customized for current user
                                      choseZoomLevel(zoomLevel),
                                      (BasicPoint.BasicInfoOptionalField)Enum.Parse(typeof(BasicPoint.BasicInfoOptionalField),optionalField));

            return Json(new { data = pointsData.DataContractJSONSerialize() }, JsonRequestBehavior.AllowGet);
        }
            
        [HttpGet]
        public JsonResult RequestPointDetails(decimal latitude, decimal longitude, int identifier, int zoomLevel)
        {
            //TODO: change the user and the dataset name
            DatabaseInteractionHandler databaseInteractionHandler = new DatabaseInteractionHandler();
            
            var point = databaseInteractionHandler.RequestPointDetails("mainTest",//this will be changed and customized for current user
                                                                       "woofwoof",//this will be changed and customized for current user
                                                                       choseZoomLevel(zoomLevel),
                                                                       new BasicPoint()
                                                                       {
                                                                           Latitude = latitude,
                                                                           Longitude = longitude,
                                                                           Number = identifier
                                                                       });
        
            return Json(new { data = point.JSONSerialize() }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            var AuthenticationManager = HttpContext.GetOwinContext().Authentication;
            AuthenticationManager.SignOut();

            return RedirectToAction("Index", "Login");
        }
    }
}