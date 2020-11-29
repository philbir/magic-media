using MagicMedia.BingMaps;
using MagicMedia.Configuration;
using MagicMedia.Hubs;
using MagicMedia.Messaging;
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
    public class SecurityOptions
    {
        public string Authority { get; set; }

    }
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

            SecurityOptions secOptions = Configuration.GetSection("MagicMedia:Security")
                .Get<SecurityOptions>();

            services.AddMvc();
            services.AddSignalR();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "jwt";
            })
            .AddJwtBearer("jwt", options =>
            {
                options.RequireHttpsMetadata = false;
                options.Authority = secOptions.Authority;
                options.Audience = "api.magic";
            });

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

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
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
