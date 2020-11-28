using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using IdentityModel;
using MagicMedia;
using MagicMedia.AzureAI;
using MagicMedia.BingMaps;
using MagicMedia.Face;
using MagicMedia.Store.MongoDb;
using MagicMedia.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Sample.Web
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
            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor |
                    ForwardedHeaders.XForwardedProto;
            });

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "oidc";
            })
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                    options.Cookie.Name = "media-sample";
                })
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = "http://localhost:5500";
                    options.RequireHttpsMetadata = false;

                    options.ClientSecret = "geCDNACu94a5DfZQ2Sm46DBjkSErAnNA";
                    options.ClientId = "Media.UI";
                    options.ResponseType = "code";

                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");

                    options.ClaimActions.MapAllExcept("iss", "nbf", "exp", "aud", "nonce", "iat", "c_hash");

                    options.SaveTokens = true;
                    options.Events = new OpenIdConnectEvents
                    {
                        OnRedirectToIdentityProvider = (ctx) =>
                        {
                            return Task.CompletedTask;
                        },
                        OnTicketReceived = (ctx) =>
                        {
                            return Task.CompletedTask;
                        }
                    };

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = JwtClaimTypes.Name,
                        RoleClaimType = JwtClaimTypes.Role,
                    };
                });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                options.OnAppendCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                options.OnDeleteCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            });

            services.AddCoreMediaServices(Configuration);
            services.AddFaceDetection(Configuration);
            BingMapsOptions bingOptions = Configuration.GetSection("MagicMedia:BingMaps")
                .Get<BingMapsOptions>();

            services.AddFileSystemStore(Configuration);

            //services.AddBingMaps(bingOptions);

            services.AddMongoDbStore(Configuration);

            services.AddSingleton<SampleService>();

            AzureAIOptions azureAi = Configuration.GetSection("MagicMedia:AzureAI")
                .Get<AzureAIOptions>();

            services.AddAzureAI(azureAi);
        }

        public void Configure(IApplicationBuilder app, SampleService sampleService)
        {
            sampleService.BuildSampleStore();

            app.UseForwardedHeaders();
            app.UseCookiePolicy();

            app.UseDeveloperExceptionPage();
 
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
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
