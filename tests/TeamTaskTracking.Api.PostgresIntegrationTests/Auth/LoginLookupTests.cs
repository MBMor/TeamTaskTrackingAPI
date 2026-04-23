using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TeamTaskTracking.Api.PostgresIntegrationTests.Infrastructure;

namespace TeamTaskTracking.Api.PostgresIntegrationTests.Auth;

public sealed class LoginLookupTests
{
    [Fact]
    public async Task Login_WithDifferentEmailCase_ShouldSucceed()
    {
        await using var factory = await PostgresTestHost.CreateInitializedAsync();
        using var client = factory.CreateClient();

        await PostgresDatabaseHelper.MigrateAsync(factory.Services);

        await client.PostAsJsonAsync("/api/auth/register", new
        {
            email = "Admin@Example.com",
            firstName = "A",
            lastName = "B",
            password = "StrongPassword123!"
        });

        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "admin@example.com",
            password = "StrongPassword123!"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}