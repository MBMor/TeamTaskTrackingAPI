namespace TeamTaskTracking.Api.Contracts.Auth;

public sealed record RegisterUserRequest(
    string Email,
    string FirstName,
    string LastName,
    string Password);
