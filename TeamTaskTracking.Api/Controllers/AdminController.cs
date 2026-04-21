using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamTaskTracking.Application.Auth;
using TeamTaskTracking.Application.Users;

namespace TeamTaskTracking.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
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
    [ProducesResponseType(typeof(IReadOnlyCollection<UserDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyCollection<UserDetailsDto>>> GetUsers(
    CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllAsync(cancellationToken);
        return Ok(users);
    }
}

