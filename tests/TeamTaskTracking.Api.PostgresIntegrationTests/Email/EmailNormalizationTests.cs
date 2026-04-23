using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TeamTaskTracking.Api.PostgresIntegrationTests.Infrastructure;

namespace TeamTaskTracking.Api.PostgresIntegrationTests.Email;

public sealed class EmailNormalizationTests
{
    [Fact]
    public async Task Register_WithTrimmedEmail_ShouldWork()
    {
        await using var factory = await PostgresTestHost.CreateInitializedAsync();
        using var client = factory.CreateClient();

        await PostgresDatabaseHelper.MigrateAsync(factory.Services);

        var response = await client.PostAsJsonAsync("/api/auth/register", new
        {
            email = "  admin@example.com  ",
            firstName = "A",
            lastName = "B",
            password = "StrongPassword123!"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var login = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "admin@example.com",
            password = "StrongPassword123!"
        });

        login.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}