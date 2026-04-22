using System;
using System.Collections.Generic;
using System.Text;

namespace TeamTaskTracking.Application.Auth;

public static class Permissions
{
    public const string UsersReadSelf = "users.read.self";
    public const string UsersReadAll = "users.read.all";
    public const string UsersManageRoles = "users.manage.roles";
}
