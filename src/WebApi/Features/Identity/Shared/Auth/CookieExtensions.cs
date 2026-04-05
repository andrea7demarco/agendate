namespace WebApi.Features.Identity.Shared.Auth;

public static class CookieExtensions
{
    private const string RefreshTokenCookieName = "refreshToken";
    private const string RefreshTokenPath = "/auth/refresh";

    public static void AppendRefreshTokenCookie(
        this HttpResponse response,
        string refreshToken,
        DateTime expiresAtUtc
    )
    {
        response.Cookies.Append(
            RefreshTokenCookieName,
            refreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = expiresAtUtc,
                Path = RefreshTokenPath,
            }
        );
    }

    public static void DeleteRefreshTokenCookie(this HttpResponse response)
    {
        response.Cookies.Delete(
            RefreshTokenCookieName,
            new CookieOptions
            {
                Path = RefreshTokenPath,
                Secure = true,
                SameSite = SameSiteMode.Strict,
            }
        );
    }
}
