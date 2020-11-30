using MagicMedia.AspNetCore;
using MagicMedia.BingMaps;
using MagicMedia.Hubs;
using MagicMedia.Messaging;
using MagicMedia.Store.MongoDb;
using MagicMedia.Stores;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace MagicMedia.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMagicMediaServer(Configuration)
                .AddGraphQLServer()
                .AddBingMaps()
                .AddMongoDbStore()
                .AddFileSystemStore()
                .AddApiMessaging();

            services.AddMvc();
            services.AddSignalR();

            services.ConfigureSameSiteCookies();
            services.AddAuthentication(Configuration);

            services.AddAuthorization(o => o.AddPolicy("Read", p =>
            {
                p.RequireClaim("scope", "api.magic.read");
            }));
        }

        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IBusControl busControl)
        {
            busControl.Start();

            app.UseDefaultForwardedHeaders();
            app.UseCookiePolicy();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.Use(async (context, next) =>
                {
                    if (!context.User.Identity.IsAuthenticated)
                    {
                        await context.ChallengeAsync();
                    }
                    else
                    {
                        await next();
                    }
                });
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseSerilogRequestLogging();

            app.UseCors();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

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
