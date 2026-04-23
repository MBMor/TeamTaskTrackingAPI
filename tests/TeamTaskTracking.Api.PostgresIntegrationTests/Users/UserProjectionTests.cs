using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeamTaskTracking.Api.PostgresIntegrationTests.Infrastructure;
using TeamTaskTracking.Infrastructure.Persistence;

namespace TeamTaskTracking.Api.PostgresIntegrationTests.Users;

public sealed class UserProjectionTests
{
    [Fact]
    public async Task GetAllUsers_ShouldReturnEmailsCorrectly()
    {
        await using var factory = new CustomPostgresWebApplicationFactory();
        await factory.InitializeAsync();
        using var client = factory.CreateClient();

        await PostgresDatabaseHelper.MigrateAsync(factory.Services);

        await client.PostAsJsonAsync("/api/auth/register", new
        {
            email = "admin@example.com",
            firstName = "Admin",
            lastName = "User",
            password = "StrongPassword123!"
        });

        // promote to admin
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var user = await db.Users.SingleAsync();
            user.ChangeRole(Domain.Users.UserRole.Admin);
            await db.SaveChangesAsync();

            var tokenService = scope.ServiceProvider.GetRequiredService<Application.Auth.ITokenService>();
            var (token, _) = tokenService.CreateAccessToken(user);

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await client.GetAsync("/api/admin/users");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        json.Should().Contain("admin@example.com");
    }
}