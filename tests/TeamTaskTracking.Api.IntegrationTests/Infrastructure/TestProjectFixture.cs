using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace TeamTaskTracking.Api.IntegrationTests.Infrastructure;

public sealed class TestProjectFixture
{
    private readonly HttpClient _client;

    public TestProjectFixture(HttpClient client)
    {
        _client = client;
    }

    public async Task<Guid> CreateAsync(
        string accessToken,
        string name = "Test Project",
        string? description = "Test Description")
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _client.PostAsJsonAsync("/api/projects", new
        {
            name,
            description
        });

        if (response.StatusCode != HttpStatusCode.Created)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(
                $"Project creation failed. Status: {response.StatusCode}. Body: {body}");
        }

        using var stream = await response.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(stream);

        return document.RootElement.GetProperty("id").GetGuid();
    }
}