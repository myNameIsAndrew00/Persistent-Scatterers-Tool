using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Domain
{
    public class SiteIdentity : IIdentity
    {
        public string Name { get; set; }

        public string AuthenticationType { get; set; } = "default"; //modify here in the future

        public bool IsAuthenticated { get; set; } = true; //modify here in the future

     
    }

    public class SiteUser : IPrincipal
    {        
        public IIdentity Identity { get; set; } 
         
        public SiteUser(SiteIdentity identity)
        {
            this.Identity = identity;
        }

        public bool IsInRole(string role)
        {
            return true;
        }

    }
}
