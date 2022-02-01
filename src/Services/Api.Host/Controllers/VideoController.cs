using System;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MagicMedia.Api.Controllers;

[Authorize(AuthorizationPolicies.Names.ApiAccess)]
[Route("api/video")]
public class VideoController : Controller
{
    private readonly IVideoPlayerService _videoPlayerService;

    public VideoController(IVideoPlayerService videoPlayerService)
    {
        _videoPlayerService = videoPlayerService;
    }

    [Authorize(AuthorizationPolicies.Names.MediaView)]
    [Route("preview/{id}")]
    [HttpGet]
    [ResponseCache(Duration = OutputCacheOptions.Duration, Location = ResponseCacheLocation.Client, NoStore = false)]
    public IActionResult Preview(Guid id, CancellationToken cancellationToken)
    {
        MediaStream? mediaStream = _videoPlayerService.GetVideoPreview(id, cancellationToken);
        return new FileStreamResult(mediaStream.Stream, mediaStream.MimeType)
        { EnableRangeProcessing = true };
    }

    [Authorize(AuthorizationPolicies.Names.MediaView)]
    [Route("{id}")]
    [HttpGet]
    public async Task<IActionResult> VideoAsync(Guid id, CancellationToken cancellationToken)
    {
        MediaStream? mediaStream = await _videoPlayerService.GetVideoAsync(id, cancellationToken);

        return new FileStreamResult(mediaStream.Stream, $"video/{mediaStream.MimeType}")
        { EnableRangeProcessing = true };
    }
}
