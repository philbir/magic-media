using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Discovery;
using MagicMedia.Face;
using MagicMedia.Processing;
using MagicMedia.Store;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MagicMedia.Api.Controllers
{
    [Route("api/media")]
    public class MediaController : Controller
    {
        private readonly IMediaBlobStore _mediaBlobStore;
        private readonly IMediaService _mediaService;
        private readonly IThumbnailBlobStore _thumbnailBlobStore;
        private readonly IFaceService _faceService;

        public MediaController(
            IMediaBlobStore mediaBlobStore,
            IMediaService mediaService,
            IThumbnailBlobStore thumbnailBlobStore,
            IFaceService faceService)
        {
            _mediaBlobStore = mediaBlobStore;
            _mediaService = mediaService;
            _thumbnailBlobStore = thumbnailBlobStore;
            _faceService = faceService;
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

        [HttpGet]
        [Route("thumbnail/media/{id}/{size?}")]
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

            return new FileContentResult(thumb?.Data, "image/jpg");
        }


        [HttpGet]
        [Route("thumbnail/face/{faceId}")]
        public async Task<IActionResult> ThumbnailByFaceAsync(
            Guid faceId,
            CancellationToken cancellationToken)
        {
            MediaThumbnail thumb = await _faceService.GetThumbnailAsync(faceId, cancellationToken);

            return new FileContentResult(thumb.Data, $"image/{thumb.Format}".ToLower());
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> UploadAsync(
            IFormFile file,
            CancellationToken cancellationToken)
        {
            using var target = new MemoryStream();
            file.CopyTo(target);

            //Use massaging to process file

            //IMediaProcessorFlow flow = _processorFlowFactory.CreateFlow("ImportImage");

            //await flow.ExecuteAsync(new MediaProcessorContext
            //{
            //    OriginalData = target.ToArray(),
            //    File = new MediaDiscoveryIdentifier
            //    {
            //        Id = file.FileName,
            //        Source = MediaDiscoverySource.WebUpload
            //    }
            //    ,
            //    Options = new MediaProcessingOptions
            //    {
            //        SaveMedia = new SaveMediaFileOptions
            //        {
            //            SaveMode = SaveMediaMode.CreateNew,
            //            SourceAction = SaveMediaSourceAction.Keep
            //        }
            //    }
            //}, cancellationToken);

            return Ok();
        }


    }
}
