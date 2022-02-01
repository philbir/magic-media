using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using MagicMedia.Api.Host.Tests.Containers;
using MagicMedia.Store.MongoDb;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using MongoDB.Extensions.Context;
using Squadron;
using StrawberryShake;
using StrawberryShake.Http;
using StrawberryShake.Http.Pipelines;
using Xunit;

namespace MagicMedia.Api.Host.Tests.Infrastructure
{
    public class ApiTestServerWithIdentity : IAsyncLifetime
    {
        public HttpClient HttpClient { get; private set; }
        public IMagicMediaTest GraphQLClient { get; private set; }
        public ComposeResource<MagicApiAppOptions> Containers { get; private set; }
        public MongoResource MongoResource { get; private set; }
        public GenericContainerResource<IdentityHostOptions> IdentityHost { get; private set; }
        public IMongoDatabase Database { get; private set; }
        public MediaStoreContext DbContext { get; private set; }
        public IServiceProvider Services { get; private set; }

        private InMemoryHttpClientFactory _httpClientFactory = new InMemoryHttpClientFactory();

        public async Task InitializeAsync()
        {
            Containers = new ComposeResource<MagicApiAppOptions>();
            await Containers.InitializeAsync();

            MongoResource = Containers.GetResource<MongoResource>("mongo");
            IdentityHost = Containers
                .GetResource<GenericContainerResource<IdentityHostOptions>>("identity");

            Database = MongoResource.CreateDatabase();

            DbContext = new MediaStoreContext(
                new MongoOptions
                {
                    ConnectionString = MongoResource.ConnectionString,
                    DatabaseName = Database.DatabaseNamespace.DatabaseName
                });

            string identityUrl = IdentityHost.GetContainerUri().ToString();

            IWebHostBuilder hostBuilder = new WebHostBuilder()
                .ConfigureAppConfiguration(builder =>
                {
                    builder.SetBasePath(Directory.GetCurrentDirectory());
                    builder.AddJsonFile("appsettings.test.json", optional: true);
                    builder.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["MagicMedia:Database:ConnectionString"] =
                        MongoResource.ConnectionString,
                        ["MagicMedia:Database:DatabaseName"] =
                        Database.DatabaseNamespace.DatabaseName,
                        ["MagicMedia:Security:Authority"] = identityUrl
                    });
                })
                .ConfigureTestServices(services =>
                {
                    services.AddSingleton(_httpClientFactory);
                    services.AddMagicMediaTest();
                    services.RemoveAll<IOperationExecutorFactory>();
                    services.AddSingleton<IOperationExecutorFactory>(sp =>
                        new HttpOperationExecutorFactory(
                            "MagicMediaTest",
                            sp.GetRequiredService<InMemoryHttpClientFactory>().CreateClient,
                            PipelineFactory(sp),
                            sp));
                });
               //.UseStartup<Startup>();

            var server = new TestServer(hostBuilder);
            Services = server.Services;
            HttpClient = server.CreateClient();

            GraphQLClient = Services.GetService<IMagicMediaTest>();
            _httpClientFactory.HttpClient = server.CreateClient();
            _httpClientFactory.HttpClient.BaseAddress = new Uri("http://localhost/graphql");
            _httpClientFactory.TokenResolver = GetTokenAsync;

            await SeedIntialDataAsync();

            //TODO. Improve readycheck for IdentityServer container
            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        public async Task<string> GetTokenAsync()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = IdentityHost.GetContainerUri();

            TokenResponse res = await httpClient.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest
                {
                    Address = $"{httpClient.BaseAddress}connect/token",
                    ClientId = "Media.Test",
                    ClientSecret = "geCDNACu94a5DfZQ2Sm46DBjkSErAnNA"
                });

            return res.AccessToken;
        }


        public async Task DisposeAsync()
        {
            await Containers?.DisposeAsync();
        }

        public async Task SeedIntialDataAsync()
        {
            var seeder = new DataSeeder(DbContext);
            await seeder.SeedIntialDataAsync();
        }

        private static OperationDelegate PipelineFactory(IServiceProvider services)
        {
            return services.GetRequiredService<OperationDelegate>();
        }
    }
}
