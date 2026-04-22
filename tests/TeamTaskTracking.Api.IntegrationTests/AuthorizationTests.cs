using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;
using TeamTaskTracking.Api.IntegrationTests.Infrastructure;

namespace TeamTaskTracking.Api.IntegrationTests;

public sealed class AuthorizationTests
{
    [Fact]
    public async Task AdminUsers_ForRegularUser_ShouldReturnForbidden()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var auth = new TestAuthFixture(client);

        var token = await auth.RegisterAndLoginAsync(
            "user@example.com",
            "Regular",
            "User");

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/api/admin/users");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AdminUsers_ForAdmin_ShouldReturnOk()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var auth = new TestAuthFixture(client);
        var seeder = new TestDataSeeder(factory.Services);

        const string email = "admin@example.com";

        await auth.RegisterAsync(email, "Admin", "User");
        await seeder.PromoteUserToAdminAsync(email);

        var adminToken = await auth.LoginAsync(email);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", adminToken);

        var response = await client.GetAsync("/api/admin/users");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}