namespace TeamTaskTracking.Application.Users;

public sealed record RegisterUserCommand(
    string Email,
    string FirstName,
    string LastName,
    string Password);
