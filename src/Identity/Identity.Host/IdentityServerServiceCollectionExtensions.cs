using System;
using MagicMedia.Identity.AuthProviders;
using MagicMedia.Identity.Services;
using MagicMedia.Identity.Stores;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MagicMedia.Identity
{
    public static class IdentityServerServiceCollectionExtensions
    {
        public static IIdentityServerBuilder AddIdentityServer(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            //LoginOptions loginOptions = configuration.GetLoginOptions();
            //CachingOptions? cacheOptions = loginOptions.Caching;

            //services.AddSingleton(loginOptions);
            //services.AddSingleton(cacheOptions);

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
            .AddInMemoryCaching()
            .AddResourceStoreCache<ResourceStore>()
            .AddPersistedGrantStore<PersistedGrantStore>()
            .AddCorsPolicyCache<CorsPolicyService>()
            .AddClientStoreCache<ClientStore>();

            services.AddAuthentication()
                  .AddExternalProviders(configuration);

            return builder;
        }
    }
}
