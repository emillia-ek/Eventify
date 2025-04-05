using System;
using System.Security.Cryptography;
using System.Text;

namespace Eventify.Utils
{
    public static class PasswordHasher
    {
        public static string Hash(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }

        public static bool Verify(string password, string hashedPassword)
        {
            return Hash(password) == hashedPassword;
        }
    }
}