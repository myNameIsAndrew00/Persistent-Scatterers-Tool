using MapWebSite.Core;
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
        public JsonResult RequestDataPoints()
        {
            Random randomSource = new Random();

            //mock data
            List<Point> pointsData = new List<Point>()
            {
                new Point() { //zoom 20
                    Longitude = 26.102538390000063m,
                    Latitude = 44.4267674m
                },
                new Point()
                {
                    Longitude = 26.102538390000063m - 0.961m,
                    Latitude = 44.4267674m - 0.961m
                }
            };

            for (int i = 2; i < 18; i++)
                pointsData.Add(new Point()
                {
                    Latitude = pointsData[0].Latitude + 0.0001m * i * i,
                    Longitude = pointsData[0].Longitude + 0.0001m * i * i,

                });


            for (int i = 0; i < 1000; i++)
                pointsData.Add(new Point()
                {
                    Longitude = 26.102538390000063m + (decimal)randomSource.NextDouble() * 10,
                    Latitude = 44.4267674m + (decimal)randomSource.NextDouble() * 10
                });

            return Json(new { data = pointsData.DataContractJSONSerialize() }, JsonRequestBehavior.AllowGet);
        }

    }
}