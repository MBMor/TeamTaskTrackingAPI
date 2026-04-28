using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TeamTaskTracking.Application.Auth;
using TeamTaskTracking.Application.Auth.Requirements;

namespace TeamTaskTracking.Infrastructure.Auth;

public sealed class AdminOrSelfAuthorizationHandler : AuthorizationHandler<AdminOrSelfRequirement, Guid>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AdminOrSelfRequirement requirement,
        Guid resource)
    {
        var isAdmin = context.User.HasPermission(Permissions.AdminAccess);

        if (isAdmin)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var subject = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? context.User.FindFirstValue("sub");

        if (Guid.TryParse(subject, out var currentUserId) && currentUserId == resource)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
