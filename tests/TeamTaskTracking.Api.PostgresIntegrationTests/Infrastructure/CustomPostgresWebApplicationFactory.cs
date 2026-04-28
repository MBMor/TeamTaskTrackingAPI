using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TeamTaskTracking.Infrastructure.Persistence;
using Microsoft.AspNetCore.TestHost;

namespace TeamTaskTracking.Api.PostgresIntegrationTests.Infrastructure;

public sealed class CustomPostgresWebApplicationFactory
    : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer;
    private string? _connectionString;

    public CustomPostgresWebApplicationFactory()
    {
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
        _connectionString = _postgresContainer.GetConnectionString();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.UseSetting("Jwt:Issuer", "TeamTaskTracking.Api.Tests");
        builder.UseSetting("Jwt:Audience", "TeamTaskTracking.Api.Tests.Client");
        builder.UseSetting("Jwt:SigningKey", "this-is-a-test-signing-key-with-32-plus-chars");
        builder.UseSetting("Jwt:AccessTokenExpirationMinutes", "15");
        builder.UseSetting("Jwt:RefreshTokenExpirationDays", "30");

        builder.ConfigureTestServices(services =>
        {
            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                throw new InvalidOperationException(
                    "PostgreSQL test container must be initialized before creating the test host.");
            }

            services.RemoveAll<DbContextOptions<AppDbContext>>();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(_connectionString);
            });
        });
    }

    public new async Task DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}