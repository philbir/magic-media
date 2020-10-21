using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MagicMedia.Api.Controllers
{
    [Route("media")]
    public class MediaController : Controller
    {
        private readonly IMediaBlobStore _mediaBlobStore;
        private readonly IThumbnailBlobStore _thumbnailBlobStore;

        public MediaController(
            IMediaBlobStore mediaBlobStore,
            IThumbnailBlobStore thumbnailBlobStore)
        {
            _mediaBlobStore = mediaBlobStore;
            _thumbnailBlobStore = thumbnailBlobStore;
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
    }
}
