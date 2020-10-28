using HotChocolate;
using MagicMedia.GraphQL;
using MagicMedia.GraphQL.DataLoaders;
using MagicMedia.GraphQL.Face;
using MagicMedia.BingMaps;
using MagicMedia.Store.MongoDb;
using MagicMedia.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Extensions.Context;
using MagicMedia.Massaging;
using MassTransit;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Threading.Tasks;

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

            services.AddFileSystemStore(@"C:\MagicMedia");
            services.AddMagicMedia();
            services.AddBingMaps(bingOptions);
            services.AddMessaging(Configuration);
            services.AddMvc();
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder => {
                            builder
                                .WithOrigins("http://localhost:8080")
                                .WithOrigins("http://localhost:8081")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                            });
            });

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
        }
    }
}
