using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TeamTaskTracking.Application.Common.Exceptions;
using TeamTaskTracking.Application.Users;
using TeamTaskTracking.Domain.Users;
using TeamTaskTracking.Infrastructure.Persistence;

namespace TeamTaskTracking.Infrastructure.Users;

public sealed class UserService : IUserService
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<RegisterUserCommand> _registerValidator;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IValidator<ChangeUserRoleCommand> _changeRoleValidator;

    public UserService(
        AppDbContext dbContext,
        IValidator<RegisterUserCommand> registerValidator, 
        IPasswordHasher<User> passwordHasher, 
        IValidator<ChangeUserRoleCommand> changeRoleValidator)
    {
        _dbContext = dbContext;
        _registerValidator = registerValidator;
        _passwordHasher = passwordHasher;
        _changeRoleValidator = changeRoleValidator;
    }




    public async Task<UserDto> RegisterAsync(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        await _registerValidator.ValidateAndThrowAsync(command, cancellationToken);

        var normalizedEmail = Email.Create(command.Email);

        var existingEmails = await _dbContext.Users
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var emailExists = existingEmails.Any(x => x.Email.Value == normalizedEmail.Value);

        if (emailExists)
            throw new DuplicateEmailException(command.Email);

        var user = new User(
            normalizedEmail,
            command.FirstName,
            command.LastName);

        var passwordHash = _passwordHasher.HashPassword(user, command.Password);
        user.SetPasswordHash(passwordHash);

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new UserDto(
            user.Id,
            user.Email.Value,
            user.FirstName,
            user.LastName,
            user.CreatedAtUtc);
    }

    public async Task<UserDetailsDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (user is null)
            return null;

        return new UserDetailsDto(
            user.Id,
            user.Email.Value,
            user.FirstName,
            user.LastName,
            user.Role,
            user.CreatedAtUtc);
    }
    public async Task<IReadOnlyCollection<UserDetailsDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var users = await _dbContext.Users
            .AsNoTracking()
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .ToListAsync(cancellationToken);

        return users
            .Select(x => new UserDetailsDto(
                x.Id,
                x.Email.Value,
                x.FirstName,
                x.LastName,
                x.Role,
                x.CreatedAtUtc))
            .ToList();
    }

    public async Task<bool> ChangeRoleAsync(Guid id, ChangeUserRoleCommand command, CancellationToken cancellationToken)
    {
        await _changeRoleValidator.ValidateAndThrowAsync(command, cancellationToken);

        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (user is null)
            return false;

        user.ChangeRole(command.Role);

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
