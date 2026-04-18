using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TeamTaskTracking.Application.Projects;
using TeamTaskTracking.Application.Users;
using TeamTaskTracking.Domain.Users;
using TeamTaskTracking.Infrastructure.Persistence;
using TeamTaskTracking.Infrastructure.Projects;
using TeamTaskTracking.Infrastructure.Users;

namespace TeamTaskTracking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IUserService, UserService>();

        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        return services;

    }
}
