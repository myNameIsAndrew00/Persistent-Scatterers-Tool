using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Core
{
    public static class Helper
    {
        public static byte[] GenerateRandomBytes(int bytesCount)
        {
           if (bytesCount == 0) return null;

           byte[] result = new byte[bytesCount];

            using (var randomProvider = new RNGCryptoServiceProvider())
                randomProvider.GetNonZeroBytes(result);

            return result;
        }

        public static byte[] HashData(byte[] Data, byte[] Salt)
        {
            if (Data?.Length == 0) return null;

            byte[] hashInput = new byte[Data.Length + Salt?.Length ?? 0];
            byte[] digest = null;

            //concatenate data and salt
            for (int index = 0; index < Data.Length; index++)
                hashInput[index] = Data[index];
            for (int index = 0; index < Salt?.Length; index++)
                hashInput[index + Data.Length] = Salt[index];

            using (SHA256 sha256Instance = SHA256.Create())
            {
                sha256Instance.ComputeHash(hashInput, 0, Data.Length + Salt.Length);
                digest = sha256Instance.Hash;             
            }

            return digest;
        }
    }
}
