using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MagicMedia.Api.DevTokenAuthentication;

public class DevTokenAuthorizationHandler
      : AuthenticationHandler<DevTokenAuthenticationOptions>
{
    public DevTokenAuthorizationHandler(
        IOptionsMonitor<DevTokenAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock)
    { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Request.Host.Host != "localhost")
        {
            return Task.FromResult(AuthenticateResult.Fail("Not localhost"));
        }

        var token = GetDevToken(Request);
        if (string.IsNullOrEmpty(token))
        {
            return Task.FromResult(AuthenticateResult.Fail("No token in header"));
        }

        Claim[]? claims = new[]
        {
                new Claim("sub", token),
                new Claim(ClaimTypes.NameIdentifier, token)
            };

        var claimsIdentity = new ClaimsIdentity(claims,
                    nameof(DevTokenAuthorizationHandler));

        var ticket = new AuthenticationTicket(
            new ClaimsPrincipal(claimsIdentity), Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private string? GetDevToken(HttpRequest request)
    {
        string authorization = request.Headers["Authorization"];

        if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith(
            "dev ",
            StringComparison.OrdinalIgnoreCase))
        {
            return authorization.Substring("dev ".Length).Trim();
        }

        return null;
    }
}
