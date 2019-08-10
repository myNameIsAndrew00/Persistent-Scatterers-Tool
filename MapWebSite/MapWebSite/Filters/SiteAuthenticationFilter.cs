using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace MapWebSite.Filters
{
    /// <summary>
    /// Use this filter-attribute to decorate each action that must be authenticated
    /// </summary>
    public class SiteAuthenticationFilter : ActionFilterAttribute, IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            if (string.IsNullOrEmpty(Convert.ToString(filterContext.HttpContext.Session["username"])))
                filterContext.Result = new HttpUnauthorizedResult();
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            //check if an unauthorized access was raised
            if (filterContext.Result == null || filterContext.Result is HttpUnauthorizedResult)
                //TODO: send the user to a special where he will be redirected to login
                filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary()
                {
                    { "controller", "Login" },
                    { "action", "Index" }
                });
        }

        public static void AuthenticateUser(string username, HttpSessionStateBase session)
        {
            //any authentication changes are made here
            session["username"] = username;
        }

        public static void LogoutUser(HttpSessionStateBase session)
        {
            session["username"] = null;
        }
    }
}