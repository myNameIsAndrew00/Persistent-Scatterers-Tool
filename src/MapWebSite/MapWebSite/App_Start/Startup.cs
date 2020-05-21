using Microsoft.Owin;
using Owin;
using MapWebSite.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;



[assembly: OwinStartup(typeof(MapWebSite.Startup))]

namespace MapWebSite
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            /*Register the authentication component to the application*/

            app.CreatePerOwinContext<UserManager>(Domain.Owin.ComponentsFactory.CreateUserManager);
            app.CreatePerOwinContext<SignInManager>(SignInManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Login/Index"),

                Provider = new CookieAuthenticationProvider { }
            });


            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            
            app.MapSignalR();

            Domain.DomainLoader.Load();


        } 
    }
}