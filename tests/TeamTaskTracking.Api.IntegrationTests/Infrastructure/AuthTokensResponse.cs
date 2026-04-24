namespace TeamTaskTracking.Api.IntegrationTests.Infrastructure;

public sealed record AuthTokensResponse(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc,
    string TokenType);