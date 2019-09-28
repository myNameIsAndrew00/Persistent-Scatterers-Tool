using MapWebSite.Core.Database;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Authentication
{
    public class Store : IUserStore<User, string>,
                         IUserLockoutStore<User,string>,
                         IUserPasswordStore<User, string>,
                         IUserTwoFactorStore<User, string>
    {

        IUserRepository userRepository = null;

        public Store(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        #region IUserStore

        //unused
        public Task CreateAsync(User user)
        {
            return Task.FromResult(userRepository.InsertUser(user));           
        }

        //unused
        public Task DeleteAsync(User user)
        {
            throw new NotImplementedException();
        }

        //unused
        public void Dispose()
        {
            return;
        }

        //redirect to findByNameAsync
        public Task<User> FindByIdAsync(string userId)
        {
            return FindByNameAsync(userId);
        }

        //unused
        public Task UpdateAsync(User user)
        {
            throw new NotImplementedException();

        }

        public Task<User> FindByNameAsync(string username)
        {
            var user = userRepository.GetUser(username);
            return Task.FromResult(new User()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username
            });
        }

        #endregion

        #region IUserLockoutStore

        public Task<int> GetAccessFailedCountAsync(User user)
        {
            return Task.FromResult(0);
        }

        public Task<bool> GetLockoutEnabledAsync(User user)
        {
            return Task.FromResult(false);
        }

        //unused
        public Task<DateTimeOffset> GetLockoutEndDateAsync(User user)
        {
            throw new NotImplementedException();
        }

        //unused
        public Task<int> IncrementAccessFailedCountAsync(User user)
        {
            throw new NotImplementedException();
        }

        //unused
        public Task ResetAccessFailedCountAsync(User user)
        {
            throw new NotImplementedException();
        }

        //unused
        public Task SetLockoutEnabledAsync(User user, bool enabled)
        {
            throw new NotImplementedException();
        }

        //unused
        public Task SetLockoutEndDateAsync(User user, DateTimeOffset lockoutEnd)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IUserPasswordStore

        public Task SetPasswordHashAsync(User user, string passwordHash)
        {
            return new Task(() => { });
        }

        public Task<string> GetPasswordHashAsync(User user)
        {
            var hashedPassword = userRepository.GetUserHashedPassword(user.Username);
            return Task.FromResult(Convert.ToBase64String(
                    hashedPassword
                ));
        }

        public Task<bool> HasPasswordAsync(User user)
        {
            return Task.FromResult(true);
        }


        #endregion

        #region IUserTwoFactorStore

        //unused
        public Task SetTwoFactorEnabledAsync(User user, bool enabled)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetTwoFactorEnabledAsync(User user)
        {
            return Task.FromResult(false);
        }

        #endregion
    }
}
