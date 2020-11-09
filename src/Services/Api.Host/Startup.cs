using MagicMedia.BingMaps;
using MagicMedia.Massaging;
using MagicMedia.Store.MongoDb;
using MagicMedia.Stores;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            services.AddGraphQLServer()
                .AddMagicMediaGrapQL();

            BingMapsOptions bingOptions = Configuration.GetSection("MagicMedia:BingMaps")
                .Get<BingMapsOptions>();

            SecurityOptions secOptions = Configuration.GetSection("MagicMedia:Security")
                .Get<SecurityOptions>();

            services.AddMongoDbStore(Configuration);

            services.AddFileSystemStore(Configuration);
            services.AddMagicMedia(Configuration);
            services.AddBingMaps(bingOptions);
            services.AddMessaging(Configuration);
            services.AddMvc();

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

            app.UseWebSockets();
            app.UseCors();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL();
                endpoints.MapControllers();
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
