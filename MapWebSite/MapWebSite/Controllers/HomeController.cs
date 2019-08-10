using MapWebSite.Model;
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
                default:
                    return View((string)"Settings Content/ColorPicker");
            }
        }
         
        //TODO:require authentification
        public ActionResult SaveColorsPalette(ColorMap colorMap)
        {
            //TODO: save the color map to the database
            return View();
        }

        /*this class is for testing purpose*/
        public class MapPoint
        {
            public double Longitude { get; set; }

            public double Latitude { get; set; }
        }

        [HttpGet] 
        public JsonResult RequestDataPoints()
        {
            Random randomSource = new Random();

            //mock data
            List<MapPoint> pointsData = new List<MapPoint>()
            {
                new MapPoint() {
                    Longitude = 26.102538390000063,
                    Latitude = 44.4267674
                },
            };


            for (int i = 0; i < 1000; i++)
                pointsData.Add(new MapPoint()
                {
                    Longitude = 26.102538390000063 + randomSource.NextDouble() * 10,
                    Latitude = 44.4267674 + randomSource.NextDouble() * 10
                });


            return Json(pointsData, JsonRequestBehavior.AllowGet);
        }

    }
}