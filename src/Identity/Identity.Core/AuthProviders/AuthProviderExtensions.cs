using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace MagicMedia.Identity.AuthProviders;

public static class AuthProviderExtensions
{
    public static AuthenticationBuilder AddExternalProviders(
        this AuthenticationBuilder authBuilder,
        IConfiguration configuration)
    {
        IEnumerable<AuthProviderOptions>? options = configuration
            .GetSection("Identity:AuthProviders")
            .Get<IEnumerable<AuthProviderOptions>>();

        foreach (AuthProviderOptions? option in options)
        {
            Log.Information("Add {Name} auth with clientId: {ClientId}",option.Name,option.ClientId);

            authBuilder.Services.AddSingleton(option);

            switch (option.Name)
            {
                case "Google":
                    authBuilder.AddGoogle(option);
                    break;
                case "GitHub":
                    authBuilder.AddGitHub(option);
                    break;
                case "Microsoft":
                    authBuilder.AddMicrosoftAccount(option);
                    break;
            }
        }

        return authBuilder;
    }
}
