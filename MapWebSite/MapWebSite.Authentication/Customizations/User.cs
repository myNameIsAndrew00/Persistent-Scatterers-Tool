using Microsoft.AspNet.Identity; 
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MapWebSite.Core;

namespace MapWebSite.Authentication
{
    public class User : Model.User, IUser<string>
    {
        public string Id => Username;

        public string UserName { get => Username; set => Username = value; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager manager)
        {
            //AuthenticationType must be the same as the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            //Add custom user claims here
            userIdentity.AddClaim(new Claim(Claims.Username, Username, ClaimValueTypes.String));
            userIdentity.AddClaim(new Claim(Claims.FirstName, FirstName, ClaimValueTypes.String));
            userIdentity.AddClaim(new Claim(Claims.LastName, LastName, ClaimValueTypes.String));           

            return userIdentity;
        }
    }
}
