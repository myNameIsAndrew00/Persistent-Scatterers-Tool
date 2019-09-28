using MapWebSite.Authentication;
using MapWebSite.Repository;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Interaction.Owin
{
    public static class ComponentsFactory
    {

        public static UserManager CreateUserManager(IdentityFactoryOptions<UserManager> options, IOwinContext context)
        {
            return UserManager.Create(options,
                                      context,
                                      new SQLUserRepository()); 
        }
    }
}
