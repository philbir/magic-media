using Duende.Bff;
using Duende.Bff.Yarp;
using Yarp.ReverseProxy.Configuration;

namespace MagicMedia.Bff;

internal static class YarpConfigBuilder
{
    internal static IReverseProxyBuilder LoadFromBffOptions(
        this IReverseProxyBuilder builder,
        BffOptions options)
    {
        var metadata = new Dictionary<string, string>
        {
            { "Duende.Bff.Yarp.TokenType", "User" },
            { "Duende.Bff.Yarp.AntiforgeryCheck", (!options.DisableAntiForgery).ToString().ToLower() }
        };

        builder.LoadFromMemory(
            new[]
            {
                new RouteConfig()
                {
                    RouteId = "api",
                    ClusterId = "cluster1",
                    Match = new() { Path = "/api/{**catch-all}" },
                    Metadata = metadata
                },
                new RouteConfig()
                {
                    RouteId = "graphql",
                    ClusterId = "cluster1",
                    Match = new() { Path = "/graphql/{**catch-all}" },
                    Metadata = metadata
                },
                new RouteConfig()
                {
                    RouteId = "signalr",
                    ClusterId = "cluster1",
                    Match = new() { Path = "/signalr/{**catch-all}" },
                    Metadata = metadata
                }
            },
            new[]
            {
                new ClusterConfig
                {
                    ClusterId = "cluster1",
                    Destinations =
                        new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
                        {
                            { "destination1", new() { Address = options.ApiUrl } },
                        },
                    HttpClient = new HttpClientConfig()
                    {
                        DangerousAcceptAnyServerCertificate = options.SkipCertificateValidation
                    }
                }
            });

        return builder;
    }
}
