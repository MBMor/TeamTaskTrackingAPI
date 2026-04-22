using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeamTaskTracking.Domain.Users;
using TeamTaskTracking.Infrastructure.Persistence;

namespace TeamTaskTracking.Api.IntegrationTests.Infrastructure;

public sealed class TestDataSeeder
{
    private readonly IServiceProvider _services;

    public TestDataSeeder(IServiceProvider services)
    {
        _services = services;
    }

    public async Task PromoteUserToAdminAsync(string email)
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var users = await db.Users.ToListAsync();
        var user = users.Single(x => x.Email.Value == email);

        user.ChangeRole(UserRole.Admin);

        await db.SaveChangesAsync();
    }
}