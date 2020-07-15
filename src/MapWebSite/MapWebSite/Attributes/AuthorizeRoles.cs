using MapWebSite.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Web;
using System.Web.Http.Controllers;

namespace MapWebSite.Attributes
{
    public class AuthorizeApiRoles : System.Web.Http.AuthorizeAttribute
    {
        private UserRoles[] roles;

        public AuthorizeApiRoles(params UserRoles[] roles)
        {
            this.roles = roles;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            base.OnAuthorization(actionContext);
        }
    }

    public class AuthorizeRoles : System.Web.Mvc.AuthorizeAttribute
    {
        private UserRoles[] roles;

        public AuthorizeRoles(params UserRoles[] roles)
        {
            this.roles = roles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            foreach (var role in roles)
                if (httpContext.User.IsInRole(role.ToString()))
                    return true;
            
            return false;
        }
    }
}
