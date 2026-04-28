using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Security.Claims;
using TeamTaskTracking.Application.Auth;
using TeamTaskTracking.Application.Projects;
using TeamTaskTracking.Domain.Projects;
using TeamTaskTracking.Domain.Users;

namespace TeamTaskTracking.Infrastructure.Projects;

public sealed class ProjectAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Project>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        Project resource)
    {
        var isAdmin = context.User.HasPermission(Permissions.AdminAccess);

        if (isAdmin)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var subject = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? context.User.FindFirstValue("sub");

        if (!Guid.TryParse(subject, out var currentUserId))
            return Task.CompletedTask;

        var isOwner = currentUserId == resource.OwnerUserId;

        if (isOwner &&
            (requirement.Name == ProjectOperations.Read.Name ||
             requirement.Name == ProjectOperations.Update.Name ||
             requirement.Name == ProjectOperations.Delete.Name))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
