using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TeamTaskTracking.Api.PostgresIntegrationTests.Infrastructure;

namespace TeamTaskTracking.Api.PostgresIntegrationTests.Email;

public sealed class EmailQueryTests
{
    [Fact]
    public async Task Login_ShouldFindUserByEmail()
    {
        await using var factory = await PostgresTestHost.CreateInitializedAsync();
        using var client = factory.CreateClient();

        await PostgresDatabaseHelper.MigrateAsync(factory.Services);

        await client.PostAsJsonAsync("/api/auth/register", new
        {
            email = "user@example.com",
            firstName = "A",
            lastName = "B",
            password = "StrongPassword123!"
        });

        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "user@example.com",
            password = "StrongPassword123!"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}