using MapWebSite.Core;
using MapWebSite.Core.Database;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MapWebSite.Authentication
{
    public class Store : IUserStore<User, string>,
                         IUserLockoutStore<User,string>,
                         IUserPasswordStore<User, string>,
                         IUserTwoFactorStore<User, string>  ,
                         IUserRoleStore<User,string>,
                         IUserSecurityStampStore<User,string>,
                         IUserEmailStore<User,string>
    {
        /*cached data used to not hit the database foreach request*/
        User user = null;
        IList<string> userRoles = null;

        /*repository used to acces user data*/
        IUserRepository userRepository = null;

        public Store(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        #region IUserStore

        public Task CreateAsync(User user)
        {
            if (!userRepository.InsertUser(user)) throw new Exception("Failed to insert the user");
            return Task.FromResult(0);          
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


        public Task UpdateAsync(User user)
        {
            userRepository.UpdateUser(user);

            return Task.FromResult(0);
        }

        public Task<User> FindByNameAsync(string username)
        {
            /*If the user is not set, it means that it was an anonymous authentication*/
            if (string.IsNullOrEmpty(username))
            {
                this.user = (User)AnonymousUser.Get;
                return Task.FromResult(this.user);
            }

            return this.findUser(username, false);
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
            if (string.IsNullOrEmpty(passwordHash))
                throw new ArgumentException("Password hash can not be null or empty");

            user.PasswordHash = Convert.FromBase64String(passwordHash);

            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(User user)
        {
            var hashedPassword = userRepository.GetUserHashedPassword(user.Username);

            if (hashedPassword == null) return Task.FromResult<string>(null);

            return Task.FromResult(Convert.ToBase64String(
                    hashedPassword
                ));
        }

        public Task<bool> HasPasswordAsync(User user)
        {

            return Task.FromResult(user.PasswordHash != null);
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

        #region IUserRoleStore

        //unused
        public Task AddToRoleAsync(User user, string roleName)
        {
            throw new NotImplementedException();
        }

        //unused
        public Task RemoveFromRoleAsync(User user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<IList<string>> GetRolesAsync(User user)
        {          
            if (user.Username == AnonymousUser.Get.Username) return Task.FromResult(AnonymousUser.Roles);

            if (this.userRoles != null) return Task.FromResult(userRoles);

            this.userRoles = new List<string>();             
            var userRolesData = userRepository.GetUserRoles(user.Username);

            foreach (var role in userRolesData)
                this.userRoles.Add(role.GetEnumString());

            return Task.FromResult(this.userRoles);
        }

        public Task<bool> IsInRoleAsync(User user, string roleName)
        {
            if (this.userRoles == null)
                GetRolesAsync(user);

            return Task.FromResult(this.userRoles.Contains(roleName));            
        }

        #endregion

        #region ISecurityStampStore

        public Task SetSecurityStampAsync(User user, string stamp)
        {
            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        public Task<string> GetSecurityStampAsync(User user)
        {
            return Task.FromResult(user.SecurityStamp);
        }

        #endregion

        #region IUserEmailStore

        public Task<string> GetEmailAsync(User user)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(User user)
        {
            return Task.FromResult(user.ConfirmedEmail);
        }

        public Task SetEmailAsync(User user, string email)
        {
            user.Email = email;
            this.userRepository.SetEmail(user.Username, email);

            return Task.FromResult(0);
        }

        public Task SetEmailConfirmedAsync(User user, bool confirmed)
        {
            this.userRepository.SetEmailConfirmed(user.Username, confirmed);

            return Task.FromResult(0);
        }

        public Task<User> FindByEmailAsync(string email)
        {
            return this.findUser(email, true);
        }

        #endregion

        #region Private methods

        private Task<User> findUser(string username, bool emailSearch)
        {
            /*If the user was already intialised, do not create it again*/
            if (this.user != null) return Task.FromResult(this.user);

            var userModel = emailSearch ? userRepository.GetUserByEmail(username) : userRepository.GetUser(username);

            /*If the user do not exist, return null*/
            if (userModel == null) return Task.FromResult<User>(null);

            this.user = new User()
            {
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                Username = userModel.Username,
                SecurityStamp = userModel.SecurityStamp,
                ConfirmedEmail = userModel.ConfirmedEmail,
                Email = userModel.Email
            };

            return Task.FromResult(this.user);
        }

        #endregion
    }
}
