using Duende.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.DependencyInjection;

namespace MagicMedia.Identity.AuthProviders;

public static class GitHubExtensions
{
    internal static AuthenticationBuilder AddGitHub(
        this AuthenticationBuilder authBuilder,
        AuthProviderOptions options)
    {

        authBuilder.AddGitHub(options.Name, gitOptions =>
        {
            gitOptions.SignInScheme =  IdentityServerConstants
                .ExternalCookieAuthenticationScheme;

            gitOptions.ClientId = options.ClientId;
            gitOptions.ClientSecret = options.Secret;

            gitOptions.Events = new OAuthEvents
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
