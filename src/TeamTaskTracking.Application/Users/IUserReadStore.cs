using TeamTaskTracking.Domain.Users;

namespace TeamTaskTracking.Application.Users;

public interface IUserReadStore
{
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken);
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken);
    Task<UserDetailsDto?> GetUserDetailsByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<UserDetailsDto>> GetAllUserDetailsAsync(CancellationToken cancellationToken);
}
