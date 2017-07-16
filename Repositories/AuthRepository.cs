using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using DataModels;
using Repositories.Interfaces;
using Shared.Attributes;
using Shared.Dto;

namespace Repositories
{
    [Singleton]
    public class AuthRepository : IAuthRepository
    {        
        private const int SALT_LENGTH = 16;

        private static string GetSalt()
        {
            byte[] bytes = new byte[SALT_LENGTH];
            using (var keyGenerator = RandomNumberGenerator.Create())
            {
                keyGenerator.GetBytes(bytes);
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        private static string GetSaltFromStoredPassword(string password)
        {
            return password.Substring(password.Length - SALT_LENGTH * 2);
        }

        public string GeneratePassword(string plainPassword, string salt = null)
        {
            using (var sha256 = SHA256.Create())
            {
                if (salt == null)
                    salt = GetSalt();

                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(plainPassword + salt));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower() + salt;
            }
        }

        public bool IsPasswordValid(User user, string password)
        {
            var testPassword = GeneratePassword(password, GetSaltFromStoredPassword(user.Password));
            return testPassword == user.Password;
        }                

    }
}
