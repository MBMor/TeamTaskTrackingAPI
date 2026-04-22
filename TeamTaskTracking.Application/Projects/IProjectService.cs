using TeamTaskTracking.Domain.Projects;

namespace TeamTaskTracking.Application.Projects;

public interface IProjectService
{
    Task<IReadOnlyCollection<ProjectDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<ProjectDto>> GetAllForUserAsync(Guid userId, bool isAdmin, CancellationToken cancellationToken);
    Task<ProjectDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Project?> GetProjectForAuthorizationAsync(Guid id, CancellationToken cancellationToken);
    Task<ProjectDto> CreateAsync(CreateProjectCommand command, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Guid id, UpdateProjectCommand command, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
