using System;
using System.Threading.Tasks;
using IdentityModel;
using MagicMedia.Api;
using MagicMedia.Api.DevTokenAuthentication;
using MagicMedia.Api.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace MagicMedia
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddAuthorization(
            this IServiceCollection services,
            IWebHostEnvironment env)
        {
            services.AddAuthorization(c =>
            {
                c.AddPolicy("ApiAccess", new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build());

                c.AddPolicy("Media_View", p => p.Requirements.Add(new AuhorizedOnMediaRequirement()));
            });

            services.AddSingleton<IAuthorizationHandler, MediaAuthorizationHandler>();

            return services;
        }
    }

    public static class AuthenticationExtensions
    {
        public static AuthenticationBuilder AddAuthentication(
            this IServiceCollection services,
            IWebHostEnvironment env,
            IConfiguration configuration)
        {
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
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.Cookie.Name = "mm-id";
            })
            .AddOpenIdConnect("oidc", options =>
            {
                options.Authority = secOptions.Authority;
                options.RequireHttpsMetadata = false;

                options.ClientSecret = secOptions.Secret;
                options.ClientId = secOptions.ClientId;
                options.ResponseType = "code";

                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");

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
            });

            //.AddJwtBearer("jwt", options =>
            //    {
            //        options.RequireHttpsMetadata = false;
            //        options.Authority = secOptions.Authority;
            //        options.Audience = "api.magic";
            //    });

            if (env.IsDevelopment())
            {
                authBuilder.AddDevToken();
            }


            return authBuilder;
        }
    }
}
