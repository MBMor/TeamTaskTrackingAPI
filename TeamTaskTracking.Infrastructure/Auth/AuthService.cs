using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TeamTaskTracking.Application.Auth;
using TeamTaskTracking.Application.Common.Exceptions;
using TeamTaskTracking.Domain.Users;
using TeamTaskTracking.Infrastructure.Persistence;

namespace TeamTaskTracking.Infrastructure.Auth;

public sealed class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<LoginCommand> _loginValidator;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(
        AppDbContext dbContext, 
        IValidator<LoginCommand> loginValidator, 
        IPasswordHasher<User> passwordHasher, 
        ITokenService tokenService)
    {
        _dbContext = dbContext;
        _loginValidator = loginValidator;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<LoginResultDto> LoginAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        await _loginValidator.ValidateAndThrowAsync(command, cancellationToken);

        Email email;

        try
        {
            email = Email.Create(command.Email);
        }
        catch (ArgumentException)
        {
            throw new InvalidCredentialsException();
        }

        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Email == email, cancellationToken);

        if (user is null)
            throw new InvalidCredentialsException();

        var verificationResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            command.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
            throw new InvalidCredentialsException();

        return _tokenService.CreateAccessToken(user);
    }
}
