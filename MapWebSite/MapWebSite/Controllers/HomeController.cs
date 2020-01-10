using MapWebSite.Core;
using MapWebSite.HtmlHelpers;
using MapWebSite.Domain;
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
        public JsonResult RequestPointDetails(decimal latitude, 
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