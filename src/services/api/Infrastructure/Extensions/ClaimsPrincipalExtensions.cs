using System.Security.Claims;

namespace API.Infrastructure.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid Id(this ClaimsPrincipal claims)
    {
        string userId = claims.FindFirstValue(ClaimTypes.NameIdentifier);
        return userId is null ? Guid.Empty : Guid.Parse(userId);
    }

    public static string Role(this ClaimsPrincipal claims)
    {
        return claims.FindFirstValue(ClaimTypes.Role);
    }
}
