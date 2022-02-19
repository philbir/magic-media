namespace MagicMedia.Identity;

public static class PreferedIdpCookieExtensions
{
    private static readonly string CookieName = "mm-idp";

    public static void SetPreferedIdp(this HttpContext httpContext, string idp)
    {
        httpContext.Response.Cookies.Append(
            CookieName,
            idp,
            new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(180),
                Secure = true
            });
    }

    public static string? GetPreferedIdp(this HttpContext httpContext)
    {
        return httpContext.Request.Cookies[CookieName];
    }
}
