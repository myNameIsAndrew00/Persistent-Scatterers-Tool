using System;

namespace MapWebSite.Core
{
    ///<summary>
    ///Decorate with this attribute every class which is a user defined type in database <br></br>
    ///This decoration can be used to generate queries, code or database objects
    ///</summary>
    public class UserDefinedTypeAttribute : Attribute
    {
        public UserDefinedTypeAttribute()
        {
        }
    }

    ///<summary>
    ///Decorate with this attribute every member which is a user defined type column in user defined type classes <br></br>
    ///This decoration can be used to generate queries, code or database objects
    ///</summary>
    public class UserDefinedTypeColumnAttribute : Attribute
    {
        public string NameInDatabase { get; } = null;

        public bool UseIfNull { get; } = true;

        public UserDefinedTypeColumnAttribute(string nameInDatabase, bool useIfNull = true)
        {
            this.NameInDatabase = nameInDatabase;
            this.UseIfNull = useIfNull;
        }
    }

}
