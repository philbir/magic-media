using System.Security.Claims;
using System.Text.Encodings.Web;
using MagicMedia.Api.Security;
using MagicMedia.Security;
using MagicMedia.Store;
using MagicMedia.Store.MongoDb;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace MagicMedia.Api.Authorization;

public class ApiKeyAuthenticationOptions
    : AuthenticationSchemeOptions
{
}

public static class ApiKeyDefaults
{
    public static readonly string AuthenticationScheme = "ApiKey";
}

public static class ApiKeyExtensions
{
    public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        return builder
            .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
                ApiKeyDefaults.AuthenticationScheme,
                "API Key Auth",
                _ => { });
    }
}

public class ApiKeyAuthenticationHandler(
    IOptionsMonitor<ApiKeyAuthenticationOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IApiKeyValidator keyValidator,
    ISystemClock clock)
    : AuthenticationHandler<ApiKeyAuthenticationOptions>(options, logger, encoder, clock)
{

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var apiKey = GetApiKey(Request);
        if (string.IsNullOrEmpty(apiKey))
        {
            return AuthenticateResult.Fail("No Api Key found");
        }

        var name = await keyValidator.ValidateAsync(apiKey);

        if (name == null)
        {
            return AuthenticateResult.Fail("Invalid API Key");
        }

        Claim[] claims =
        [
            new Claim(ClaimTypes.NameIdentifier, $"API-{name}")
        ];

        var claimsIdentity = new ClaimsIdentity(claims, ApiKeyDefaults.AuthenticationScheme);

        var ticket = new AuthenticationTicket(
            new ClaimsPrincipal(claimsIdentity), Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }

    private string? GetApiKey(HttpRequest request)
    {
        request.Headers.TryGetValue("X-api-key", out StringValues key);

        return key;
    }
}
