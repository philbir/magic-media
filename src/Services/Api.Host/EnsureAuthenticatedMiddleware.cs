using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

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
            await _next(context);
        }
    }
}
