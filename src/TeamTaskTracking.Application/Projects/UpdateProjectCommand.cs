namespace TeamTaskTracking.Application.Projects;

public sealed record UpdateProjectCommand(string Name, string? Description);
