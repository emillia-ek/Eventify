using Eventify.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Models.Accounts
{
    public class RegularUser : User
    {
        public RegularUser(string username, string passwordHash, string email)
            : base(username, passwordHash, "User", email) { }

        
    }
}
