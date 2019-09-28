using MapWebSite.Authentication;
using MapWebSite.Interaction;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MapWebSite.Controllers
{
    public class LoginController : Controller
    { 
        public ActionResult Index()
        {         
            return View();            
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var signInManager = HttpContext.GetOwinContext().Get<SignInManager>();

            var signInStatus = signInManager.PasswordSignIn(username, password, true, false);

            if (signInStatus == SignInStatus.Success)

                //TODO: modify string with SecureString       
                //Filters.SiteAuthenticationFilter.LogoutUser();

                //DatabaseInteractionHandler databaseInteractionHandler = new DatabaseInteractionHandler();


                //        if (databaseInteractionHandler.ValidateUser(username, password))
                //            Filters.SiteAuthenticationFilter.AuthenticateUser(username);
                    
                return RedirectToAction("Index", "Home");

            else

                return RedirectToAction("Index");
        }

    }
}