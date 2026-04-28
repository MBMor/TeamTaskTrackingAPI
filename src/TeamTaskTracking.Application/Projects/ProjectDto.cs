namespace TeamTaskTracking.Application.Projects;

public sealed record ProjectDto
(
    Guid Id,
    Guid OwnerUserId,
    string Name,
    string? Description,
    DateTime CreateAtUtc
);
