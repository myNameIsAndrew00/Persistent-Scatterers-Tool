using MapWebSite.Interaction;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace MapWebSite.Filters
{
    /// <summary>
    /// Base handler for authentication. It may be is used by API and MVC filter attributes
    /// </summary>
    public static class AuthenticationHandler
    {
        public const string authenticationCookieName = "authentication";

        public static SiteUser CheckCookie(string cookieValue)
        {         
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookieValue);
            return JsonConvert.DeserializeObject<SiteUser>(ticket.UserData);
        }
    }
}