using System;
using MagicMedia.Audit;
using MagicMedia.Face;
using MagicMedia.Messaging;
using MagicMedia.Metadata;
using MagicMedia.Operations;
using MagicMedia.Processing;
using MagicMedia.Security;
using MagicMedia.Thumbnail;
using MagicMedia.Thumbprint;
using MagicMedia.Video;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MagicMedia;

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
        services.AddSingleton<IExportMediaHandler, ExportMediaHandler>();

        services.AddSingleton<IMediaSearchService, MediaSearchService>();
        services.AddSingleton<IMediaService, MediaService>();
        services.AddSingleton<IVideoPlayerService, VideoPlayerService>();
        services.AddSingleton<IVideoProcessingService, VideoProcessingService>();
        services.AddSingleton<IMediaAIService, MediaAIService>();
        services.AddFFmpeg(configuration);

        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<IAlbumMediaIdResolver, AlbumMediaIdResolver>();
        services.AddSingleton<IUserAuthorizationService, UserAuthorizationService>();
        services.AddSingleton<IAuditService, AuditService>();
        services.AddSingleton<IUserContextMessagePublisher, UserContextMessagePublisher>();
        services.AddSingleton<IMediaDownloadService, MediaDownloadService>();
        services.AddSingleton<IMediaExportService, MediaExportService>();
        services.AddSingleton<ISimilarMediaService, SimilarMediaService>();
        services.AddSingleton<IDuplicateMediaGuard, DuplicateMediaGuard>();
        services.AddSingleton<IMediaExportProfileService, MediaExportProfileService>();
        services.AddSingleton<IMediaTransformService, MediaTransformService>();

        return services;
    }

    public static IMagicMediaServerBuilder AddClientThumbprintServices(
        this IMagicMediaServerBuilder builder)
    {

        IPGeolocationApiOptions options = builder.Configuration
            .GetSection("MagicMedia:IPGeolocation")
            .Get<IPGeolocationApiOptions>();

        builder.Services.AddHttpClient("GeoIP", c =>
        {
            c.BaseAddress = new Uri(options.Url);
        }).AddHttpMessageHandler(h => new IPGeolocationApiKeyHandler(options));


        builder.Services.AddSingleton<IGeoIPLocationService, IPGeolocationApiClient>();
        builder.Services.AddSingleton<IUserAgentInfoService, UserAgentInfoService>();
        builder.Services.AddSingleton<IClientThumbprintService, ClientThumbprintService>();

        return builder;
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
        builder.Services.AddSingleton<IMediaProcessorTask, CreateMediaHashesTask>();
        builder.Services.AddSingleton<IMediaProcessorTask, CheckDuplicateTask>();
        builder.Services.AddSingleton<IMediaProcesserTaskFactory, MediaProcesserTaskFactory>();
        builder.Services.AddSingleton<IMediaProcessorFlowFactory, MediaProcessorFlowFactory>();
        builder.Services.AddSingleton<IMediaProcessorFlowFactory, MediaProcessorFlowFactory>();
        builder.Services.AddSingleton<IMediaFaceScanner, MediaFaceScanner>();
        builder.Services.AddSingleton<IMediaSourceScanner, MediaSourceScanner>();
        builder.Services.AddSingleton<IRescanFacesHandler, RescanFacesHandler>();

        return builder;
    }
}
