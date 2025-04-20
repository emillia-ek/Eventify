using Eventify.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Models.Accounts
{
    public class Manager : User
    {
        public Manager(string username, string passwordHash, string email)
            : base(username, passwordHash, "Manager", email) { }

        
    }
}