using Microsoft.AspNetCore.Authorization;

namespace TeamTaskTracking.Application.Auth.Requirements;

public sealed class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }

}
