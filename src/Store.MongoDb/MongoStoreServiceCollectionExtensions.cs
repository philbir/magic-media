using Microsoft.Extensions.DependencyInjection;
using MongoDB.Extensions.Context;

namespace MagicMedia.Store.MongoDb
{
    public static class MongoStoreServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoDbStore(
            this IServiceCollection services,
            MongoOptions options)
        {
            services.AddSingleton(new MediaStoreContext(options));
            services.AddSingleton<IThumbnailBlobStore>((c) =>
            {
               MediaStoreContext mongoCtx = c.GetService<MediaStoreContext>();
               return new GridFsThumbnailStore(mongoCtx.CreateGridFsBucket());
            });

            services.AddSingleton<IMediaStore, MongoMediaStore>();
            services.AddSingleton<IFaceStore, FaceStore>();

            return services;
        }
    }
}
