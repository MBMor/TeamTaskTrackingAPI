using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using TeamTaskTracking.Application.Auth.Requirements;
using TeamTaskTracking.Domain.Users;

namespace TeamTaskTracking.Infrastructure.Auth;

public sealed class AdminOrSelfAuthorizationHandler : AuthorizationHandler<AdminOrSelfRequirement, Guid>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AdminOrSelfRequirement requirement,
        Guid resource)
    {
        var role = context.User.FindFirstValue(ClaimTypes.Role);
        if (string.Equals(role, UserRole.Admin.ToString(), StringComparison.Ordinal))
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
