using Elastic.Apm.AspNetCore;
using Elastic.Apm.DiagnosticSource;
using MagicMedia.Api.Security;
using MagicMedia.AspNetCore;
using MagicMedia.BingMaps;
using MagicMedia.Hubs;
using MagicMedia.Messaging;
using MagicMedia.Security;
using MagicMedia.Store.MongoDb;
using MagicMedia.Stores;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace MagicMedia.Api
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMagicMediaServer(Configuration)
                .AddGraphQLServer()
                .AddBingMaps()
                .AddAzureAI()
                .AddMongoDbStore()
                .AddFileSystemStore()
                .AddApiMessaging();

            services.AddMvc();
            services.AddSignalR();

            services.AddAuthorization();
            services.ConfigureSameSiteCookies();
            services.AddAuthentication(_env, Configuration);
            services.AddHttpContextAccessor();

            services.AddSingleton<IUserContextFactory, ClaimsPrincipalUserContextFactory>();
        }

        public void Configure(
            IApplicationBuilder app,
            IBusControl busControl)
        {
            busControl.Start();

            app.UseDefaultForwardedHeaders();
            app.UseCookiePolicy();

            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                //app.UseElasticApm(Configuration,
                //    new HttpDiagnosticsSubscriber());
            }

            app.UseSerilogRequestLogging();

            app.UseCors();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<EnsureAuthenticatedMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL();
                endpoints.MapControllers();
                endpoints.MapHub<MediaHub>("/signalr");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501
                //spa.Options.SourcePath = "ClientApp";
            });
        }
    }
}
