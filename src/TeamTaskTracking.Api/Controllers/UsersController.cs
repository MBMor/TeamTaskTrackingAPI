using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamTaskTracking.Application.Auth.Requirements;
using TeamTaskTracking.Application.Users;
using TeamTaskTracking.Domain.Users;

namespace TeamTaskTracking.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuthorizationService _authorizationService;

    public UsersController(IUserService userService, IAuthorizationService authorizationService)
    {
        _userService = userService;
        _authorizationService = authorizationService;
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(UserDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserDetailsDto>> Me(CancellationToken cancellationToken)
    {
        var subject = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(subject, out var userId))
            return Unauthorized();

        var user = await _userService.GetByIdAsync(userId, cancellationToken);
        if (user is null)
            return Unauthorized();

        return Ok(user);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDetailsDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var authorizationResult = await _authorizationService.AuthorizeAsync(
            User,
            id,
            new AdminOrSelfRequirement());

        if (!authorizationResult.Succeeded)
            return Forbid();

        var user = await _userService.GetByIdAsync(id, cancellationToken);

        if (user is null)
            return NotFound();

        return Ok(user);    
    }

}