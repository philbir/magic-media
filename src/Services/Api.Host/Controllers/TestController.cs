using MagicMedia.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MagicMedia.Host.Controllers;

[Route("api/test")]
[AllowAnonymous]
public class TestController : Controller
{
    private readonly IUserService _userService;

    public TestController(IUserService userService)
    {
        _userService = userService;
    }


    [Route("foo")]
    [HttpGet]
    public async Task<IActionResult> Foo(CancellationToken cancellationToken)
    {
        IEnumerable<Store.User>? users = await _userService.GetAllAsync(cancellationToken);
        return Json(users.First());
    }


    [Route("bar")]
    [HttpGet]
    public async Task<IActionResult> Bar()
    {
        return Json(true);
    }
}
