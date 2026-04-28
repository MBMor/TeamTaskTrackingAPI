using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using TeamTaskTracking.Application.Auth;
using TeamTaskTracking.Application.Projects;
using TeamTaskTracking.Domain.Projects;
using TeamTaskTracking.Infrastructure.Projects;

namespace TeamTaskTracking.UnitTests.Projects;

public sealed class ProjectAuthorizationHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenUserHasAdminAccessPermission_ShouldSucceed()
    {
        var project = new Project(Guid.NewGuid(), "Test project", null);
        var user = CreateUser(Guid.NewGuid(), Permissions.AdminAccess);

        var context = CreateContext(ProjectOperations.Read, user, project);

        await new ProjectAuthorizationHandler().HandleAsync(context);

        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_WhenUserIsOwner_ShouldSucceed()
    {
        var ownerUserId = Guid.NewGuid();
        var project = new Project(ownerUserId, "Test project", null);
        var user = CreateUser(ownerUserId);

        var context = CreateContext(ProjectOperations.Read, user, project);

        await new ProjectAuthorizationHandler().HandleAsync(context);

        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_WhenUserIsNotOwnerAndNotAdmin_ShouldNotSucceed()
    {
        var project = new Project(Guid.NewGuid(), "Test project", null);
        var user = CreateUser(Guid.NewGuid());

        var context = CreateContext(ProjectOperations.Read, user, project);

        await new ProjectAuthorizationHandler().HandleAsync(context);

        context.HasSucceeded.Should().BeFalse();
    }

    private static AuthorizationHandlerContext CreateContext(
        IAuthorizationRequirement requirement,
        ClaimsPrincipal user,
        Project project)
    {
        return new AuthorizationHandlerContext([requirement], user, project);
    }

    private static ClaimsPrincipal CreateUser(Guid userId, string? permission = null)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString())
        };

        if (permission is not null)
        {
            claims.Add(new Claim("permission", permission));
        }

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
    }
}