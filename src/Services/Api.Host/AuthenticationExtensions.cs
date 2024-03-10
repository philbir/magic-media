using MagicMedia.Api;
using MagicMedia.Api.Authorization;
using MagicMedia.Api.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.JsonWebTokens;

namespace MagicMedia;

public static partial class AuthenticationExtensions
{
    public static AuthenticationBuilder AddAuthentication(
        this IServiceCollection services,
        IWebHostEnvironment env,
        IConfiguration configuration)
    {
        JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

        services.AddOptions<ApiKeyOptions>()
            .Bind(configuration.GetSection("MagicMedia:Security:ApiKey"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<ThrustedHeaderAuthenticationOptions>(ThrustedHeaderDefaults.AuthenticationScheme)
            .Bind(configuration.GetSection("MagicMedia:Security:ThrustedHeader"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IApiKeyValidator, ApiKeyValidator>();

        AuthenticationBuilder authBuilder = services.AddAuthentication(options =>
        {
            options.DefaultScheme = ThrustedHeaderDefaults.AuthenticationScheme;
        }).AddThrustedHeader()
        .AddApiKey();

        return authBuilder;
    }
}
