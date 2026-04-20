using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using TeamTaskTracking.Application.Auth;
using TeamTaskTracking.Application.Common.Exceptions;
using TeamTaskTracking.Domain.Users;
using TeamTaskTracking.Infrastructure.Persistence;

namespace TeamTaskTracking.Infrastructure.Auth;

public sealed class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<LoginCommand> _loginValidator;
    private readonly IValidator<RefreshTokenCommand> _refreshValidator;
    private readonly IValidator<LogoutCommand> _logoutValidator;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(
        AppDbContext dbContext,
        IValidator<LoginCommand> loginValidator,
        IPasswordHasher<User> passwordHasher,
        ITokenService tokenService,
        IValidator<RefreshTokenCommand> refreshValidator,
        IValidator<LogoutCommand> logoutValidator)
    {
        _dbContext = dbContext;
        _loginValidator = loginValidator;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _refreshValidator = refreshValidator;
        _logoutValidator = logoutValidator;
    }

    public async Task<AuthTokensDto> LoginAsync(LoginCommand command, CancellationToken cancellationToken)
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

        var (accessToken, accessTokenExpiresAtUtc) = _tokenService.CreateAccessToken(user);
        var (plainRefreshToken, refreshTokenHash, refreshTokenExpiresAtUtc) = _tokenService.CreateRefreshToken();

        var refreshToken = new RefreshToken(
            user.Id,
            refreshTokenHash,
            refreshTokenExpiresAtUtc);

        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AuthTokensDto(
            accessToken,
            accessTokenExpiresAtUtc,
            plainRefreshToken,
            refreshTokenExpiresAtUtc,
            "Bearer");

    }

    public async Task<AuthTokensDto> RefreshAsync(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        await _refreshValidator.ValidateAndThrowAsync(command, cancellationToken);

        var providedHash = ComputeSha256(command.RefreshToken);

        var existingRefreshToken = await _dbContext.RefreshTokens
            .SingleOrDefaultAsync(x => x.TokenHash == providedHash, cancellationToken);

        if (existingRefreshToken is null || !existingRefreshToken.IsActive)
            throw new InvalidRefreshTokenException();

        var user = await _dbContext.Users
            .SingleOrDefaultAsync(x => x.Id == existingRefreshToken.UserId, cancellationToken);

        if (user is null)
            throw new InvalidRefreshTokenException();

        var (newAccessToken, accessTokenExpiresAtUtc) = _tokenService.CreateAccessToken(user);
        var (newPlainRefreshToken, newRefreshTokenHash, refreshTokenExpiresAtUtc) = _tokenService.CreateRefreshToken();

        existingRefreshToken.Revoke(newRefreshTokenHash);

        var replacementRefreshToken = new RefreshToken(
            user.Id,
            newRefreshTokenHash,
            refreshTokenExpiresAtUtc);

        _dbContext.RefreshTokens.Add(replacementRefreshToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AuthTokensDto(
            newAccessToken,
            accessTokenExpiresAtUtc,
            newPlainRefreshToken,
            refreshTokenExpiresAtUtc,
            "Bearer");
    }

    public async Task LogoutAsync(LogoutCommand command, CancellationToken cancellationToken)
    {
        await _logoutValidator.ValidateAndThrowAsync(command, cancellationToken);

        var providedHash = ComputeSha256(command.RefreshToken);

        var refreshToken = await _dbContext.RefreshTokens
            .SingleOrDefaultAsync(x => x.TokenHash == providedHash, cancellationToken);

        if (refreshToken is null)
            return;

        refreshToken.Revoke();
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static string ComputeSha256(string value)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes);
    }
}

