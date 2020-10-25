using MagicMedia.Identity.Data.Mongo;
using MagicMedia.Identity.Data.Mongo.Seeding;
using MagicMedia.Identity.Services.Sms;
using MagicMedia.Identity.SignUp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagicMedia.Identity
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private void CheckSameSite(HttpContext httpContext, CookieOptions options)
        {
            if (options.SameSite == SameSiteMode.None)
            {
                var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
                // TODO: Use your User Agent library of choice here.
                if (true)
                {
                    options.SameSite = SameSiteMode.Unspecified;
                }
            }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            IIdentityServerBuilder builder = services.AddIdentityServer(Configuration)
                .AddTestUsers(TestUsers.Users);

            builder.AddDeveloperSigningCredential();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                options.OnAppendCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                options.OnDeleteCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            });

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor |
                    ForwardedHeaders.XForwardedProto;
            });

            services.AddDataAccess(Configuration);

            services.AddECallSms(Configuration);
            services.AddSingleton<SignUpService>();

            services.AddControllersWithViews();
        }

        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            DataSeeder dataSeeder)
        {
            //TODO: Move to hosted service
            dataSeeder.SeedIntialDataAsync(default).GetAwaiter().GetResult();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseForwardedHeaders();
            app.UseCookiePolicy();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseIdentityServer();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
