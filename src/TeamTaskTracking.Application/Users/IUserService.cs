namespace TeamTaskTracking.Application.Users;

public interface IUserService
{
    Task<UserDto> RegisterAsync(RegisterUserCommand command, CancellationToken cancellation);
    Task<UserDetailsDto?> GetByIdAsync(Guid id,  CancellationToken cancellation);
    Task<IReadOnlyCollection<UserDetailsDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<bool> ChangeRoleAsync(Guid id, ChangeUserRoleCommand command, CancellationToken cancellationToken);
}
