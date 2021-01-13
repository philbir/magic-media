using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Extensions.Context;

namespace MagicMedia.Store.MongoDb
{
    public static class MongoStoreServiceCollectionExtensions
    {
        public static IMagicMediaServerBuilder AddMongoDbStore(
            this IMagicMediaServerBuilder builder)
        {
            builder.Services.AddMongoDbStore(builder.Configuration);

            return builder;
        }

        public static IServiceCollection AddMongoDbStore(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            MongoOptions options = configuration.GetSection("MagicMedia:Database")
                .Get<MongoOptions>();

            services.AddSingleton(new MediaStoreContext(options));
            services.AddSingleton<IThumbnailBlobStore>((c) =>
            {
                MediaStoreContext mongoCtx = c.GetRequiredService<MediaStoreContext>();
                return new GridFsThumbnailStore(mongoCtx.CreateGridFsBucket());
            });

            services.AddSingleton<IMediaStore, MongoMediaStore>();
            services.AddSingleton<IFaceStore, FaceStore>();
            services.AddSingleton<ICameraStore, CameraStore>();
            services.AddSingleton<IUserStore, UserStore>();
            services.AddSingleton<IPersonStore, PersonStore>();
            services.AddSingleton<IGroupStore, GroupStore>();
            services.AddSingleton<IAlbumStore, AlbumStore>();
            services.AddSingleton<IMediaAIStore, MediaAIStore>();
            services.AddSingleton<IAuditEventStore, AuditEventStore>();
            services.AddSingleton<IClientThumbprintStore, ClientThumbprintStore>();
            services.AddSingleton<ISimilarMediaStore, SimilarMediaStore>();

            return services;
        }
    }
}
