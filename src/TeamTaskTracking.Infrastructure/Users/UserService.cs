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
    private readonly IUserReadStore _userReadStore;
    private readonly IValidator<RegisterUserCommand> _registerValidator;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IValidator<ChangeUserRoleCommand> _changeRoleValidator;

    public UserService(
        AppDbContext dbContext,
        IUserReadStore userReadStore,
        IValidator<RegisterUserCommand> registerValidator,
        IPasswordHasher<User> passwordHasher,
        IValidator<ChangeUserRoleCommand> changeRoleValidator)
    {
        _dbContext = dbContext;
        _userReadStore = userReadStore;
        _registerValidator = registerValidator;
        _passwordHasher = passwordHasher;
        _changeRoleValidator = changeRoleValidator;
    }

    public async Task<UserDto> RegisterAsync(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        await _registerValidator.ValidateAndThrowAsync(command, cancellationToken);

        var normalizedEmail = Email.Create(command.Email);

        var emailExists = await _userReadStore.ExistsByEmailAsync(normalizedEmail, cancellationToken);
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

    public Task<UserDetailsDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => _userReadStore.GetUserDetailsByIdAsync(id, cancellationToken);

    public Task<IReadOnlyCollection<UserDetailsDto>> GetAllAsync(CancellationToken cancellationToken)
        => _userReadStore.GetAllUserDetailsAsync(cancellationToken);

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