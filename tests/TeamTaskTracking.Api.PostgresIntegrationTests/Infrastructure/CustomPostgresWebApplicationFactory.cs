using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;

namespace TeamTaskTracking.Api.PostgresIntegrationTests.Infrastructure;

public sealed class CustomPostgresWebApplicationFactory
    : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer;

    public CustomPostgresWebApplicationFactory()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Testing");

        Environment.SetEnvironmentVariable("Jwt__Issuer", "TeamTaskTracking.Api.Tests");
        Environment.SetEnvironmentVariable("Jwt__Audience", "TeamTaskTracking.Api.Tests.Client");
        Environment.SetEnvironmentVariable("Jwt__SigningKey", "this-is-a-test-signing-key-with-32-plus-chars");
        Environment.SetEnvironmentVariable("Jwt__AccessTokenExpirationMinutes", "60");
        Environment.SetEnvironmentVariable("Jwt__RefreshTokenExpirationDays", "30");

        _postgresContainer = new PostgreSqlBuilder("postgres:16.2")
            .WithDatabase("teamtasktracking_tests")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithCleanUp(true)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();

        Environment.SetEnvironmentVariable(
            "ConnectionStrings__DefaultConnection",
            _postgresContainer.GetConnectionString());
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
    }

    public new async Task DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();

        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null);
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", null);
        Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", null);

        Environment.SetEnvironmentVariable("Jwt__Issuer", null);
        Environment.SetEnvironmentVariable("Jwt__Audience", null);
        Environment.SetEnvironmentVariable("Jwt__SigningKey", null);
        Environment.SetEnvironmentVariable("Jwt__AccessTokenExpirationMinutes", null);
        Environment.SetEnvironmentVariable("Jwt__RefreshTokenExpirationDays", null);

        await base.DisposeAsync();
    }
}