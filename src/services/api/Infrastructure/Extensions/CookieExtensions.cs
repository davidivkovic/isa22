namespace API.Infrastructure.Extensions;

public static class CookieExtensions
{
    private static readonly string _refreshTokenCookieName = "refresh-token";

    public static string RefreshToken(this IRequestCookieCollection cookies) 
    {
        return cookies[_refreshTokenCookieName];
    }

    public static void DeleteRefreshToken(this IResponseCookies cookies)
    {
        cookies.Delete(_refreshTokenCookieName);
    }

    public static void AppendRefreshToken(this IResponseCookies cookies, string refreshToken)
    {
        cookies.Append(_refreshTokenCookieName, refreshToken, new CookieOptions
        {
            IsEssential = true,
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Unspecified
        });
    }
}
