using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TeamTaskTracking.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        var email = User.FindFirstValue(ClaimTypes.Email)
            ?? User.FindFirstValue("email");

        var firstName = User.FindFirstValue(ClaimTypes.GivenName)
            ?? User.FindFirstValue("given_name");

        var lastName = User.FindFirstValue(ClaimTypes.Surname)
            ?? User.FindFirstValue("family_name");

        var role = User.FindFirstValue(ClaimTypes.Role);

        return Ok(new
        {
            Id = userId,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Role = role
        });
    }
}