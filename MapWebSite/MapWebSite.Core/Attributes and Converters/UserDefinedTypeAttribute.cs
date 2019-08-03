using System;

namespace MapWebSite.Core
{
    ///Decorate with this attribute every class which is a user defined type in database <br></br>
    ///This decoration can be used to generate queries, code or database objects
    public class UserDefinedTypeAttribute : Attribute
    {
        public UserDefinedTypeAttribute()
        {
        }
    }

    ///Decorate with this attribute every member which is a user defined type column in user defined type classes <br></br>
    ///This decoration can be used to generate queries, code or database objects
    public class UserDefinedTypeColumnAttribute : Attribute
    {
        public string NameInDatabase { get; set; }

        public UserDefinedTypeColumnAttribute(string nameInDatabase)
        {
            this.NameInDatabase = nameInDatabase;
        }
    }

}
