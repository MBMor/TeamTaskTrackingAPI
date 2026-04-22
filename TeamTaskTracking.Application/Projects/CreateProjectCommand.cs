namespace TeamTaskTracking.Application.Projects;

public sealed record CreateProjectCommand(
    Guid OwnerUserId,
    string Name,
    string? Description);


