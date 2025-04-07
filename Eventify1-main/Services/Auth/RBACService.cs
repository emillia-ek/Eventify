using Eventify.Models.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Services.Auth
{
    public static class RBACService
    {
        public static bool HasPermission(User user, string requiredPermission)
        {
            switch (user.Role)
            {
                case "Admin":
                    return true; // Admin ma wszystkie uprawnienia

                case "Manager":
                    return requiredPermission == "ManageEvents" ||
                           requiredPermission == "ViewReports";

                case "User":
                    return requiredPermission == "ViewEvents" ||
                           requiredPermission == "MakeReservations";

                default:
                    return false;
            }
        }
    }
}
