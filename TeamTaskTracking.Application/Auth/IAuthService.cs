namespace TeamTaskTracking.Application.Auth;

public interface IAuthService
{
    Task<AuthTokensDto> LoginAsync(LoginCommand command, CancellationToken cancellationToken);
    Task<AuthTokensDto> RefreshAsync(RefreshTokenCommand command, CancellationToken cancellationToken);
    Task LogoutAsync(LogoutCommand command, CancellationToken cancellation);
}
