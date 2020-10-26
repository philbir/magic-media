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
            services.AddGraphQLServer()
                .AddMagicMediaGrapQL();

            BingMapsOptions bingOptions = Configuration.GetSection("MagicMedia:BingMaps")
                .Get<BingMapsOptions>();

            services.AddMongoDbStore(new MongoOptions
            {
                DatabaseName = "magic",
                ConnectionString = "mongodb://localhost:27017"
            });

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
            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL();
                endpoints.MapControllers();
                //endpoints.MapControllerRoute(
                //    name: "default",
                //    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
