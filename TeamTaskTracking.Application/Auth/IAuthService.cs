namespace TeamTaskTracking.Application.Auth;

public interface IAuthService
{
    Task<LoginResultDto> LoginAsync(LoginCommand command, CancellationToken cancellationToken);
}
