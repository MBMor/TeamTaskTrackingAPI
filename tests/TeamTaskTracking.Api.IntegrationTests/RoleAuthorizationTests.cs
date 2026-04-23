using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeamTaskTracking.Api.IntegrationTests.Infrastructure;
using TeamTaskTracking.Infrastructure.Persistence;

namespace TeamTaskTracking.Api.IntegrationTests;

public sealed class RoleAuthorizationTests
{
    [Fact]
    public async Task ChangeUserRole_ForRegularUser_ShouldReturnForbidden()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var auth = new TestAuthFixture(client);

        var regularUserToken = await auth.RegisterAndLoginAsync(
            "regular@example.com",
            "Regular",
            "User");

        await auth.RegisterAsync(
            "target@example.com",
            "Target",
            "User");

        Guid targetUserId;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var users = await db.Users.AsNoTracking().ToListAsync();
            targetUserId = users.Single(x => x.Email.Value == "target@example.com").Id;
        }

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", regularUserToken);

        var response = await client.PutAsJsonAsync($"/api/admin/users/{targetUserId}/role", new
        {
            role = "Admin"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}