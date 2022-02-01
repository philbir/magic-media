using System;
using MagicMedia.Identity.AuthProviders;
using MagicMedia.Identity.Services;
using MagicMedia.Identity.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MagicMedia.Identity;

public static class IdentityServerServiceCollectionExtensions
{
    public static IIdentityServerBuilder AddIdentityServer(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment hostEnvironment)
    {
        IIdentityServerBuilder builder = services.AddIdentityServer(options =>
       {
           options.Caching.ClientStoreExpiration = TimeSpan.FromMinutes(30);
           options.Caching.CorsExpiration = TimeSpan.FromMinutes(30);
           options.Caching.ResourceStoreExpiration = TimeSpan.FromMinutes(30);
           options.Events.RaiseErrorEvents = true;
           options.Events.RaiseFailureEvents = true;
           options.Events.RaiseInformationEvents = true;
           options.Events.RaiseSuccessEvents = true;
           options.Endpoints.EnableIntrospectionEndpoint = true;
       })
        .AddDeveloperSigningCredential(persistKey: true, filename: "sign_key.jwk")
        .AddInMemoryCaching()
        .AddResourceStoreCache<ResourceStore>()
        .AddPersistedGrantStore<PersistedGrantStore>()
        .AddCorsPolicyCache<CorsPolicyService>()
        .AddClientStoreCache<ClientStore>();

        AuthenticationBuilder authBuilder = services.AddAuthentication();
        if (!hostEnvironment.IsDemo())
        {
            authBuilder.AddExternalProviders(configuration);
        }

        return builder;
    }
}
