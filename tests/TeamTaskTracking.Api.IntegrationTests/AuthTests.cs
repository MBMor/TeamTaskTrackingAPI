using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TeamTaskTracking.Api.IntegrationTests.Infrastructure;

namespace TeamTaskTracking.Api.IntegrationTests;

public sealed class AuthTests
{
    [Fact]
    public async Task Register_ShouldReturnCreated()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/register", new
        {
            email = "john@example.com",
            firstName = "John",
            lastName = "Doe",
            password = "StrongPassword123!"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ShouldReturnUnauthorized()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var auth = new TestAuthFixture(client);

        await auth.RegisterAsync("john2@example.com");

        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "john2@example.com",
            password = "WrongPassword123!"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Me_WithoutToken_ShouldReturnUnauthorized()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/users/me");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}