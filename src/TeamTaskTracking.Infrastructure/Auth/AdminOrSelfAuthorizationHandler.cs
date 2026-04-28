using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TeamTaskTracking.Application.Auth;
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
        var isAdmin = context.User.HasClaim(claim =>
            string.Equals(claim.Type, "permission", StringComparison.OrdinalIgnoreCase) &&
            string.Equals(claim.Value, Permissions.AdminAccess, StringComparison.OrdinalIgnoreCase));

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
