using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eventify.Enums;

namespace Eventify.Models.Accounts
{
    public class RegularUser : User
    {
        public RegularUser(string username, string password) : base(username, password)
        {
            Role = Role.User;
        }

        public override bool HasPermission(Permission permission)
        {
            if (permission == Permission.ViewEvents ||
                permission == Permission.MakeReservations)
            {
                return true;
            }
            return false;
        }
    }
}