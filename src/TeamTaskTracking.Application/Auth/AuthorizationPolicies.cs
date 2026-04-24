namespace TeamTaskTracking.Application.Auth;

public static class AuthorizationPolicies
{
    public const string AdminOnly = "AdminOnly";
    public const string AdminOrSelf = "AdminOrSelf";

    public const string UsersReadAll = "UsersReadAll";
    public const string UsersManageRoles = "UserManageRoles";

}
