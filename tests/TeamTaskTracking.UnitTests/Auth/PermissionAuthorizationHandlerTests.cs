using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using TeamTaskTracking.Application.Auth;
using TeamTaskTracking.Application.Auth.Requirements;
using TeamTaskTracking.Infrastructure.Auth;

namespace TeamTaskTracking.UnitTests.Auth;

public sealed class PermissionAuthorizationHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenUserHasRequiredPermission_ShouldSucceed()
    {
        var requirement = new PermissionRequirement(Permissions.UsersReadAll);
        var context = CreateContext(requirement, Permissions.UsersReadAll);

        await new PermissionAuthorizationHandler().HandleAsync(context);

        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_WhenUserDoesNotHaveRequiredPermission_ShouldNotSucceed()
    {
        var requirement = new PermissionRequirement(Permissions.UsersManageRoles);
        var context = CreateContext(requirement, Permissions.UsersReadSelf);

        await new PermissionAuthorizationHandler().HandleAsync(context);

        context.HasSucceeded.Should().BeFalse();
    }

    private static AuthorizationHandlerContext CreateContext(
        PermissionRequirement requirement,
        string permission)
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim("permission", permission)
        ], "TestAuth"));

        return new AuthorizationHandlerContext([requirement], user, resource: null);
    }
}