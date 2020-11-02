using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Discovery;
using MagicMedia.Processing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MagicMedia.Api.Controllers
{
    [Route("api/media")]
    public class MediaController : Controller
    {
        private readonly IMediaBlobStore _mediaBlobStore;
        private readonly IThumbnailBlobStore _thumbnailBlobStore;
        private readonly IMediaProcessorFlowFactory _processorFlowFactory;

        public MediaController(
            IMediaBlobStore mediaBlobStore,
            IThumbnailBlobStore thumbnailBlobStore,
            IMediaProcessorFlowFactory processorFlowFactory)
        {
            _mediaBlobStore = mediaBlobStore;
            _thumbnailBlobStore = thumbnailBlobStore;
            _processorFlowFactory = processorFlowFactory;
        }

        [HttpGet]
        [Route("webimage/{id}")]
        public async Task<IActionResult> WebImageAsync(Guid id, CancellationToken cancellationToken)
        {
            MediaBlobData data = await _mediaBlobStore.GetAsync(
                new MediaBlobData
                {
                    Type = MediaBlobType.Web,
                    Filename = id.ToString("N") + ".webp"
                },
                cancellationToken);

            return new FileContentResult(data.Data, "image/webp");
        }

        [HttpGet]
        [Route("thumbnail/{id}")]
        public async Task<IActionResult> ThumbnailAsync(Guid id, CancellationToken cancellationToken)
        {
            byte[] data = await _thumbnailBlobStore.GetAsync(id, cancellationToken);

            return new FileContentResult(data, "image/jpg");
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> UploadAsync(
            IFormFile file,
            CancellationToken cancellationToken)
        {
            using var target = new MemoryStream();
            file.CopyTo(target);

            IMediaProcessorFlow flow = _processorFlowFactory.CreateFlow("ImportImage");

            await flow.ExecuteAsync(new MediaProcessorContext
            {
                OriginalData = target.ToArray(),
                File = new MediaDiscoveryIdentifier
                {
                    Id = file.FileName,
                    Source = MediaDiscoverySource.WebUpload
                }
            }, cancellationToken);

            return Ok();
        }
    }
}
