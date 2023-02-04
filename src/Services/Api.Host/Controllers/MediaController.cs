using MagicMedia.Authorization;
using MagicMedia.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MagicMedia.Api.Controllers;

[Authorize(AuthorizationPolicies.Names.ApiAccess)]
[Route("api/media")]
public class MediaController : Controller
{
    private readonly IMediaBlobStore _mediaBlobStore;
    private readonly IMediaService _mediaService;
    private readonly IThumbnailBlobStore _thumbnailBlobStore;

    public MediaController(
        IMediaBlobStore mediaBlobStore,
        IMediaService mediaService,
        IThumbnailBlobStore thumbnailBlobStore)
    {
        _mediaBlobStore = mediaBlobStore;
        _mediaService = mediaService;
        _thumbnailBlobStore = thumbnailBlobStore;
    }

    [Authorize(AuthorizationPolicies.Names.MediaView)]
    [HttpGet]
    [Route("webimage/{id}")]
    [ResponseCache(Duration = OutputCacheOptions.Duration, Location = ResponseCacheLocation.Client, NoStore = false)]
    public async Task<IActionResult> WebImageAsync(Guid id, CancellationToken cancellationToken)
    {
        MediaBlobData data = await _mediaBlobStore.GetAsync(
            new MediaBlobData
            {
                Type = MediaBlobType.Web,
                Filename = id.ToString("N") + ".webp"
            },
            cancellationToken);

        Response.Headers["X-Sw-Cache-Image"] = "true";

        return new FileContentResult(data.Data, "image/webp") { EnableRangeProcessing = true };
    }

    //[HttpGet]
    //[Route("thumbnail/{id}")]
    //[ResponseCache(Duration = OutputCacheOptions.Duration, Location = ResponseCacheLocation.Client, NoStore = false)]
    //public async Task<IActionResult> ThumbnailAsync(Guid id, CancellationToken cancellationToken)
    //{
    //    //TODO: Authorize Thumb
    //    byte[] data = await _thumbnailBlobStore.GetAsync(id, cancellationToken);

    //    Response.Headers["X-Is-Cacheable"] = "true";
    //    return new FileContentResult(data, "image/jpg");
    //}

    [Authorize(AuthorizationPolicies.Names.MediaView)]
    [HttpGet]
    [Route("{id}/thumbnail/{size?}")]
    [ResponseCache(Duration = OutputCacheOptions.Duration, Location = ResponseCacheLocation.Client, NoStore = false)]
    public async Task<IActionResult> ThumbnailByMediaAsync(
        Guid id,
        ThumbnailSizeName size = ThumbnailSizeName.M,
        CancellationToken cancellationToken = default)
    {
        MediaThumbnail? thumb = await _mediaService.GetThumbnailAsync(id, size, cancellationToken);

        if (thumb == null)
        {
            return NotFound();
        }

        Response.Headers["X-Sw-Cache-Thumbnail"] = "true";
        return new FileContentResult(thumb?.Data, "image/jpg");
    }

    [Authorize(AuthorizationPolicies.Names.MediaView)]
    [HttpGet]
    [Route("{id}/thumbnailbyid/{thumbnailId}")]
    [ResponseCache(Duration = OutputCacheOptions.Duration, Location = ResponseCacheLocation.Client, NoStore = false)]
    public async Task<IActionResult> ThumbnailByIdAsync(
        Guid id, //required for auth,
        Guid thumbnailId,
        CancellationToken cancellationToken = default)
    {
        byte[] data = await _thumbnailBlobStore.GetAsync(thumbnailId, cancellationToken);

        Response.Headers["X-Sw-Cache-Thumbnail"] = "true";
        return new FileContentResult(data, "image/webp");
    }
}
