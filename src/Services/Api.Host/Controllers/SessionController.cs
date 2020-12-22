using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MagicMedia.Api.Controllers
{
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

        [AllowAnonymous]
        [Route("auth")]
        public IActionResult Authenticate()
        {
            return Redirect("/");
        }

        [AllowAnonymous]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect("/loggedout");
        }
    }
}
