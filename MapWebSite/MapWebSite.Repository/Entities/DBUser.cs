﻿ 

namespace MapWebSite.Repository.Entities
{
    public class DBUser
    {
        public string Username { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
