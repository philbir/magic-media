using System.IdentityModel.Tokens.Jwt;
using IdentityModel;
using MagicMedia.Api;
using MagicMedia.Api.DevTokenAuthentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace MagicMedia;

public static partial class AuthenticationExtensions
{
    public static AuthenticationBuilder AddAuthentication(
        this IServiceCollection services,
        IWebHostEnvironment env,
        IConfiguration configuration)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        SecurityOptions secOptions = configuration.GetSection("MagicMedia:Security")
            .Get<SecurityOptions>();

        AuthenticationBuilder authBuilder = services.AddAuthentication(options =>
        {
            options.DefaultScheme = "jwt";
        }).AddJwtBearer("jwt", options =>
        {
            options.RequireHttpsMetadata = env.IsProduction();
            options.Authority = secOptions.Authority;
            options.Audience = "api.magic";
        });

        /*
        if (env.IsDevelopment())
        {
            SetupDevelopmentAuthentication(authBuilder);
        }*/

        return authBuilder;
    }

    static partial void SetupDevelopmentAuthentication(AuthenticationBuilder builder);
}
