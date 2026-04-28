using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Data.Common;
using TeamTaskTracking.Infrastructure.Persistence;

namespace TeamTaskTracking.Api.IntegrationTests.Infrastructure;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private DbConnection? _connection;

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
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
            services.RemoveAll<DbConnection>();
            services.RemoveAll<AppDbContext>();

            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            services.AddSingleton<DbConnection>(_ => _connection);

            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                var connection = sp.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
            });

            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _connection?.Dispose();
        }
    }
}