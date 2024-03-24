using System.Security.Claims;
using System.Text.Encodings.Web;
using MagicMedia.Security;
using MagicMedia.Store;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using RabbitMQ.Client;

namespace MagicMedia.Api.Authorization;

public class ThrustedHeaderAuthenticationOptions
    : AuthenticationSchemeOptions
{
    public IEnumerable<ThrustedHeaderConfiguration> Headers { get; set; }
}

public class ThrustedHeaderConfiguration
{
    public string HeaderName { get; set; }

    public string Method { get; set; }
}

public static class ThrustedHeaderDefaults
{
    public static readonly string AuthenticationScheme = "ThrustedHeader";
}

public static class ThrustedHeaderExtensions
{
    public static AuthenticationBuilder AddThrustedHeader(this AuthenticationBuilder builder)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        return builder
            .AddScheme<ThrustedHeaderAuthenticationOptions, ThrustedHeaderAuthenticationHandler>(
                ThrustedHeaderDefaults.AuthenticationScheme,
                "Thrusted Header Auth",
                _ => { });
    }
}

public class ThrustedHeaderAuthenticationHandler
    : AuthenticationHandler<ThrustedHeaderAuthenticationOptions>
{
    private readonly IUserService _userService;

    public ThrustedHeaderAuthenticationHandler(
        IOptionsMonitor<ThrustedHeaderAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IUserService userService,
        ISystemClock clock) : base(options, logger, encoder, clock)
    {
        _userService = userService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string username = null;
        string method = null;

        foreach (ThrustedHeaderConfiguration headerConfiguration in Options.Headers)
        {
            username = GetUsername(Request, headerConfiguration.HeaderName);

            Logger.LogInformation("Get user from header:  {headerConfiguration.HeaderName} -> {Username}", username);

            if (username != null)
            {
                method = headerConfiguration.Method;
                break;
            }
        }

        if (string.IsNullOrEmpty(username))
        {
            return AuthenticateResult.Fail("No forwarded user found");
        }

        User? user = await _userService.TryGetByIdentifier(method, username, CancellationToken.None);

        if (user == null)
        {
            return AuthenticateResult.Fail("User not found");
        }

        Claim[] claims =
        [
            new Claim("sub", user.Id.ToString("N")),
            new Claim(ClaimTypes.NameIdentifier, username)
        ];

        var claimsIdentity = new ClaimsIdentity(claims, ThrustedHeaderDefaults.AuthenticationScheme);

        var ticket = new AuthenticationTicket(
            new ClaimsPrincipal(claimsIdentity), Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }

    private string? GetUsername(HttpRequest request, string headerName)
    {
        request.Headers.TryGetValue(headerName, out StringValues username);

        return username;
    }
}
