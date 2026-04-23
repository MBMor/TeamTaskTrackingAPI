using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeamTaskTracking.Infrastructure.Persistence;

namespace TeamTaskTracking.Api.PostgresIntegrationTests.Infrastructure;

public static class PostgresDatabaseHelper
{
    public static async Task MigrateAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await db.Database.MigrateAsync();
    }
}