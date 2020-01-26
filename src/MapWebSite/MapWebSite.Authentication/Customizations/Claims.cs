using MapWebSite.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace MapWebSite.Authentication
{
    public static class Claims
    {
        public static string FirstName = "http://schemas.naeem.com/ws/2017/02/identity/claims/firstname";

        public static string LastName = "http://schemas.naeem.com/ws/2017/02/identity/claims/lastname";

        public static string Username = "http://schemas.naeem.com/ws/2017/02/identity/claims/username";
    }
}
