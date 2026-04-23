using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using TeamTaskTracking.Api.IntegrationTests.Infrastructure;

namespace TeamTaskTracking.Api.IntegrationTests;

public sealed class RefreshTokenTests
{
    [Fact]
    public async Task Refresh_WithValidRefreshToken_ShouldReturnNewTokens()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var auth = new TestAuthFixture(client);

        await auth.RegisterAsync("refresh@example.com", "Refresh", "User");

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "refresh@example.com",
            password = "StrongPassword123!"
        });

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using var loginStream = await loginResponse.Content.ReadAsStreamAsync();
        using var loginJson = await JsonDocument.ParseAsync(loginStream);

        var refreshToken = loginJson.RootElement.GetProperty("refreshToken").GetString();
        refreshToken.Should().NotBeNullOrWhiteSpace();

        var refreshResponse = await client.PostAsJsonAsync("/api/auth/refresh", new
        {
            refreshToken
        });

        refreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using var refreshStream = await refreshResponse.Content.ReadAsStreamAsync();
        using var refreshJson = await JsonDocument.ParseAsync(refreshStream);

        var newAccessToken = refreshJson.RootElement.GetProperty("accessToken").GetString();
        var newRefreshToken = refreshJson.RootElement.GetProperty("refreshToken").GetString();

        newAccessToken.Should().NotBeNullOrWhiteSpace();
        newRefreshToken.Should().NotBeNullOrWhiteSpace();
        newRefreshToken.Should().NotBe(refreshToken);
    }
}