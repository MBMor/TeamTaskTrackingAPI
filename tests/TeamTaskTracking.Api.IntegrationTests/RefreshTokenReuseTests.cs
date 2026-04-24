using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;
using TeamTaskTracking.Api.IntegrationTests.Infrastructure;
using TeamTaskTracking.Infrastructure.Persistence;

namespace TeamTaskTracking.Api.IntegrationTests;

public sealed class RefreshTokenReuseTests
{
    [Fact]
    public async Task Refresh_ReusingOldRefreshToken_ShouldRevokeTokenFamily()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        await client.PostAsJsonAsync("/api/auth/register", new
        {
            email = "reuse@example.com",
            firstName = "Reuse",
            lastName = "User",
            password = "StrongPassword123!"
        });

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "reuse@example.com",
            password = "StrongPassword123!"
        });

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginTokens = await loginResponse.Content.ReadFromJsonAsync<AuthTokensResponse>();
        loginTokens.Should().NotBeNull();

        var firstRefreshResponse = await client.PostAsJsonAsync("/api/auth/refresh", new
        {
            refreshToken = loginTokens!.RefreshToken
        });

        firstRefreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var firstRefreshTokens = await firstRefreshResponse.Content.ReadFromJsonAsync<AuthTokensResponse>();
        firstRefreshTokens.Should().NotBeNull();

        var reuseResponse = await client.PostAsJsonAsync("/api/auth/refresh", new
        {
            refreshToken = loginTokens.RefreshToken
        });

        reuseResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var familyTokenResponse = await client.PostAsJsonAsync("/api/auth/refresh", new
        {
            refreshToken = firstRefreshTokens!.RefreshToken
        });

        familyTokenResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Refresh_UsingCurrentRefreshTokenOnce_ShouldSucceed()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        await client.PostAsJsonAsync("/api/auth/register", new
        {
            email = "valid-refresh@example.com",
            firstName = "Valid",
            lastName = "Refresh",
            password = "StrongPassword123!"
        });

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "valid-refresh@example.com",
            password = "StrongPassword123!"
        });

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginTokens = await loginResponse.Content.ReadFromJsonAsync<AuthTokensResponse>();
        loginTokens.Should().NotBeNull();

        var refreshResponse = await client.PostAsJsonAsync("/api/auth/refresh", new
        {
            refreshToken = loginTokens!.RefreshToken
        });

        refreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var refreshTokens = await refreshResponse.Content.ReadFromJsonAsync<AuthTokensResponse>();
        refreshTokens.Should().NotBeNull();
        refreshTokens!.RefreshToken.Should().NotBe(loginTokens.RefreshToken);
        refreshTokens.AccessToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Refresh_ReusingOldRefreshToken_ShouldMarkTokenFamilyAsCompromised()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        await client.PostAsJsonAsync("/api/auth/register", new
        {
            email = "reuse-db@example.com",
            firstName = "Reuse",
            lastName = "Db",
            password = "StrongPassword123!"
        });

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "reuse-db@example.com",
            password = "StrongPassword123!"
        });

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginTokens = await loginResponse.Content.ReadFromJsonAsync<AuthTokensResponse>();
        loginTokens.Should().NotBeNull();

        var firstRefreshResponse = await client.PostAsJsonAsync("/api/auth/refresh", new
        {
            refreshToken = loginTokens!.RefreshToken
        });

        firstRefreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var reuseResponse = await client.PostAsJsonAsync("/api/auth/refresh", new
        {
            refreshToken = loginTokens.RefreshToken
        });

        reuseResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var refreshTokens = await db.RefreshTokens.ToListAsync();

        refreshTokens.Should().NotBeEmpty();
        refreshTokens.Should().AllSatisfy(x =>
        {
            x.IsCompromised.Should().BeTrue();
            x.RevokedAtUtc.Should().NotBeNull();
        });
    }
}