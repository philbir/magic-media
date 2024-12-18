using Microsoft.AspNetCore.HttpOverrides;

namespace MagicMedia.Api;

public static class ForwardHeaderExtensions
{
    public static IApplicationBuilder UseDefaultForwardedHeaders(this IApplicationBuilder app)
    {
        var forwardedHeadersOptions = new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                               ForwardedHeaders.XForwardedProto,
            RequireHeaderSymmetry = false
        };
        forwardedHeadersOptions.KnownNetworks.Clear();
        forwardedHeadersOptions.KnownProxies.Clear();

        app.UseForwardedHeaders(forwardedHeadersOptions);

        return app;
    }
}
