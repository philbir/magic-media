using System;
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
        [Authorize(AuthorizationPolicies.Names.MediaDownload)]
        [HttpGet]
        [Route("{id}/{profile?}")]
        public async Task<IActionResult> DownloadAsync(
            Guid id,
            string profile,
            CancellationToken cancellationToken)
        {
            MediaDownload downloadResult = await _mediaDownloadService.CreateDownloadAsync(
                id,
                profile,
                cancellationToken);

            return new FileStreamResult(downloadResult.Stream, "application/octet-stream")
            {
                FileDownloadName = downloadResult.Filename
            };
        }
    }
}
