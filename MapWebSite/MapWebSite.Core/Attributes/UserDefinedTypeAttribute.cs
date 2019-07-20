

using System;

namespace MapWebSite.Core
{
    //Decorate with this attribute every class which is a user defined type in database
    public class UserDefinedTypeAttribute : Attribute
    {
        public UserDefinedTypeAttribute()
        {
        }
    }
}
