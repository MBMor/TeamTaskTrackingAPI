using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamTaskTracking.Api.Contracts.Users;
using TeamTaskTracking.Application.Auth;
using TeamTaskTracking.Application.Users;
using TeamTaskTracking.Domain.Users;

namespace TeamTaskTracking.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize]
public sealed class AdminController : ControllerBase
{
    private readonly IUserService _userService;

    public AdminController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok("Admin access granted");
    }

    [HttpGet("users")]
    [Authorize(Policy = AuthorizationPolicies.UsersReadAll)]
    [ProducesResponseType(typeof(IReadOnlyCollection<UserDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyCollection<UserDetailsDto>>> GetUsers(
    CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllAsync(cancellationToken);
        return Ok(users);
    }

    [HttpPut("users/{id:guid}/role")]
    [Authorize(Policy = AuthorizationPolicies.UsersManageRoles)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ChangeUserRole(
    Guid id,
    ChangeUserRoleRequest request,
    CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<UserRole>(request.Role, ignoreCase: true, out var role))
        {
            return BadRequest(new
            {
                Title = "Invalid role.",
                Detail = "Role must be one of: User, Admin."
            });
        }

        var command = new ChangeUserRoleCommand(role);

        var changed = await _userService.ChangeRoleAsync(id, command, cancellationToken);
        if (!changed)
            return NotFound();

        return NoContent();
    }
}

