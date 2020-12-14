using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.ImageAI;
using MagicMedia.Store;
using Microsoft.AspNetCore.Mvc;

namespace MagicMedia.Api.Controllers
{
    [Route("api/ai")]
    public class MediaAIController : Controller
    {
        private readonly IMediaAIService _mediaAIService;
        private readonly IMediaService _mediaService;

        public MediaAIController(IMediaAIService mediaAIService, IMediaService mediaService)
        {
            _mediaAIService = mediaAIService;
            _mediaService = mediaService;
        }

        [HttpGet]
        [Route("MediaWithoutImageAISource")]
        public async Task<IActionResult> GetMediaWithoutImageAISourceAsync(CancellationToken cancellationToken)
        {
            IEnumerable<Media> medias = await _mediaAIService.GetMediaIdsForImageAIJobAsync(10, cancellationToken);

            return Ok(medias.Select(x => new
            {
                Id = x.Id,
                Dimension = x.Dimension
            }));
        }

        [HttpGet]
        [Route("image/{id}")]
        public async Task<IActionResult> GetImageAsync(Guid id, CancellationToken cancellationToken)
        {
            Media media = await _mediaService.GetByIdAsync(id, cancellationToken);

            MediaBlobData data = await _mediaService.GetMediaData(media, cancellationToken);

            return new FileContentResult(data.Data, "image/jpg");
        }


        [HttpPost]
        [Route("save")]
        public async Task<IActionResult> AddImageAIData([FromBody] ImageAIDetectionResult imageAIResult, CancellationToken cancellationToken)
        {
            await _mediaAIService.SaveImageAIDetectionAsync(imageAIResult, cancellationToken);

            return Ok(true);
        }
    }
}
