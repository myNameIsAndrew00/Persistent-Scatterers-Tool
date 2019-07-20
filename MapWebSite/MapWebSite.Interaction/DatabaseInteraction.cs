using MapWebSite.Core;
using MapWebSite.Core.Database;
using MapWebSite.Model;
using MapWebSite.Repository;
using System.Text;

namespace MapWebSite.Interaction
{
    public class DatabaseInteractionHandler
    {
        public bool RegisterUser(string username, string firstName, string lastName, string password)
        {
            IUserRepository userRepository = new SQLUserRepository();

            byte[] passwordSalt = Helper.GenerateRandomBytes(32);

            return userRepository.InsertUser(new User()
            {
                FirstName = firstName,
                LastName = lastName,
                PasswordHash = Helper.HashData(Encoding.UTF8.GetBytes(password), passwordSalt),
                PasswordSalt = passwordSalt,
                Username = username
            });
             
        }

        public bool ValidateUser(string username, string password)
        {
            IUserRepository userRepository = new SQLUserRepository();

            return userRepository.CheckUser(username, password);
        }
    }
}
