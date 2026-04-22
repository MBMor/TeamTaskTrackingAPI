using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;
using TeamTaskTracking.Api.IntegrationTests.Infrastructure;

namespace TeamTaskTracking.Api.IntegrationTests;

public sealed class ProjectAuthorizationTests
{
    [Fact]
    public async Task User_ShouldNotAccess_ForeignProject()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var auth = new TestAuthFixture(client);
        var projects = new TestProjectFixture(client);

        var ownerToken = await auth.RegisterAndLoginAsync(
            "owner@example.com",
            "Owner",
            "User");

        var projectId = await projects.CreateAsync(
            ownerToken,
            "Owner Project",
            "Secret");

        var otherUserToken = await auth.RegisterAndLoginAsync(
            "other@example.com",
            "Other",
            "User");

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", otherUserToken);

        var response = await client.GetAsync($"/api/projects/{projectId}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}