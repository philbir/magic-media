using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MagicMedia.Api.Controllers;

[Route("info")]
public class InfoController : Controller
{
    [HttpGet("")]
    [AllowAnonymous]
    public IActionResult Info()
    {
        var headers = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
        return Json(headers);
    }
}
