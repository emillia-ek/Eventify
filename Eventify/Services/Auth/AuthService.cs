using Eventify.Models.Accounts;
using Eventify.Utils;
using System;
using System.IO;
using System.Linq;

namespace Eventify.Services.Auth
{
    public class AuthService
    {
        private const string UsersFilePath = "Data/users.txt";

        public User Authenticate(string username, string password)
        {
            try
            {
                if (!File.Exists(UsersFilePath))
                    throw new FileNotFoundException("Users database not found");

                var lines = File.ReadAllLines(UsersFilePath);

                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 3 &&
                        parts[0].IndexOf(username, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if (PasswordHasher.Verify(password, parts[1]))
                        {
                            return CreateUserInstance(username, parts[1], parts[2]);
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                LoggerService.Log($"Authentication error: {ex.Message}");
                throw;
            }
        }

        private User CreateUserInstance(string username, string hashedPassword, string role)
        {
            var lowerRole = role.ToLower();
            if (lowerRole == "admin")
            {
                return new Admin(username, hashedPassword);
            }
            else if (lowerRole == "manager")
            {
                return new Manager(username, hashedPassword);
            }
            else
            {
                return new RegularUser(username, hashedPassword);
            }
        }
    }
}