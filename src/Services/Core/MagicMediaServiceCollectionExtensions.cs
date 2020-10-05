using MagicMedia.Face;
using MagicMedia.Thumbnail;
using Microsoft.Extensions.DependencyInjection;

namespace MagicMedia
{
    public static class MagicMediaServiceCollectionExtensions
    {
        public static IServiceCollection AddMagicMedia(this IServiceCollection services)
        {
            services.AddFaceDetection();
            services.AddThumbnailService();
            services.AddSingleton<IMetadataExtractor, MetadataExtractor>();
            services.AddSingleton<IBoxExtractorService, BoxExtractorService>();

            return services;
        }
    }
}
