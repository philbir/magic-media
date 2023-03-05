using System.Diagnostics;

namespace MagicMedia.Bff;

public static class DevelopmentRouteBuilder
{
    public static IEndpointRouteBuilder MapDevelopmentHandler(
        this IEndpointRouteBuilder endpoints)
    {
        if (Debugger.IsAttached)
        {
            endpoints.MapGet("/", context =>
            {
                if (context.User?.Identity.IsAuthenticated is not true)
                {
                    context.Response.Redirect("/bff/login");
                }
                else
                {
                    context.Response.Redirect("http://localhost:8080");
                }

                return Task.CompletedTask;
            });
        }

        return endpoints;
    }
}
