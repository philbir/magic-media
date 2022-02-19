using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;

namespace MagicMedia.Api;

public class EnsureAuthenticatedMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _env;

    public EnsureAuthenticatedMiddleware(
        RequestDelegate next,
        IWebHostEnvironment env)
    {
        _next = next;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (_env.IsDevelopment()
            || context.Request.Path.StartsWithSegments("/api")
            || context.Request.Path.StartsWithSegments("/graphql")
            || context.Request.Path.StartsWithSegments("/error"))
        {
            await _next(context);
        }
        else if (!context.User.Identity.IsAuthenticated)
        {
            await context.ChallengeAsync();
        }
        else
        {
            Activity.Current?.AddTag("user.name", context.User.Identity.Name);

            await _next(context);
        }
    }
}
