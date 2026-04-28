using Microsoft.AspNetCore.Authorization;
using TeamTaskTracking.Application.Auth;
using TeamTaskTracking.Application.Auth.Requirements;

namespace TeamTaskTracking.Infrastructure.Auth;

public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var hasPermission = context.User.Claims.Any(c =>
        string.Equals(c.Type, CustomClaims.Permission, StringComparison.OrdinalIgnoreCase) &&
        string.Equals(c.Value, requirement.Permission, StringComparison.OrdinalIgnoreCase));

        if (hasPermission)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;

    }
}

