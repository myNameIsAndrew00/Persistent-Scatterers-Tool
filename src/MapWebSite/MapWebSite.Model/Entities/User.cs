using MapWebSite.Types;
using Newtonsoft.Json;

namespace MapWebSite.Model
{
    /// <summary>
    /// Main user model used in solution
    /// </summary>
    public class User
    {
        public string Username { get; set; }

        [JsonIgnore]
        public byte[] PasswordHash { get; set; }         

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [JsonIgnore]
        public string SecurityStamp { get; set; }

        public string Email { get; set; }

        [JsonIgnore]
        public bool ConfirmedEmail { get; set; }
         
    }

    /// <summary>
    /// User possible roles.
    /// *Enum must be always mapped with Roles table from the database
    /// </summary>
    public enum UserRoles
    {
        [EnumString("Anonymous")]
        Anonymous ,
        [EnumString("Normal")]
        Normal,
        [EnumString("Administrator")]
        Administrator,
        [EnumString("SuperAdministrator")]
        SuperAdministrator
    }
}
