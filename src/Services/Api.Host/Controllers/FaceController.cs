using System;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Authorization;
using MagicMedia.Face;
using MagicMedia.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MagicMedia.Api.Controllers
{
    [Authorize(AuthorizationPolicies.Names.ApiAccess)]
    [Route("api/face")]
    public class FaceController : Controller
    {
        private readonly IThumbnailBlobStore _thumbnailBlobStore;
        private readonly IFaceService _faceService;

        public FaceController(
            IThumbnailBlobStore thumbnailBlobStore,
            IFaceService faceService)
        {
            _thumbnailBlobStore = thumbnailBlobStore;
            _faceService = faceService;
        }


        [Authorize(AuthorizationPolicies.Names.FaceView)]
        [HttpGet]
        [Route("{id}/thumbnail/{thumbnailId}")]
        public async Task<IActionResult> GetThumbnailAsync(
            Guid id, //Needed for authorization
            Guid thumbnailId,
            CancellationToken cancellationToken)
        {
            byte[] data = await _thumbnailBlobStore.GetAsync(thumbnailId, cancellationToken);

            return new FileContentResult(data, "image/jpg");
        }

        [Authorize(AuthorizationPolicies.Names.FaceView)]
        [HttpGet]
        [Route("{id}/thumbnail")]
        public async Task<IActionResult> GetThumbnailAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            MediaThumbnail thumb = await _faceService.GetThumbnailAsync(id, cancellationToken);

            return new FileContentResult(thumb.Data, $"image/{thumb.Format}".ToLower());
        }
    }
}
