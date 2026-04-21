using Microsoft.AspNetCore.Authorization;

namespace TeamTaskTracking.Application.Auth.Requirements;

public sealed class AdminOrSelfRequirement : IAuthorizationRequirement

{
}
