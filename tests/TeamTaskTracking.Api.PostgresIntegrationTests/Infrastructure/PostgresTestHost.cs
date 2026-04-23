namespace TeamTaskTracking.Api.PostgresIntegrationTests.Infrastructure;

public static class PostgresTestHost
{
    public static async Task<CustomPostgresWebApplicationFactory> CreateInitializedAsync()
    {
        var factory = new CustomPostgresWebApplicationFactory();
        await factory.InitializeAsync();
        return factory;
    }
}