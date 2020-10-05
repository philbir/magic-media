using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace MagicMedia.BingMaps
{
    public class BingMapsOptions
    {
        public string ApiKey { get; set; }
    }


    public static class BingMapsServiceCollectionExtensions
    {
        public static IServiceCollection AddBingMaps(
            this IServiceCollection services,
            BingMapsOptions options)
        {
            services.AddSingleton(options);
            services.AddSingleton<IGeoDecoderService, BingMapsGeoDecoderService>();

            return services;
        }
    }
}
