using Api.Host.GraphQL;
using HotChocolate;
using MagicMedia.BingMaps;
using MagicMedia.Store.MongoDb;
using MagicMedia.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Extensions.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                .AddQueryType<Query>();

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
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL();
                //endpoints.MapControllerRoute(
                //    name: "default",
                //    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
