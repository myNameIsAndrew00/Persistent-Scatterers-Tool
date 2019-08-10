using MapWebSite.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MapWebSite.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            //TODO: modify string with SecureString       
            Filters.SiteAuthenticationFilter.LogoutUser(Session);

            DatabaseInteractionHandler databaseInteractionHandler = new DatabaseInteractionHandler();

            if (databaseInteractionHandler.ValidateUser(username, password))
                Filters.SiteAuthenticationFilter.AuthenticateUser(username, Session);

            return RedirectToAction("Index", "Home");
        }

    }
}