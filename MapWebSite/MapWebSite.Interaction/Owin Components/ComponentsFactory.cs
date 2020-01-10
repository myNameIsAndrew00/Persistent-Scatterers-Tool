using MapWebSite.Authentication;
using MapWebSite.Repository;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace MapWebSite.Domain.Owin
{
    public static class ComponentsFactory
    {
        /// <summary>
        /// Create the user manager used by web application. Used to inject the repository 
        /// </summary>  
        public static UserManager CreateUserManager(IdentityFactoryOptions<UserManager> options, IOwinContext context)
        {
            return UserManager.Create(options,
                                      context,
                                      new SQLUserRepository()); 
        }
    }
}
