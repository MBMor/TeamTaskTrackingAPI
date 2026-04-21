using System;
using System.Collections.Generic;
using System.Text;
using TeamTaskTracking.Application.Auth;
using TeamTaskTracking.Domain.Users;

namespace TeamTaskTracking.Infrastructure.Auth;

public static class PermissionProvider
{
    public static IReadOnlyCollection<string> GetPermissions(UserRole role) =>
        role switch
        {
            UserRole.Admin =>
            [
                Permissions.UsersReadSelf,
                Permissions.UsersReadAll,
                Permissions.UsersManageRoles
            ],
            _ => 
            [
                Permissions.UsersReadSelf
            ]

        };
}
