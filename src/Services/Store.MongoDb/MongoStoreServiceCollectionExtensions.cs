using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Extensions.Context;

namespace MagicMedia.Store.MongoDb
{
    public static class MongoStoreServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoDbStore(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            MongoOptions options = configuration.GetSection("MagicMedia:Database")
                .Get<MongoOptions>();

            services.AddSingleton(new MediaStoreContext(options));
            services.AddSingleton<IThumbnailBlobStore>((c) =>
            {
                MediaStoreContext mongoCtx = c.GetService<MediaStoreContext>();
                return new GridFsThumbnailStore(mongoCtx.CreateGridFsBucket());
            });

            services.AddSingleton<IMediaStore, MongoMediaStore>();
            services.AddSingleton<IFaceStore, FaceStore>();
            services.AddSingleton<ICameraStore, CameraStore>();
            services.AddSingleton<IPersonStore, PersonStore>();
            services.AddSingleton<IGroupStore, GroupStore>();
            services.AddSingleton<IAlbumStore, AlbumStore>();

            return services;
        }

    }
}
