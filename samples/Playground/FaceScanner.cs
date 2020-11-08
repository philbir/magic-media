using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        private readonly IMediaProcessorFlowFactory _flowFactory;

        public FaceScanner(
            MediaStoreContext storeContext,
            IMediaProcessorFlowFactory flowFactory)
        {
            _storeContext = storeContext;
            _flowFactory = flowFactory;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            IMediaProcessorFlow flow = _flowFactory.CreateFlow("ScanFaces");

            List<Store.Media> medias = await _storeContext.Medias.AsQueryable()
                .Where(x => x.FaceCount == 0)
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
            return await Image.LoadAsync(media.Source.Identifier);
        }
    }
}