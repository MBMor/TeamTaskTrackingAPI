using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeamTaskTracking.Api.PostgresIntegrationTests.Infrastructure;
using TeamTaskTracking.Domain.Users;
using TeamTaskTracking.Infrastructure.Persistence;
using DomainEmail = TeamTaskTracking.Domain.Users.Email;

namespace TeamTaskTracking.Api.PostgresIntegrationTests.Email;

public sealed class EmailUniquenessTests
{
    [Fact]
    public async Task Register_WithDuplicateEmail_ShouldReturnConflict()
    {
        await using var factory = await PostgresTestHost.CreateInitializedAsync();
        using var client = factory.CreateClient();

        await PostgresDatabaseHelper.MigrateAsync(factory.Services);

        await client.PostAsJsonAsync("/api/auth/register", new
        {
            email = "admin@example.com",
            firstName = "A",
            lastName = "B",
            password = "StrongPassword123!"
        });

        var response = await client.PostAsJsonAsync("/api/auth/register", new
        {
            email = "admin@example.com",
            firstName = "A",
            lastName = "B",
            password = "StrongPassword123!"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Register_WithSameEmailDifferentCase_ShouldReturnConflict()
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

        var response = await client.PostAsJsonAsync("/api/auth/register", new
        {
            email = "admin@example.com",
            firstName = "A",
            lastName = "B",
            password = "StrongPassword123!"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Database_ShouldEnforceUniqueEmailConstraint()
    {
        await using var factory = await PostgresTestHost.CreateInitializedAsync();
        using var client = factory.CreateClient();

        await PostgresDatabaseHelper.MigrateAsync(factory.Services);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        var email = DomainEmail.Create("test@example.com");

        db.Users.Add(new User(email, "A", "B"));
        await db.SaveChangesAsync();

        db.Users.Add(new User(email, "C", "D"));

        Func<Task> act = async () => await db.SaveChangesAsync();

        await act.Should().ThrowAsync<DbUpdateException>();
    }
}