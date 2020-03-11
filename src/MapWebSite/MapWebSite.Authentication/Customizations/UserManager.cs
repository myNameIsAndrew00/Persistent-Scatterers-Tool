using MapWebSite.Core.Database;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Text;

namespace MapWebSite.Authentication
{
    public class UserManager : UserManager<User,string>
    { 
        public UserManager(IUserStore<User, string> store)
         : base(store)
        { 
        }

        public static UserManager Create(IdentityFactoryOptions<UserManager> options, 
                                         IOwinContext context, 
                                         IUserRepository userRepository)
        {            
            
            var manager = new UserManager(new Store(userRepository))
            { 
                PasswordHasher = new Authentication.PasswordHasher(),        
                PasswordValidator = new PasswordValidator()
                {
                    RequiredLength = 6,
                    RequireDigit = true,
                    RequireLowercase = true,
                    RequireNonLetterOrDigit = true,
                    RequireUppercase = true
                },
                UserTokenProvider = new EmailTokenProvider<User>(),
                EmailService = new EmailService(),
                
                
            };

            manager.UserLockoutEnabledByDefault = false;

            return manager;
        }
    }
}
