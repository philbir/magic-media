using MagicMedia.Configuration;
using MagicMedia.Face;
using MagicMedia.Metadata;
using MagicMedia.Operations;
using MagicMedia.Processing;
using MagicMedia.Thumbnail;
using MagicMedia.Video;
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
            services.AddSingleton<IUpdateMediaMetadataHandler, UpdateMediaMetadataHandler>();
            services.AddSingleton<IMediaSearchService, MediaSearchService>();
            services.AddSingleton<IMediaService, MediaService>();
            services.AddSingleton<IVideoPlayerService, VideoPlayerService>();
            services.AddSingleton<IVideoProcessingService, VideoProcessingService>();

            return services;
        }


        public static IMagicMediaServerBuilder AddProcessingMediaServices(this IMagicMediaServerBuilder builder)
        {
            builder.Services.AddSingleton<IMediaProcesserTask, AutoOrientTask>();
            builder.Services.AddSingleton<IMediaProcesserTask, ExtractMetadataTask>();
            builder.Services.AddSingleton<IMediaProcesserTask, GenerateThumbnailsTask>();
            builder.Services.AddSingleton<IMediaProcesserTask, BuildFaceDataTask>();
            builder.Services.AddSingleton<IMediaProcesserTask, GenerateWebImageTask>();
            builder.Services.AddSingleton<IMediaProcesserTask, SaveMediaTask>();
            builder.Services.AddSingleton<IMediaProcesserTask, PredictPersonsTask>();
            builder.Services.AddSingleton<IMediaProcesserTask, SaveFaceDataAsync>();
            builder.Services.AddSingleton<IMediaProcesserTask, ExtractVideoDataTask>();
            builder.Services.AddSingleton<IMediaProcesserTask, CleanUpSourceTask>();
            builder.Services.AddSingleton<IMediaProcesserTaskFactory, MediaProcesserTaskFactory>();
            builder.Services.AddSingleton<IMediaProcessorFlowFactory, MediaProcessorFlowFactory>();
            builder.Services.AddSingleton<IMediaProcessorFlowFactory, MediaProcessorFlowFactory>();
            builder.Services.AddSingleton<IMediaFaceScanner, MediaFaceScanner>();
            builder.Services.AddSingleton<IMediaSourceScanner, MediaSourceScanner>();

            return builder;
        }
    }
}
