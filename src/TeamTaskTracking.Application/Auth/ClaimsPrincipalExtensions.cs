using System.Security.Claims;

namespace TeamTaskTracking.Application.Auth;

public static class ClaimsPrincipalExtensions
{
    public static bool HasPermission(this ClaimsPrincipal user, string permission)
    {
        return user.HasClaim(claim =>
            string.Equals(claim.Type, CustomClaims.Permission, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(claim.Value, permission, StringComparison.OrdinalIgnoreCase));
    }
}