using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MagicMedia.Host.Controllers;

[Route("api/test2")]
[AllowAnonymous]
public class Test2Controller : Controller
{
    [Route("bar")]
    [HttpGet]
    public async Task<IActionResult> Bar()
    {
        return Json(true);
    }
}
