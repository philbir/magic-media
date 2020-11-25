using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MagicMedia.BingMaps
{
    public static class BingMapsServiceCollectionExtensions
    {
        public static IMagicMediaServerBuilder AddBingMaps(
            this IMagicMediaServerBuilder builder,
            BingMapsOptions options)
        {
            builder.Services.AddSingleton(options);
            builder.Services.AddSingleton<IGeoDecoderService, BingMapsGeoDecoderService>();

            return builder;
        }

        public static IMagicMediaServerBuilder AddBingMaps(
            this IMagicMediaServerBuilder builder)
        {
            BingMapsOptions options = builder.Configuration
                .GetSection("MagicMedia:BingMaps")
                .Get<BingMapsOptions>();

            builder.AddBingMaps(options);

            return builder;
        }
    }
}
