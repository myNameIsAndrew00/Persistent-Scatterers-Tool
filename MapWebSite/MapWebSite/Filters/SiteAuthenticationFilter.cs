using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace MapWebSite.Filters
{
    /// <summary>
    /// Use this filter-attribute to decorate each action that must be authenticated
    /// </summary>
    public class SiteAuthenticationFilter : ActionFilterAttribute, IAuthenticationFilter
    {
        public const string authenticationCookieName = "authentication";

        public void OnAuthentication(AuthenticationContext filterContext)
        {
            try{
                HttpCookie cookie = filterContext.RequestContext.HttpContext.Request.Cookies[authenticationCookieName];

                if (cookie == null) throw new Exception();
                if (cookie.Expires > DateTime.Now) throw new Exception();

                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);


                filterContext.HttpContext.User = JsonConvert.DeserializeObject<Interaction.SiteUser>(ticket.UserData);
                                                         
                if(filterContext.HttpContext.User == null)  throw new Exception();
             }
            catch{
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            //check if an unauthorized access was raised
            if (filterContext.Result == null || filterContext.Result is HttpUnauthorizedResult)
                //TODO: send the user to a special page from where he will be redirected to login
                filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary()
                {
                    { "controller", "Login" },
                    { "action", "Index" }
                });
        }

        public static void AuthenticateUser(string username)
        {
            //any authentication changes are made here
            if (string.IsNullOrEmpty(username)) return;

            var userData = new JavaScriptSerializer().Serialize(new Interaction.SiteUser(new Interaction.SiteIdentity() { Name = username }));

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                                                                            username,
                                                                            DateTime.Now,
                                                                            DateTime.Now.AddDays(7),
                                                                            true,
                                                                            userData);

            string cookieData = FormsAuthentication.Encrypt(ticket);

            HttpCookie cookie = new HttpCookie(authenticationCookieName, cookieData)
            {
                Expires = ticket.Expiration,
                HttpOnly = true,
                Secure = true               
            };

            HttpContext.Current.Response.Cookies.Add(cookie); 
        }

        public static void LogoutUser()
        {
            var cookie = HttpContext.Current.Request.Cookies[authenticationCookieName];

            if (cookie != null)
                HttpContext.Current.Response.Cookies.Add(new HttpCookie(authenticationCookieName)
                {
                    Expires = DateTime.Now.AddDays(-1)
                });
            
                 
        }
 

    }
}