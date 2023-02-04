namespace MagicMedia.Api;

public static class HttpContextExtensions
{
    private static readonly string[] ClientIpHeaders =
    {
            "X-Original-Forwarded-For", "X-Real-IP", "X-Forwarded-For"
        };

    public static string GetRemoteIpAddress(this HttpContext httpContext)
    {
        foreach (var header in ClientIpHeaders)
        {
            if (httpContext.Request.Headers.ContainsKey(header))
            {
                return RemovePort(httpContext?.Request.Headers[header]);
            }
        }

        return httpContext.Connection?.RemoteIpAddress?.ToString();
    }

    private static string? RemovePort(string? ipAddress)
    {
        if (ipAddress != null && ipAddress.Contains(
            ":",
            StringComparison.InvariantCultureIgnoreCase))
        {
            return ipAddress.Split(':').First();
        }

        return ipAddress;
    }
}
