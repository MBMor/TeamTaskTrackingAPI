using FluentAssertions;
using TeamTaskTracking.Application.Auth;
using TeamTaskTracking.Domain.Users;
using TeamTaskTracking.Infrastructure.Auth;

namespace TeamTaskTracking.UnitTests.Auth;

public sealed class PermissionProviderTests
{
    [Fact]
    public void GetPermissions_ForAdmin_ShouldReturnAllAdminPermissions()
    {
        var permissions = PermissionProvider.GetPermissions(UserRole.Admin);

        permissions.Should().Contain(Permissions.AdminAccess);
        permissions.Should().Contain(Permissions.UsersReadSelf);
        permissions.Should().Contain(Permissions.UsersReadAll);
        permissions.Should().Contain(Permissions.UsersManageRoles);
    }

    [Fact]
    public void GetPermissions_ForRegularUser_ShouldReturnOnlySelfReadPermission()
    {
        var permissions = PermissionProvider.GetPermissions(UserRole.User);

        permissions.Should().BeEquivalentTo([Permissions.UsersReadSelf]);
    }
}
