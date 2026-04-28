using TeamTaskTracking.Domain.Users;

namespace TeamTaskTracking.Application.Users;

public sealed record ChangeUserRoleCommand(UserRole Role);
