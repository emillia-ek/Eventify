using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eventify.Enums;

namespace Eventify.Models.Accounts
{
    public class Admin : User
    {
        public Admin(string username, string password) : base(username, password)
        {
            Role = Role.Admin;
        }

        public override bool HasPermission(Permission permission)
        {
            // Admin ma zawsze wszystkie uprawnienia
            return true;
        }
    }
}