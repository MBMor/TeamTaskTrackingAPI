using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using TeamTaskTracking.Application.Auth;
using TeamTaskTracking.Application.Auth.Requirements;
using TeamTaskTracking.Infrastructure.Projects;

namespace TeamTaskTracking.Infrastructure.Auth;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddApplicationAuthorization(this IServiceCollection services)
    {

        services.AddAuthorizationBuilder()
            .AddPolicy(AuthorizationPolicies.AdminOnly, policy =>
                policy.RequireRole("Admin"))
            .AddPolicy(AuthorizationPolicies.AdminOrSelf, policy =>
                policy.RequireAuthenticatedUser()
                    .AddRequirements(new AdminOrSelfRequirement()))
            .AddPolicy(AuthorizationPolicies.UsersReadAll, policy =>
                policy.RequireAuthenticatedUser()
                    .AddRequirements(new PermissionRequirement(Permissions.UsersReadAll)))
            .AddPolicy(AuthorizationPolicies.UsersManageRoles, policy =>
                policy.RequireAuthenticatedUser()
                    .AddRequirements(new PermissionRequirement(Permissions.UsersManageRoles)));

        services.AddScoped<IAuthorizationHandler, AdminOrSelfAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, ProjectAuthorizationHandler>();

        return services;
    }
}
