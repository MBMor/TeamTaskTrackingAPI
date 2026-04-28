using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using TeamTaskTracking.Application.Auth;
using TeamTaskTracking.Application.Auth.Requirements;
using TeamTaskTracking.Infrastructure.Auth;

namespace TeamTaskTracking.UnitTests.Auth;

public sealed class AdminOrSelfAuthorizationHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenUserHasAdminAccessPermission_ShouldSucceed()
    {
        var resourceUserId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid();

        var context = CreateContext(
            resourceUserId,
            CreateUser(currentUserId, Permissions.AdminAccess));

        await new AdminOrSelfAuthorizationHandler().HandleAsync(context);

        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_WhenUserIsSelf_ShouldSucceed()
    {
        var userId = Guid.NewGuid();

        var context = CreateContext(
            userId,
            CreateUser(userId));

        await new AdminOrSelfAuthorizationHandler().HandleAsync(context);

        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_WhenUserIsNotAdminAndNotSelf_ShouldNotSucceed()
    {
        var context = CreateContext(
            Guid.NewGuid(),
            CreateUser(Guid.NewGuid()));

        await new AdminOrSelfAuthorizationHandler().HandleAsync(context);

        context.HasSucceeded.Should().BeFalse();
    }

    private static AuthorizationHandlerContext CreateContext(
        Guid resourceUserId,
        ClaimsPrincipal user)
    {
        var requirement = new AdminOrSelfRequirement();

        return new AuthorizationHandlerContext(
            [requirement],
            user,
            resourceUserId);
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