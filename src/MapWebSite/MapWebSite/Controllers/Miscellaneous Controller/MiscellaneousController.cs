using MapWebSite.Core;
using MapWebSite.HtmlHelpers;
using MapWebSite.Domain;
using MapWebSite.Model; 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc; 
using System.Net.Http;

namespace MapWebSite.Controllers
{
    /// <summary>
    /// This controller provides general purpose pages (or other responses)
    /// </summary>
    [System.Web.Mvc.Authorize] 
    public class MiscellaneousController : Controller
    {
        [HttpGet]
        public ActionResult GetNotificationsPage()
        {
            return View("~/Views/Home/Miscellaneous Content/Notifications.cshtml");
        }

        [HttpGet]
        public ActionResult GetChoseMapTypePage()
        {
            return View("~/Views/Home/Miscellaneous Content/ChoseMapType.cshtml");
        }
 
    }

     
}