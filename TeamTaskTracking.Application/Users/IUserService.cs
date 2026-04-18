namespace TeamTaskTracking.Application.Users;

public interface IUserService
{
    Task<UserDto> RegisterAsync(RegisterUserCommand command, CancellationToken cancellation);
}
