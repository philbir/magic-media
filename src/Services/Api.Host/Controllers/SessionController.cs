using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MagicMedia.Api.Controllers;

[Route("api/session")]
public class SessionController : Controller
{
    [AllowAnonymous]
    [Route("")]
    public IActionResult Check()
    {
        return Ok(new
        {
            IsAuthenticated = User.Identity?.IsAuthenticated
        });
    }
}
