using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MagicMedia.Api.Controllers
{
    [Route("api/video")]
    public class VideoController : Controller
    {
        private readonly IVideoPlayerService _videoPlayerService;

        public VideoController(IVideoPlayerService videoPlayerService)
        {
            _videoPlayerService = videoPlayerService;
        }

        [Route("preview/{id}")]
        [HttpGet]
        public IActionResult Preview(Guid id, CancellationToken cancellationToken)
        {
            MediaStream? mediaStream = _videoPlayerService.GetVideoPreview(id, cancellationToken);
            return new FileStreamResult(mediaStream.Stream, mediaStream.MimeType);
        }


        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> VideoAsync(Guid id, CancellationToken cancellationToken)
        {
            MediaStream? mediaStream = await _videoPlayerService.GetVideoAsync(id, cancellationToken);

            return new FileStreamResult(mediaStream.Stream, $"video/{mediaStream.MimeType}");
        }
    }
}
