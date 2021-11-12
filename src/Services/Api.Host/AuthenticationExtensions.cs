using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using IdentityModel;
using MagicMedia.Api;
using MagicMedia.Api.DevTokenAuthentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace MagicMedia
{
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
                options.DefaultScheme = env.IsDevelopment() ?
                    DevTokenDefaults.AuthenticationScheme :
                    CookieAuthenticationDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme = "oidc";
            });

            authBuilder.AddCookie(options =>
            {
                options.SlidingExpiration = true;
                options.Cookie.Name = "mm-id";
            })
            .AddOpenIdConnect("oidc", options =>
            {
                options.Authority = secOptions.Authority;
                options.RequireHttpsMetadata = env.IsProduction();

                options.ClientSecret = secOptions.Secret;
                options.ClientId = secOptions.ClientId;
                options.ClaimActions.MapUniqueJsonKey("scope", "scope");
                options.ResponseType = "code";
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("api.magic.read");
                options.Scope.Add("api.magic.write");

                options.ClaimActions.MapAllExcept("iss", "nbf", "exp", "aud", "nonce", "iat", "c_hash");

                options.Events = new OpenIdConnectEvents
                {
                    OnRedirectToIdentityProvider = (ctx) =>
                    {
                        return Task.CompletedTask;
                    },
                    OnTicketReceived = (ctx) =>
                    {
                        return Task.CompletedTask;
                    },
                    OnAuthorizationCodeReceived = (ctx) =>
                    {
                        return Task.CompletedTask;
                    }
                };

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = JwtClaimTypes.Name,
                    RoleClaimType = JwtClaimTypes.Role,
                };
            })
            .AddJwtBearer("jwt", options =>
            {
                options.RequireHttpsMetadata = env.IsProduction();
                options.Authority = secOptions.Authority;
                options.Audience = "api.magic";
            });

            if (env.IsDevelopment())
            {
                SetupDevelopmentAuthentication(authBuilder);
            }

            return authBuilder;
        }

        static partial void SetupDevelopmentAuthentication(AuthenticationBuilder builder);
    }
}
