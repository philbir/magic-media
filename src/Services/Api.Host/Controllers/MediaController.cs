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

        public MediaController(IMediaBlobStore mediaBlobStore)
        {
            _mediaBlobStore = mediaBlobStore;
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
    }
}
