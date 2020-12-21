using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.DependencyInjection;

namespace MagicMedia.Identity.AuthProviders
{
    public static class MicrosoftAccountExtensions
    {
        internal static AuthenticationBuilder AddMicrosoftAccount(
            this AuthenticationBuilder authBuilder,
            AuthProviderOptions options)
        {
            authBuilder.AddMicrosoftAccount(options.Name, o =>
            {
                o.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                o.ClientId = options.ClientId;
                o.ClientSecret = options.Secret;

                o.Events = new OAuthEvents
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
