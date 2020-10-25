using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using MagicMedia.Identity.Data;
using MagicMedia.Identity.Data.Mongo.Seeding;
using MagicMedia.Identity.Host.Tests.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using Moq;
using Squadron;
using Xunit;

namespace MagicMedia.Identity.Host.Tests
{
    public class IdentityTestServer : IAsyncLifetime
    {
        public HttpClient? HttpClient { get; private set; }

        public MongoResource? MongoResource { get; private set; }

        public IMongoDatabase? Database { get; private set; }

        public IServiceProvider Services { get; private set; }

        public async Task InitializeAsync()
        {
            MongoResource = new MongoResource();
            await MongoResource.InitializeAsync();

            Database = MongoResource.CreateDatabase();

            IWebHostBuilder hostBuilder = new WebHostBuilder()
                .ConfigureAppConfiguration(builder =>
                {
                    builder.SetBasePath(Directory.GetCurrentDirectory());
                    builder.AddJsonFile("appsettings.test.json", optional: true);
                    builder.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["Identity:Database:ConnectionString"] =
                        MongoResource.ConnectionString,
                        ["Identity:Database:DatabaseName"] =
                        Database.DatabaseNamespace.DatabaseName
                    });
                })
                .ConfigureTestServices(services =>
                {

                })
               .UseStartup<Startup>();

            var server = new TestServer(hostBuilder);

            Services = server.Services;

            IIdentityDbContext dbContext = server.Services.GetService<IIdentityDbContext>();

            var seeder = new DataSeeder(dbContext);
            await seeder.SeedIntialDataAsync(default);
            await seeder.AddClientsAsync(
                new List<MagicClient> { TestData.Client01CredentialClient },
                default);

            HttpClient = server.CreateClient();
        }

        public async Task<string> CreateTokenAsync(IEnumerable<Claim> claims)
        {
            ITokenCreationService? tokenService = Services.GetService<ITokenCreationService>();

            var tokenRequest = new Token
            {
                CreationTime = DateTime.UtcNow,
                Lifetime = 60,
                Type = OidcConstants.TokenTypes.AccessToken,
                AccessTokenType = AccessTokenType.Jwt,
                Issuer = "http://localhost",
                Claims = new HashSet<Claim>(claims, new ClaimComparer())
            };

            return await tokenService.CreateTokenAsync(tokenRequest);
        }

        public async Task DisposeAsync()
        {
            await MongoResource?.DisposeAsync();
        }
    }
}
