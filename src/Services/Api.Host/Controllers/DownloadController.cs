using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MagicMedia.Api.Controllers
{
    [Authorize(AuthorizationPolicies.Names.ApiAccess)]
    [Route("api/download")]
    public class DownloadController : Controller
    {
        private readonly IMediaDownloadService _mediaDownloadService;

        public DownloadController(IMediaDownloadService mediaDownloadService)
        {
            _mediaDownloadService = mediaDownloadService;
        }


        [Authorize(AuthorizationPolicies.Names.MediaView)]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> DownloadAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            MediaDownload downloadResult = await _mediaDownloadService.CreateDownloadAsync(
                id, new DownloadMediaOptions(),
                cancellationToken);

            return new FileStreamResult(downloadResult.Stream, "application/octet-stream")
            {
                FileDownloadName = downloadResult.Filename
            };
        }
    }
}
