using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MagicMedia.GoogleMaps;

public static class GoogleMapsServiceCollectionExtensions
{
    public static IMagicMediaServerBuilder AddGoogleMaps(
        this IMagicMediaServerBuilder builder,
        GoogleMapsOptions options)
    {
        builder.Services.AddSingleton(options);
        builder.Services.AddSingleton<IGeoDecoderService, GoogleMapsGeoDecoderService>();

        return builder;
    }

    public static IMagicMediaServerBuilder AddGoogleMaps(
        this IMagicMediaServerBuilder builder)
    {
        GoogleMapsOptions options = builder.Configuration
            .GetSection("MagicMedia:GoogleMaps")
            .Get<GoogleMapsOptions>();

        builder.AddGoogleMaps(options);

        return builder;
    }
}

