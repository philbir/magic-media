using MagicMedia.AzureAI;
using MagicMedia.Face;
using MagicMedia.Thumbnail;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Web
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
            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();
            services.AddThumbnailService();
            services.AddFaceDetection();
            services.AddSingleton<SampleService>();

            AzureAIOptions azureAi = Configuration.GetSection("MagicMedia:AzureAI")
                .Get<AzureAIOptions>();

            services.AddAzureAI(azureAi);
        }

        public void Configure(IApplicationBuilder app, SampleService sampleService)
        {
            sampleService.BuildSampleStore();

            app.UseDeveloperExceptionPage();
 
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=media}/{action=Index}/{id?}");
            });
        }
    }
}
