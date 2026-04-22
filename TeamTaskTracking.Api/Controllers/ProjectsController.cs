using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TeamTaskTracking.Api.Contracts.Projects;
using TeamTaskTracking.Application.Projects;
using TeamTaskTracking.Domain.Users;
using TeamTaskTracking.Infrastructure.Persistence;


namespace TeamTaskTracking.Api.Controllers;

[ApiController]
[Route("api/projects")]
public sealed class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly IAuthorizationService _authorizationService;

    public ProjectsController(
        IProjectService projectService, 
        IAuthorizationService authorizationService)
    {
        _projectService = projectService;
        _authorizationService = authorizationService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<ProjectDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyCollection<ProjectDto>>> GetAll(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        var role = User.FindFirstValue(ClaimTypes.Role);

        if (!Guid.TryParse(userIdClaim, out var currentUserId))
            return Unauthorized();

        var isAdmin = string.Equals(role, UserRole.Admin.ToString(), StringComparison.Ordinal);

        var result = await _projectService.GetAllForUserAsync(currentUserId, isAdmin, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var project = await _projectService.GetProjectForAuthorizationAsync(id, cancellationToken);
        if (project is null)
            return NotFound();

        var authorizationResult = await _authorizationService.AuthorizeAsync(
            User,
            project,
            ProjectOperations.Read);

        if (!authorizationResult.Succeeded)
            return Forbid();

        var result = await _projectService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ProjectDto>> Create(
        CreateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var subject = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(subject, out var currentUserId))
            return Unauthorized();

        var command = new CreateProjectCommand(
            currentUserId,
            request.Name,
            request.Description);

        var result = await _projectService.CreateAsync(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var project = await _projectService.GetProjectForAuthorizationAsync(id, cancellationToken);
        if (project is null)
            return NotFound();

        var authorizationResult = await _authorizationService.AuthorizeAsync(
            User,
            project,
            ProjectOperations.Update);

        if (!authorizationResult.Succeeded)
            return Forbid();

        var command = new UpdateProjectCommand(
            request.Name,
            request.Description);

        var updated = await _projectService.UpdateAsync(id, command, cancellationToken);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var project = await _projectService.GetProjectForAuthorizationAsync(id, cancellationToken);
        if (project is null)
            return NotFound();

        var authorizationResult = await _authorizationService.AuthorizeAsync(
            User,
            project,
            ProjectOperations.Delete);

        if (!authorizationResult.Succeeded)
            return Forbid();

        var deleted = await _projectService.DeleteAsync(id, cancellationToken);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
