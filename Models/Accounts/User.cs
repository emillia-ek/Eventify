using Eventify.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Models.Accounts
{
    public abstract class User
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }

        protected User(string username, string passwordHash, string role, string email)
        {
            Username = username;
            PasswordHash = passwordHash;
            Role = role;
            Email = email;
        }

    }
}
