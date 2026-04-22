using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace TeamTaskTracking.Api.IntegrationTests.Infrastructure;

public sealed class TestAuthFixture
{
    private readonly HttpClient _client;

    public TestAuthFixture(HttpClient client)
    {
        _client = client;
    }

    public async Task RegisterAsync(
        string email,
        string firstName = "Test",
        string lastName = "User",
        string password = "StrongPassword123!")
    {
        var response = await _client.PostAsJsonAsync("/api/auth/register", new
        {
            email,
            firstName,
            lastName,
            password
        });

        if (response.StatusCode != HttpStatusCode.Created)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(
                $"Register failed. Status: {(int)response.StatusCode} {response.StatusCode}. Body: {body}");
        }
    }

    public async Task<string> LoginAsync(
        string email,
        string password = "StrongPassword123!")
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email,
            password
        });

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(
                $"Login failed. Status: {(int)response.StatusCode} {response.StatusCode}. Body: {body}");
        }

        using var stream = await response.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(stream);

        return document.RootElement.GetProperty("accessToken").GetString()
            ?? throw new InvalidOperationException("Access token was not returned.");
    }

    public async Task<string> RegisterAndLoginAsync(
        string email,
        string firstName = "Test",
        string lastName = "User",
        string password = "StrongPassword123!")
    {
        await RegisterAsync(email, firstName, lastName, password);
        return await LoginAsync(email, password);
    }
}