namespace TeamTaskTracking.Application.Auth;

public sealed record LoginResultDto(
    string AccessToken,
    DateTime ExpiresAtUtc,
    string TokenType);
