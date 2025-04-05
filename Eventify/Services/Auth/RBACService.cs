using System;
using Eventify.Enums;
using Eventify.Models.Accounts;

namespace Eventify.Services.Auth
{
    public class RBACService
    {
        public bool CheckPermission(User user, Permission requiredPermission)
        {
            return user.HasPermission(requiredPermission);
        }

        public void ValidateRoleAccess(User user, Role requiredRole)
        {
            if (user.Role < requiredRole)
            {
                throw new UnauthorizedAccessException("Insufficient privileges");
            }
        }
    }
}