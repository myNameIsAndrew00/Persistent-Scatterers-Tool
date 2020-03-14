using MapWebSite.Authentication;
using MapWebSite.Domain;
using MapWebSite.Resources.text;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using MapWebSite.Core;
using MapWebSite.Domain.ViewModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;

namespace MapWebSite.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [System.Web.Mvc.HttpPost]
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

        [System.Web.Mvc.HttpGet]
        public ActionResult LoginAnonymous()
        {
            var signInManager = HttpContext.GetOwinContext().Get<SignInManager>();

            var signInStatus = signInManager.PasswordSignIn("", null, true, false);

            if (signInStatus == SignInStatus.Success)
                return RedirectToAction("Index", "Home");
            else
                return RedirectToAction("Index");

        }

        [System.Web.Mvc.HttpPost]
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

                try
                {
                    userManager.SendEmailAsync(user.Id,
                                               TextDictionary.RegisterConfirmationEmailSubject,
                                               string.Format(TextDictionary.RegisterConfirmationEmailBody, callbackUrl)
                                               );
                }
                catch
                {
                    return Json(new { message = TextDictionary.RegisterConfirmationEmailFailed, type = "Failed" });
                }
                return Json(new { message = string.Format(TextDictionary.RegisterCompleteMessage, email), type = "Success" });
            }

            else return Json(new { message = TextDictionary.LRegisterFailMessage, type = "Failed" });           
        }


        [System.Web.Mvc.HttpGet]
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

   
    public class LoginApiController : ApiController
    {

        [System.Web.Http.HttpGet]
        /// <summary>
        /// Use this api method to check if a username is valid or not for registration
        /// </summary>
        /// <param name="username">Username which must be checked</param>
        /// <returns></returns>
        public HttpResponseMessage ValidateUsername(string username)
        {
            var response = new HttpResponseMessage();
            response.Content = null;

            DatabaseInteractionHandler handler = new DatabaseInteractionHandler();

            if (handler.GetUser(username, false) != null)
            {
                response.Content = new StringContent(new RegisterValidationResult()
                {
                    IsValid = false,
                    Message = Resources.text.TextDictionary.LRegisterInvalidUsernameInputMessage
                }.JSONSerialize());                
            }

            if(response.Content == null) response.Content = new StringContent(new RegisterValidationResult()
            {
                IsValid = true
            }.JSONSerialize());

            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            return response;
        }


        [System.Web.Http.HttpGet]
        /// <summary>
        /// Use this api method to check if a email is valid or not for registration
        /// </summary>
        /// <param name="username">Username which must be checked</param>
        /// <returns></returns>
        public HttpResponseMessage ValidateEmail(string email)
        {
            var response = new HttpResponseMessage();

            Func<HttpResponseMessage, HttpResponseMessage> setContentType = (httpResponse) => {
                httpResponse.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return httpResponse;
            };

            if (!new EmailAddressAttribute().IsValid(email))
            {
                response.Content = new StringContent(new RegisterValidationResult()
                {
                    IsValid = false,
                    Message = TextDictionary.LRegisterInvalidEmailFormatInputMessage
                }.JSONSerialize());

                return setContentType(response);
            }

            DatabaseInteractionHandler handler = new DatabaseInteractionHandler();
            if (handler.GetUser(email, true) != null)
            {
                response.Content = new StringContent(new RegisterValidationResult()
                {
                    IsValid = false,
                    Message = Resources.text.TextDictionary.LRegisterInvalidEmailInputMessage
                }.JSONSerialize());

                return setContentType(response);
            }


            response.Content = new StringContent(new RegisterValidationResult()
            {
                IsValid = true
            }.JSONSerialize());
            
            return setContentType(response);
        }


        [System.Web.Http.HttpGet]
        /// <summary>
        /// Use this api method to check if a password is valid or not for registration
        /// </summary>
        /// <param name="username">Username which must be checked</param>
        /// <returns></returns>
        public HttpResponseMessage ValidatePassword(string password)
        {
            var response = new HttpResponseMessage();
            response.Content = null;

            UserManager userManager = HttpContext.Current.GetOwinContext()
                                                  .GetUserManager<UserManager>();

            var validationTask = userManager.PasswordValidator.ValidateAsync(password);
            
            if (validationTask.Result.Errors?.FirstOrDefault()?.Contains("Passwords must be at least") ?? false)
                response.Content = new StringContent(new RegisterValidationResult() {
                    IsValid = false,
                    Message = TextDictionary.LRegisterInvalidPasswordInputMessage_Length }.JSONSerialize());
            else if (validationTask.Result.Errors?.FirstOrDefault()?.Contains("one non letter or digit") ?? false)
                response.Content = new StringContent(new RegisterValidationResult()
                {
                    IsValid = false,
                    Message = TextDictionary.LRegisterInvalidPasswordInputMessage_LetterOrDigit
                }.JSONSerialize());
            else if (validationTask.Result.Errors?.FirstOrDefault()?.Contains("one digit ('0'-'9')") ?? false)
                response.Content = new StringContent(new RegisterValidationResult()
                {
                    IsValid = false,
                    Message = TextDictionary.LRegisterInvalidPasswordInputMessage_Digit
                }.JSONSerialize());
            else if (validationTask.Result.Errors?.FirstOrDefault()?.Contains("one uppercase") ?? false)
                response.Content = new StringContent(new RegisterValidationResult()
                {
                    IsValid = false,
                    Message = TextDictionary.LRegisterInvalidPasswordInputMessage_Uppercase
                }.JSONSerialize());

            if (response.Content == null) response.Content = new StringContent(new RegisterValidationResult() { IsValid = true }.JSONSerialize());
            
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            return response;
        }

    }

}