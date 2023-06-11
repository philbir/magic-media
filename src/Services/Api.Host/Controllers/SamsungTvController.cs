using BingMapsRESTToolkit;
using MagicMedia.Authorization;
using MagicMedia.SamsungTv;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MagicMedia.Api.Controllers;

[Authorize(AuthorizationPolicies.Names.ApiAccess)]
[Route("api/samsung")]
public class SamsungTvController
{
    private readonly ISamsungTvClientFactory _clientFactory;

    public SamsungTvController(ISamsungTvClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [Authorize(AuthorizationPolicies.Names.MediaView)]
    [HttpGet]
    [Route("thumbnail/{device}/{id}")]
    public async Task<IActionResult> GetPreviewAsync(
        string device,
        string id,
        CancellationToken cancellationToken = default)
    {
        ISamsungTvArtModeClient client = _clientFactory.Create(device);

        var data = await client.GetPreviewAsync(id, cancellationToken);

        return new FileContentResult(data, "image/jpg");
    }
}
