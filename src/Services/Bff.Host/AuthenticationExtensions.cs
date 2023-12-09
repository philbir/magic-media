using System.IdentityModel.Tokens.Jwt;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace MagicMedia.Bff;

public static partial class AuthenticationExtensions
{
    public static AuthenticationBuilder AddAuthentication(
        this IServiceCollection services,
        IWebHostEnvironment env,
        IConfiguration configuration)
    {
        JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();
        SecurityOptions secOptions = configuration.GetSection("MagicMedia:Security")
            .Get<SecurityOptions>();

        AuthenticationBuilder authBuilder = services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            options.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
        });

        authBuilder.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = "__bff-mm-id";
                options.Cookie.SameSite = SameSiteMode.Lax;
            })
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.Authority = secOptions.Authority;
                options.RequireHttpsMetadata = env.IsProduction();
                options.GetClaimsFromUserInfoEndpoint = true;
                //options.MapInboundClaims = false;
                options.SaveTokens = true;
                options.ClientSecret = secOptions.Secret;
                options.ClientId = secOptions.ClientId;
                options.ClaimActions.MapUniqueJsonKey("scope", "scope");
                options.ResponseType = "code";
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("offline_access");
                options.Scope.Add("api.magic.read");
                options.Scope.Add("api.magic.write");

                options.ClaimActions.MapAllExcept("iss", "nbf", "exp", "aud", "nonce", "iat", "c_hash");

                options.Events = new OpenIdConnectEvents
                {
                    OnTicketReceived = (ctx) =>
                    {
                        return Task.CompletedTask;
                    },
                };

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = JwtClaimTypes.Name, RoleClaimType = JwtClaimTypes.Role,
                };
            });

        return authBuilder;
    }

    static partial void SetupDevelopmentAuthentication(AuthenticationBuilder builder);
}
