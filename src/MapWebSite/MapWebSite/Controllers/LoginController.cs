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
            var userManager = HttpContext.GetOwinContext().Get<UserManager>();

            var signInStatus = signInManager.PasswordSignIn(username, password, true, false);

            if (!userManager.IsEmailConfirmedAsync(username).Result)
                return RedirectToAction("Index");

            if (signInStatus == SignInStatus.Success)
            {
                return RedirectToAction("Index", "Home");            
            }
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
                                     string password,
                                     string email
            )
        {
            UserManager userManager = HttpContext.GetOwinContext()
                                                  .GetUserManager<UserManager>();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return RedirectToAction("Index");
            if (username == AnonymousUser.Get.Username)
                return RedirectToAction("Index");
            
            var createTask = userManager.CreateAsync(Authentication.User.Create(username, firstName, lastName, email),
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

            if (createTask.Result.Errors?.FirstOrDefault()?.Contains("Passwords must be at least") ?? false)
                return Json(new { message = TextDictionary.LRegisterFailMessagePasswordLen, type = "Failed" });
            if (createTask.Result.Errors?.FirstOrDefault()?.Contains("one non letter or digit") ?? false)
                return Json(new { message = TextDictionary.LRegisterFailMessageLetterOrDigit, type = "Failed" });
            if (createTask.Result.Errors?.FirstOrDefault()?.Contains("one digit ('0'-'9')") ?? false)
                return Json(new { message = TextDictionary.LRegisterFailMessageDigit, type = "Failed" });
            if (createTask.Result.Errors?.FirstOrDefault()?.Contains("one uppercase") ?? false)
                return Json(new { message = TextDictionary.LRegisterFailMessageUppercase, type = "Failed" });


            if (createTask.Result.Succeeded)
            {
                var user = userManager.FindAsync(username, password).Result;   
                var confirmationCode = userManager.GenerateEmailConfirmationTokenAsync(user.Id).Result;

                var callbackUrl = Url.Action("ConfirmEmail",
                                             "Login",
                                             new { userId = user.Id, code = confirmationCode },
                                             protocol: Request.Url.Scheme);

                userManager.SendEmailAsync(user.Id,
                                           TextDictionary.RegisterConfirmationEmailSubject,
                                           string.Format(TextDictionary.RegisterConfirmationEmailBody, callbackUrl)
                                           );

                return Json(TextDictionary.LRegisterSuccessMessage, "Success");
            }

            else return Json(TextDictionary.LRegisterFailMessage, "Failed");           
        }

        [HttpGet]
        public ActionResult ConfirmEmail(string userId, string code)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
                return View((object)TextDictionary.LCEFailMessage);

            UserManager userManager = HttpContext.GetOwinContext()
                                                 .GetUserManager<UserManager>();

            var result = userManager.ConfirmEmailAsync(userId, code).Result;
            if (result.Succeeded)
            {
                return View((object)TextDictionary.LCESuccessMessage);
            }

            return View((object)TextDictionary.LCEFailMessage);
        }

    }
}