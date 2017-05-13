using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace mPower.Framework.Utils
{
    public static class SecurityUtil
    {
        private const string PermissibleCharacters = "23456789QWERTYUPASDFGHJKLZXCVBNM!_";
        private static readonly Random Random = new Random();
        private static readonly object SyncLock = new object();

        public static string GetMD5Hash(string password)
        {
            if (string.IsNullOrEmpty(password))
                return null;

            var md5 = new MD5CryptoServiceProvider();
            byte[] originalBytes = Encoding.Default.GetBytes(password);
            byte[] encodedBytes = md5.ComputeHash(originalBytes);

            var hashString = new StringBuilder();
            foreach (byte b in encodedBytes)
                hashString.Append(b.ToString("X2"));

            return hashString.ToString().ToLower();
        }

        public static string GetUniqueToken()
        {
            var guid = Guid.NewGuid().ToString();

            return GetMD5Hash(guid);
        }

        /// <summary>
        /// Returns randomly generated password
        /// </summary>
        /// <param name="length">desireble lengh of password</param>
        /// <returns>Randomly generated password</returns>
        public static string GenerateRandomPassword(int length)
        {
            var password = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                int randomSymbolPosition = GetNextRandomNumber(PermissibleCharacters.Length);

                char randomCharacter = PermissibleCharacters[randomSymbolPosition];
                password.Append(randomCharacter);
            }
            return password.ToString();
        }

        /// <summary>
        /// Returns new randome number
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private static int GetNextRandomNumber(int length)
        {
            lock (SyncLock)
            {
                return Random.Next(length);
            }
        }
    }
}
