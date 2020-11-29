using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace MagicMedia.Thumbnail
{
    public static class ThumbnailServiceCollectionExtensions
    {
        public static IServiceCollection AddThumbnailService(this IServiceCollection services)
        {
            services.AddSingleton<IThumbnailService, ThumbnailService>();

            var defs = new List<ThumbnailSizeDefinition>
            {
                new ThumbnailSizeDefinition
                {
                    Name = ThumbnailSizeName.Xs,
                    Width = 40
                },
                new ThumbnailSizeDefinition
                {
                    Name = ThumbnailSizeName.S,
                    Width = 120
                },
                new ThumbnailSizeDefinition
                {
                    Name = ThumbnailSizeName.M,
                    Width = 240
                },
                new ThumbnailSizeDefinition
                {
                    Name = ThumbnailSizeName.L,
                    Width = 320
                },
                new ThumbnailSizeDefinition
                {
                    Name = ThumbnailSizeName.SqS,
                    Width = 120,
                    IsSquare = true
                },
                new ThumbnailSizeDefinition
                {
                    Name = ThumbnailSizeName.SqXs,
                    Width = 40,
                    IsSquare = true
                }
            };

            services.AddSingleton((IEnumerable<ThumbnailSizeDefinition>) defs);

            return services;
        }
    }
}
