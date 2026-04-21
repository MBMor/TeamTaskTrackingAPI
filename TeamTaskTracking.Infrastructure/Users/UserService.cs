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

    public UserService(
        AppDbContext dbContext, 
        IValidator<RegisterUserCommand> 
        registerValidator, IPasswordHasher<User> passwordHasher)
    {
        _dbContext = dbContext;
        _registerValidator = registerValidator;
        _passwordHasher = passwordHasher;
    }




    public async Task<UserDto> RegisterAsync(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        await _registerValidator.ValidateAndThrowAsync(command, cancellationToken);

        var normalizedEmail = Email.Create(command.Email);

        var emailExists = await _dbContext.Users
            .AsNoTracking()
            .AnyAsync(x => x.Email == normalizedEmail, cancellationToken);

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
            user.Email,
            user.FirstName,
            user.LastName,
            user.CreatedAtUtc);
    }

    public async Task<UserDetailsDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new UserDetailsDto(
                x.Id,
                x.Email.Value,
                x.FirstName,
                x.LastName,
                x.Role,
                x.CreatedAtUtc))
            .SingleOrDefaultAsync(cancellationToken);
    }
    public async Task<IReadOnlyCollection<UserDetailsDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Select(x => new UserDetailsDto(
                x.Id,
                x.Email.Value,
                x.FirstName,
                x.LastName,
                x.Role,
                x.CreatedAtUtc))
            .ToListAsync(cancellationToken);   
    }

}
