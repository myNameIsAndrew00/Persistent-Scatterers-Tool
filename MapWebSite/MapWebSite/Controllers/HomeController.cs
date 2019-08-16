using MapWebSite.Core;
using MapWebSite.Interaction;
using MapWebSite.Model;
using ServiceStack.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

            var pointsData = databaseInteractionHandler.RequestPoints(new Tuple<decimal, decimal>(latitudeFrom, longitudeFrom),
                                      new Tuple<decimal, decimal>(latitudeTo, longitudeTo),
                                      "woofwoof",
                                      "mainTest",
                                      zoomLevel % 20);

            return Json(new { data = pointsData.DataContractJSONSerialize() }, JsonRequestBehavior.AllowGet);
        }

    }
}