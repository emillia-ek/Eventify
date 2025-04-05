using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eventify.Enums;

namespace Eventify.Models.Accounts
{
    public class Manager : User
    {
        public Manager(string username, string password) : base(username, password)
        {
            Role = Role.Manager;
        }

        public override bool HasPermission(Permission permission)
        {
            if (permission == Permission.ViewEvents ||
                permission == Permission.MakeReservations ||
                permission == Permission.ManageEvents ||
                permission == Permission.ViewReports)
            {
                return true;
            }
            return false;
        }
    }
}