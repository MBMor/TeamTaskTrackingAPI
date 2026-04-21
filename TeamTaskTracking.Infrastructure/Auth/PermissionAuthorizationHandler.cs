using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;
using TeamTaskTracking.Application.Auth.Requirements;

namespace TeamTaskTracking.Infrastructure.Auth;

public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        PermissionRequirement requirement)
    {
        var hasPermission = context.User.Claims.Any(c =>
        string.Equals(c.Type, "permission", StringComparison.OrdinalIgnoreCase) &&
        string.Equals(c.Value, requirement.Permission, StringComparison.OrdinalIgnoreCase));

        if (hasPermission)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;

    }
}

