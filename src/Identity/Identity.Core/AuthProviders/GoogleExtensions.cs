using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace MagicMedia.Identity.AuthProviders
{
    public static class GoogleExtensions
    {
        internal static AuthenticationBuilder AddGoogle(
            this AuthenticationBuilder authBuilder,
            AuthProviderOptions options)
        {
            authBuilder.AddGoogle(options.Name, googleOptions =>
            {
                googleOptions.SignInScheme = IdentityServerConstants
                    .ExternalCookieAuthenticationScheme;

                googleOptions.ClientId = options.ClientId;
                googleOptions.ClientSecret = options.Secret;

                googleOptions.Events = new OAuthEvents
                {
                    OnRemoteFailure = (ctx) =>
                    {
                        return ctx.HandleRemoteFailure();
                    },
                    OnAccessDenied = (ctx) =>
                    {
                        return ctx.HandleAccessDenied();
                    }
                };
            });

            return authBuilder;
        }
    }
}
