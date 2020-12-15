using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Processing;
using MagicMedia.Store;
using MagicMedia.Store.MongoDb;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SixLabors.ImageSharp;

namespace MagicMedia.Playground
{
    public class FaceScanner
    {
        private readonly MediaStoreContext _storeContext;
        private readonly IMediaService _mediaService;
        private readonly IMediaProcessorFlowFactory _flowFactory;

        public FaceScanner(
            MediaStoreContext storeContext,
            IMediaService mediaService,
            IMediaProcessorFlowFactory flowFactory)
        {
            _storeContext = storeContext;
            _mediaService = mediaService;
            _flowFactory = flowFactory;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            IMediaProcessorFlow flow = _flowFactory.CreateFlow("ScanFaces");

            List<Store.Media> medias = await _storeContext.Medias.AsQueryable()
                .Where(x =>
                    x.FaceCount == 0 &&
                    x.MediaType == MediaType.Image &&
                    x.State == MediaState.Active)

                .OrderByDescending(x => x.DateTaken)
                .Take(100)
                .ToListAsync(cancellationToken);

            foreach (Media media in medias)
            {
                try
                {
                    Console.WriteLine($"Scanning faces: {media.Id}");
                    var context = new MediaProcessorContext
                    {
                        Media = media,
                        Image = await GetImageAsync(media)
                    };

                    await flow.ExecuteAsync(context, cancellationToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }

        private async Task<Image> GetImageAsync(Media media)
        {
            var fileName = _mediaService.GetFilename(media, MediaFileType.Original);

            return await Image.LoadAsync(fileName);
        }
    }
}
