using MagicMedia.Identity.Data.Mongo.Seeding;
using MagicMedia.Identity.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Extensions.Context;
using Serilog;

namespace MagicMedia.Identity.Data.Mongo
{
    public static class DataServiceCollectionExtensions
    {
        public static IServiceCollection AddDataAccess(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            MongoOptions dbOptions = GetMongoDbOptions(configuration);

            services.AddSingleton(dbOptions);
            services.AddSingleton<IIdentityDbContext>(new IdentityDbContext(dbOptions));
            services.AddSingleton<IClientRepository, ClientRepository>();
            services.AddSingleton<IPersistedGrantRepository, PersistedGrantRepository>();
            services.AddSingleton<IApiScopeRepository, ApiScopeRepository>();
            services.AddSingleton<IApiResourceRepository, ApiResourceRepository>();
            services.AddSingleton<IIdentityResourceRepository, IdentityResourceRepository>();
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IInviteRepository, InviteRepository>();

            services.AddSingleton<DataSeeder>();

            return services;
        }

        private static MongoOptions GetMongoDbOptions(IConfiguration configuration)
        {
            string sectionName = Wellknown.ConfigSections.Identity;

            MongoOptions options = configuration
                .GetSection($"{sectionName}:Database")
                .Get<MongoOptions>();

            if (options == null)
            {
                throw new IdentityConfigurationException(
                    $@"Could not load configuration for Database, make sure that the
                      section: '{sectionName}:Database' is defined");
            }

            if (options.ConnectionString == null)
            {
                throw new IdentityConfigurationException(
                    $@"Database ConnectionString can not be null, make sure
                      key: '{sectionName}:Database:ConnectionString' is defined");
            }

            if (options.DatabaseName == null)
            {
                throw new IdentityConfigurationException(
                    $@"DatabaseName can not be null, make sure
                      key: '{sectionName}:Database:DatabaseName' is defined");
            }

            Log.Information("MongoDb: {ConnectionString}", options.ConnectionString);

            return options;
        }
    }
}
