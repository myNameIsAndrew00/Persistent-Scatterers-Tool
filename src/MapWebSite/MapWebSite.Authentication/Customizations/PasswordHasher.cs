﻿using MapWebSite.Core;
using MapWebSite.Core.Database;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapWebSite.Authentication
{
    internal class PasswordHasher : Microsoft.AspNet.Identity.PasswordHasher
    {

        public override string HashPassword(string password)
        {
            byte[] passwordSalt = Helper.GenerateRandomBytes(32);

            var hashedPassword = Helper.HashData(Encoding.UTF8.GetBytes(password), passwordSalt).Concatenate(passwordSalt);

            return Convert.ToBase64String(hashedPassword);
        }

        public override PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            //anonymous authentication must be passed
            if (hashedPassword == null && providedPassword == null)
                return PasswordVerificationResult.Success;

            var hashedPasswordBytes = Convert.FromBase64String(hashedPassword);

            byte[] passwordHash = new byte[32];
            byte[] passwordSalt = new byte[32];

            Array.Copy(hashedPasswordBytes, 0, passwordHash, 0, 32);
            Array.Copy(hashedPasswordBytes, 32, passwordSalt, 0, 32);

            var providedHash = Helper.HashData(Encoding.UTF8.GetBytes(providedPassword), passwordSalt);

            
            if (providedHash.SequenceEqual(passwordHash))
                return PasswordVerificationResult.Success;
            else
                return PasswordVerificationResult.Failed;
        }
    }
}
