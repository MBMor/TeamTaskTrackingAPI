using Microsoft.EntityFrameworkCore;
using FluentValidation;
using TeamTaskTracking.Application.Projects;
using TeamTaskTracking.Domain.Projects;
using TeamTaskTracking.Infrastructure.Persistence;

namespace TeamTaskTracking.Infrastructure.Projects;

internal class ProjectService : IProjectService
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<CreateProjectCommand> _createValidator;
    private readonly IValidator<UpdateProjectCommand> _updateValidator;

    public ProjectService(
        AppDbContext dbContext, 
        IValidator<CreateProjectCommand> createValidator, 
        IValidator<UpdateProjectCommand> updateValidator)
    {
        _dbContext = dbContext;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IReadOnlyCollection<ProjectDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Projects
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new ProjectDto(
                x.Id,
                x.OwnerUserId,
                x.Name,
                x.Description,
                x.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ProjectDto>> GetAllForUserAsync(
    Guid userId,
    bool isAdmin,
    CancellationToken cancellationToken)
    {
        var query = _dbContext.Projects.AsNoTracking();

        if (!isAdmin)
        {
            query = query.Where(x => x.OwnerUserId == userId); 
        }

        return await query
            .OrderBy(x => x.Name)
            .Select(x => new ProjectDto(
                x.Id,
                x.OwnerUserId,
                x.Name,
                x.Description,
                x.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<ProjectDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Projects
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new ProjectDto(
                x.Id,
                x.OwnerUserId,
                x.Name,
                x.Description,
                x.CreatedAtUtc))
            .SingleOrDefaultAsync(cancellationToken);


    }

    public async Task<Project?> GetProjectForAuthorizationAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Projects.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
    public async Task<ProjectDto> CreateAsync(CreateProjectCommand command, CancellationToken cancellationToken)
    {
        await _createValidator.ValidateAndThrowAsync(command, cancellationToken);

        var project = new Project(
            command.OwnerUserId ,
            command.Name, 
            command.Description);

        _dbContext.Projects.Add(project);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ProjectDto(
            project.Id,
            project.OwnerUserId,
            project.Name,
            project.Description,
            project.CreatedAtUtc);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateProjectCommand command, CancellationToken cancellationToken)
    {
        await _updateValidator.ValidateAndThrowAsync(command, cancellationToken);

        var project = await _dbContext.Projects.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (project is null)
            return false;
        

        project.UpdateDetails(command.Name, command.Description);

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var project = await _dbContext.Projects.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (project is null)
            return false;

        _dbContext.Projects.Remove(project);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

}
