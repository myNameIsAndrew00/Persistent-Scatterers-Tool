

namespace MapWebSite.Model
{
    public class User
    {
        public string Username { get; set; }

        public byte[] PasswordHash { get; set; }         

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
