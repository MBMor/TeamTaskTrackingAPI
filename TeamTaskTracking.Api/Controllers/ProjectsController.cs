using Microsoft.AspNetCore.Mvc;
using TeamTaskTracking.Application.Projects;

namespace TeamTaskTracking.Api.Controllers;

[ApiController]
[Route("api/projects")]
public sealed class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<ProjectDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ProjectDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _projectService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _projectService.GetByIdAsync(id, cancellationToken);

        if (result is null) 
            return NotFound();

        return Ok(result);

    }

    [HttpPost]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProjectDto>> Create(
        [FromBody] CreateProjectCommand request, 
        CancellationToken cancellationToken)
    {
        var result = await _projectService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetById), 
            new { id = result.Id }, 
            result);

    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id, 
        [FromBody] UpdateProjectCommand request, 
        CancellationToken cancellationToken)
    {
        var isUpdated = await _projectService.UpdateAsync(id, request, cancellationToken);

        if (!isUpdated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var isDeleted = await _projectService.DeleteAsync(id, cancellationToken);

        if (!isDeleted)
            return NotFound();

        return NoContent();
    }

}
