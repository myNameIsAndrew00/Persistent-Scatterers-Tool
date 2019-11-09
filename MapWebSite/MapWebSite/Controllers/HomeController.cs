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
    public class HomeController : Controller
    {

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
                case "account-settings":
                    return View((string)"Settings Content/AccountSettings"); 
                default:
                    return View((string)"Settings Content/ColorPicker");
            }
        }
           
            
        [HttpGet]
        [Obsolete]
        public JsonResult RequestPointDetails(decimal latitude, decimal longitude, int identifier, decimal zoomLevel)
        {
            //TODO: change the user and the dataset name
            DatabaseInteractionHandler databaseInteractionHandler = new DatabaseInteractionHandler();
            
            //zoomLevel is not required anymore
            var point = databaseInteractionHandler.RequestPointDetails("mainTest",//this will be changed and customized for current user
                                                                       "woofwoof",//this will be changed and customized for current user
                                                                       0,
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