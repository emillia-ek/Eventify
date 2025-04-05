using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eventify.Enums;
using System.Data;
using Eventify.Utils;

namespace Eventify.Models.Accounts
{
    public abstract class User
    {
        public Guid Id { get; private set; }
        public string Username { get; set; }
        public string HashedPassword { get; set; }
        public Role Role { get; protected set; }
        public DateTime CreatedAt { get; private set; }

        protected User(string username, string password)
        {
            Id = Guid.NewGuid();
            Username = username;
            HashedPassword = PasswordHasher.Hash(password);
            CreatedAt = DateTime.UtcNow;
        }

        public abstract bool HasPermission(Permission permission);
    }
}