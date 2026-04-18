using Microsoft.AspNetCore.Mvc;
using TeamTaskTracking.Api.Contracts.Auth;
using TeamTaskTracking.Application.Users;

namespace TeamTaskTracking.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDto>> Register(
        RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(
            request.Email,
            request.FirstName,
            request.LastName,
            request.Password);

        var result = await _userService.RegisterAsync(command, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, result);
    }

}
