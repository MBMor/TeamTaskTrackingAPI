using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TeamTaskTracking.Application.Users;
using TeamTaskTracking.Domain.Users;
using TeamTaskTracking.Infrastructure.Persistence;

namespace TeamTaskTracking.Infrastructure.Users;

public sealed class UserReadStore : IUserReadStore
{
    private readonly AppDbContext _dbContext;

    public UserReadStore(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .AnyAsync(x => x.Email == email, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Email == email, cancellationToken);
    }

    public async Task<UserDetailsDto?> GetUserDetailsByIdAsync(Guid id, CancellationToken cancellationToken)
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

    public async Task<IReadOnlyCollection<UserDetailsDto>> GetAllUserDetailsAsync(CancellationToken cancellationToken)
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
