using MagicMedia.Face;
using MagicMedia.Metadata;
using MagicMedia.Operations;
using MagicMedia.Processing;
using MagicMedia.Security;
using MagicMedia.Thumbnail;
using MagicMedia.Video;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MagicMedia
{
    public static class MagicMediaServiceCollectionExtensions
    {
        public static IMagicMediaServerBuilder AddCoreMediaServices(
            this IMagicMediaServerBuilder builder)
        {
            builder.Services.AddCoreMediaServices(builder.Configuration);

            return builder;
        }

        public static IServiceCollection AddCoreMediaServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddFaceDetection(configuration);
            services.AddThumbnailService();
            services.AddSingleton<IMetadataExtractor, MetadataExtractor>();
            services.AddSingleton<IBoxExtractorService, BoxExtractorService>();
            services.AddSingleton<IImageTransformService, ImageTransformService>();
            services.AddSingleton<IWebPImageConverter, DefaultWebPImageConverter>();
            services.AddSingleton<IDateTakenParser, DateTakenParser>();
            services.AddSingleton<ICameraService, CameraService>();
            services.AddSingleton<IPersonService, PersonService>();
            services.AddSingleton<IPersonTimelineService, PersonTimelineService>();
            services.AddSingleton<IGroupService, GroupService>();
            services.AddSingleton<IFaceService, FaceService>();

            services.AddSingleton<ISearchFacetService, SearchFacetService>();
            services.AddSingleton<IFolderTreeService, FolderTreeService>();
            services.AddSingleton<IAgeOperationsService, AgeOperationsService>();
            services.AddSingleton<IAlbumService, AlbumService>();
            services.AddSingleton<IAlbumSummaryService, AlbumSummaryService>();

            services.AddSingleton<IMediaOperationsService, MediaOperationsService>();
            services.AddSingleton<IMoveMediaHandler, MoveMediaHandler>();
            services.AddSingleton<IRecycleMediaHandler, RecycleMediaHandler>();
            services.AddSingleton<IDeleteMediaHandler, DeleteMediaHandler>();
            services.AddSingleton<IUpdateMediaMetadataHandler, UpdateMediaMetadataHandler>();

            services.AddSingleton<IMediaSearchService, MediaSearchService>();
            services.AddSingleton<IMediaService, MediaService>();
            services.AddSingleton<IVideoPlayerService, VideoPlayerService>();
            services.AddSingleton<IVideoProcessingService, VideoProcessingService>();
            services.AddSingleton<IMediaAIService, MediaAIService>();
            services.AddFFmpeg(configuration);

            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IAlbumMediaIdResolver, AlbumMediaIdResolver>();
            services.AddSingleton<IUserAuthorizationService, UserAuthorizationService>();

            return services;
        }

        private static IServiceCollection AddFFmpeg(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            FFmpegOption? ffmpegOptions = configuration.GetSection("MagicMedia:FFMpeg")
                .Get<FFmpegOption>() ?? new FFmpegOption
                {
                    AutoDownload = true
                };

            services.AddSingleton<IFFmpegInitializer>(c => new FFmpegInitializer(ffmpegOptions));

            return services;
        }

        public static IMagicMediaServerBuilder AddProcessingMediaServices(this IMagicMediaServerBuilder builder)
        {
            builder.Services.AddSingleton<IMediaProcessorTask, AutoOrientTask>();
            builder.Services.AddSingleton<IMediaProcessorTask, ExtractMetadataTask>();
            builder.Services.AddSingleton<IMediaProcessorTask, GenerateThumbnailsTask>();
            builder.Services.AddSingleton<IMediaProcessorTask, BuildFaceDataTask>();
            builder.Services.AddSingleton<IMediaProcessorTask, GenerateWebImageTask>();
            builder.Services.AddSingleton<IMediaProcessorTask, SaveMediaTask>();
            builder.Services.AddSingleton<IMediaProcessorTask, PredictPersonsTask>();
            builder.Services.AddSingleton<IMediaProcessorTask, SaveFaceDataAsync>();
            builder.Services.AddSingleton<IMediaProcessorTask, ExtractVideoDataTask>();
            builder.Services.AddSingleton<IMediaProcessorTask, CleanUpSourceTask>();
            builder.Services.AddSingleton<IMediaProcessorTask, BuildVideoPreviewTask>();
            builder.Services.AddSingleton<IMediaProcessorTask, BuildGifVideoPreviewTask>();
            builder.Services.AddSingleton<IMediaProcesserTaskFactory, MediaProcesserTaskFactory>();
            builder.Services.AddSingleton<IMediaProcessorFlowFactory, MediaProcessorFlowFactory>();
            builder.Services.AddSingleton<IMediaProcessorFlowFactory, MediaProcessorFlowFactory>();
            builder.Services.AddSingleton<IMediaFaceScanner, MediaFaceScanner>();
            builder.Services.AddSingleton<IMediaSourceScanner, MediaSourceScanner>();
            builder.Services.AddSingleton<IRescanFacesHandler, RescanFacesHandler>();

            return builder;
        }
    }
}
