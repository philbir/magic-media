using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Extensions;
using MagicMedia.Store;
using MagicMedia.Store.MongoDb;
using MagicMedia.Telemetry;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Quartz;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace MagicMedia.Jobs;

public class UpdateWebPreviewJob : IJob
{
    private readonly MediaStoreContext _storeContext;
    private readonly IMediaStore _store;
    private readonly IWebPreviewImageService _webPreviewImageService;
    private readonly IWebPImageConverter _converter;

    public UpdateWebPreviewJob(
        IMediaService mediaService,
        MediaStoreContext storeContext,
        IMediaStore store,
        IWebPreviewImageService webPreviewImageService)
    {
        _storeContext = storeContext;
        _store = store;
        _webPreviewImageService = webPreviewImageService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        using Activity? activity = Tracing.Source.StartRootActivity(
            "Execute UpdateWebPreview job");

        Console.WriteLine($"Start create WebPreview");

        IReadOnlyList<TagDefintion> tagDefs =
            await _store.TagDefinitions.GetAllAsync(context.CancellationToken);

        var tagId = tagDefs.FirstOrDefault(x => x.Name.Equals("CC-MissingPreview"));

        List<Media> medias = await _storeContext.Medias.AsQueryable()
            .Where(x => x.Tags.Any(t => t.DefinitionId == tagId.Id) && x.State == MediaState.Active &&
                        x.MediaType == MediaType.Image)
            .ToListAsync(context.CancellationToken);

        Console.WriteLine($"{medias.Count} media to go");

        foreach (Media media in medias)
        {
            try
            {
                Console.WriteLine($"Create preview for {media.Id}");
                await _webPreviewImageService.SavePreviewImageAsync(media, context.CancellationToken);

                await _store.RemoveTagsByDefinitionIdAsync(media.Id, new[] { tagId.Id }, context.CancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
