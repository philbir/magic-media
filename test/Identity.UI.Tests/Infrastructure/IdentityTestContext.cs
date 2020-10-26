using System;
using System.IO;
using System.Threading.Tasks;
using Identity.UI.Tests.Container;
using MagicMedia.Identity.Data;
using MagicMedia.Identity.Data.Mongo;
using Magnet.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Extensions.Context;
using OpenQA.Selenium;
using Squadron;
using Xunit;

namespace MagicMedia.Identity.UI.Tests
{
    public class IdentityTestContext : IAsyncLifetime
    {
        public IConfiguration Configuration { get; set; }

        public IServiceProvider ServiceProvider { get; set; }

        internal MagnetClient _magnetClient;

        public IdentityTestOptions Options { get; internal set; }

        public ComposeResource<IdentityAppOptions> Containers { get; private set; }
        public string SeleniumHubUrl { get; private set; }
        public string HostUrl { get; private set; }
        public MessageReceiver MagnetSession { get; private set; }

        public IIdentityDbContext GetDbContext()
        {
            MongoResource resource = Containers
                .GetResource<MongoResource>("mongo");

            return new IdentityDbContext(new MongoOptions
            {
                ConnectionString = resource.ConnectionString,
                DatabaseName = "magic-identity"
            });
        }


        public async Task InitializeAsync()
        {
            Configuration = BuildConfiguration();

            Options = Configuration
                .GetSection("IdentityTests")
                .Get<IdentityTestOptions>();

            var services = new ServiceCollection();
            services.AddMagnet()
                .UseHttp(Options.Magnet.Url)
                .WithClientName(
                    $"{Options.Magnet.Name}-{Environment.MachineName}");

            ServiceProvider = services.BuildServiceProvider();
            _magnetClient = ServiceProvider.GetService<MagnetClient>();

            Containers = new ComposeResource<IdentityAppOptions>();

            await Containers.InitializeAsync();

            GenericContainerResource<IdentityHostOptions> host = Containers
                .GetResource<GenericContainerResource<IdentityHostOptions>>("host");

            if (Options.Driver.Mode == SeleniumDriverMode.Remote)
            {
                GenericContainerResource<SelemiumFirefoxServerOptions> selenium = Containers
                    .GetResource<GenericContainerResource<SelemiumFirefoxServerOptions>>("selenium");

                SeleniumHubUrl = selenium.GetContainerUri().ToString();
                HostUrl = host.GetInternalContainerUri().ToString();
            }
            else
            {
                HostUrl = host.GetContainerUri().ToString();
            }
        }

        public IWebDriver CreateDriver()
        {
            return WebDriverFactory.Build(
                Options.Driver.Mode,
                Options.Driver.Browser,
                SeleniumHubUrl);
        }

        public static IConfiguration BuildConfiguration()
        {
            var currentDir = Directory.GetCurrentDirectory();

            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(currentDir)
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.user.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            return config;
        }

        public async Task DisposeAsync()
        {
            await Containers.DisposeAsync();
        }

        public TApp CreateApp<TApp>()
            where TApp : IdentityApp, new()
        {
            var app = new TApp();
            app.TestContext = this;
            app.Driver = CreateDriver();
            return app;
        }

        public async Task StartMagnetAsync()
        {
            MagnetSession = await _magnetClient.StartAsync();
        }
    }
}
