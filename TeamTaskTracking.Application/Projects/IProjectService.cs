using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamTaskTracking.Application.Projects;

public interface IProjectService
{
    Task<IReadOnlyCollection<ProjectDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<ProjectDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ProjectDto> CreateAsync(CreateProjectCommand command, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Guid id, UpdateProjectCommand command, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
