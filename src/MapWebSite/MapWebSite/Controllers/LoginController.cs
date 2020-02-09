using MapWebSite.Authentication;
using MapWebSite.Domain;
using MapWebSite.Resources.text;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return RedirectToAction("Index");

            var signInManager = HttpContext.GetOwinContext().Get<SignInManager>();

            var signInStatus = signInManager.PasswordSignIn(username, password, true, false);

            if (signInStatus == SignInStatus.Success)

                //TODO: modify string with SecureString                      

                return RedirectToAction("Index", "Home");

            else

                return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult LoginAnonymous()
        {
            var signInManager = HttpContext.GetOwinContext().Get<SignInManager>();

            var signInStatus = signInManager.PasswordSignIn("", null, true, false);

            if (signInStatus == SignInStatus.Success)
                return RedirectToAction("Index", "Home");
            else
                return RedirectToAction("Index");

        }

        [HttpPost]
        public ActionResult Register(string username,
                                     string firstName,
                                     string lastName,
                                     string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return RedirectToAction("Index");
            if (username == AnonymousUser.Get.Username)
                return RedirectToAction("Index");

            var createTask = HttpContext.GetOwinContext()
                                    .GetUserManager<UserManager>().CreateAsync(Authentication.User.Create(username, firstName, lastName),
                                                                            password);
            //if an exception has been throwned return a failure message
            try
            {
                createTask.Wait();
            }
            catch 
            {
                return Json(new { message = TextDictionary.LRegisterFailMessage , type = "Failed"});
            }
            

            return Json(new { message = createTask.Result.Succeeded ? 
                                          TextDictionary.LRegisterSuccessMessage
                                        : TextDictionary.LRegisterFailMessage ,
                              type = createTask.Result.Succeeded ?  "Success" : "Failed" });

        }

    }
}