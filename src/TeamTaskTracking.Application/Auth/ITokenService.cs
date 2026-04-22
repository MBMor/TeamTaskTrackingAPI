using TeamTaskTracking.Domain.Users;

namespace TeamTaskTracking.Application.Auth;

public interface ITokenService
{
    (string Token, DateTime ExpiresAtUtc) CreateAccessToken(User user);
    (string PlainTextToken, string TokenHash, DateTime ExpiresAtUtc) CreateRefreshToken();
}
