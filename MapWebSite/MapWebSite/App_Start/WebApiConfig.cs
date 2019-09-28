using MapWebSite.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MapWebSite
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            //config.Filters.Add(new ApiAuthenticationFilter());

            config.Routes.Add("DefaultApi", config.Routes.CreateRoute("api/{controller}/{action}/{id}",
                                                                    new { id = RouteParameter.Optional },
                                                                    null));

            config.EnsureInitialized();
        }
    }
}
