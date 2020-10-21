using HotChocolate;
using MagicMedia.Api.GraphQL;
using MagicMedia.Api.GraphQL.DataLoaders;
using MagicMedia.Api.GraphQL.Face;
using MagicMedia.BingMaps;
using MagicMedia.Store.MongoDb;
using MagicMedia.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Extensions.Context;

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
                    .AddQueryType(d => d.Name("Query"))
                        .AddType<MediaQueries>()
                        .AddType<FaceQueries>()
                     .AddMutationType(d => d.Name("Mutation"))
                        .AddType<FaceMutations>()
                    .AddType<MediaType>()
                    .AddType<FaceType>()
                    .AddType<ThumbnailType>()
                    .AddDataLoader<CameraByIdDataLoader>()
                    .AddDataLoader<ThumbnailByMediaIdDataLoader>();

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

            services.AddMvc();
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder => {
                            builder
                                .WithOrigins("http://localhost:8080")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                            });
            });
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
