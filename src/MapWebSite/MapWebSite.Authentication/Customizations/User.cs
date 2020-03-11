using Microsoft.AspNet.Identity; 
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MapWebSite.Core;
using MapWebSite.Model;

namespace MapWebSite.Authentication
{
    public class User : Model.User, IUser<string>
    {
        public string Id => Username;

        public string UserName { get => Username; set => Username = value; }

        public static User Create(string username, string firstName, string lastName, string email)
        {
            return new User()
            {
                FirstName = firstName,
                LastName = lastName,
                Username = username,
                Email = email,
                SecurityStamp = Guid.NewGuid().ToString("x"),
            };
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager manager)
        {
            manager.UpdateSecurityStamp(Username); 
            //AuthenticationType must be the same as the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            //Add custom user claims here
            userIdentity.AddClaim(new Claim(Claims.Username, Username, ClaimValueTypes.String));
            userIdentity.AddClaim(new Claim(Claims.FirstName, FirstName, ClaimValueTypes.String));
            userIdentity.AddClaim(new Claim(Claims.LastName, LastName, ClaimValueTypes.String));           

            return userIdentity;
        }
    }

    public class AnonymousUser : User
    {
        private static readonly AnonymousUser anonymousUser = new AnonymousUser();

        public static AnonymousUser Get => anonymousUser;

        public static IList<string> Roles { get; }

        static AnonymousUser()
        {
            Roles = new List<string> {
                    UserRoles.Anonymous.GetEnumString()
                };
        }

        private AnonymousUser()
        {
            this.FirstName = "Anonymous";
            this.LastName = "Anonymous";
            this.Username = "Anonymous";
        }
    }
}
