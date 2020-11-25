using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;
using SixLabors.ImageSharp;

namespace MagicMedia.Processing
{
    public class MediaFaceScanner : IMediaFaceScanner
    {
        private readonly IMediaProcessorFlow _flow;
        private readonly IMediaService _mediaService;

        public MediaFaceScanner(
            IMediaProcessorFlowFactory flowFactory,
            IMediaService mediaService)
        {
            _flow = flowFactory.CreateFlow("ScanFaces");
            _mediaService = mediaService;
        }

        public async Task ScanByMediaIdAsync(Guid mediaId, CancellationToken cancellationToken)
        {
            Media media = await _mediaService.GetByIdAsync(mediaId, cancellationToken);
            Stream stream = _mediaService.GetMediaStream(media);

            var context = new MediaProcessorContext
            {
                Media = media,
                Image = await Image.LoadAsync(stream)
            };

            await _flow.ExecuteAsync(context, cancellationToken);
        }

        private async Task<Image> GetImageAsync(Media media)
        {
            return await Image.LoadAsync(media.Source.Identifier);
        }
    }
}
