using TeamTaskTracking.Domain.Users;

namespace TeamTaskTracking.Application.Users;

public sealed record UserDetailsDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    UserRole Role,
    DateTime CreatedAtUtc);
