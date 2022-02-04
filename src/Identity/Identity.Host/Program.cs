
using System.IdentityModel.Tokens.Jwt;
using MagicMedia.AspNetCore;
using MagicMedia.Identity;
using MagicMedia.Identity.Data.Mongo;
using MagicMedia.Identity.Data.Mongo.Seeding;
using MagicMedia.Identity.Messaging;
using MagicMedia.Identity.Services;
using MagicMedia.Telemetry;
using MassTransit;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>(optional: true)
    .AddJsonFile("appsettings.local.json", optional: true)
    .AddEnvironmentVariables();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

IIdentityServerBuilder idBuilder = builder.Services
    .AddIdentityServer(builder.Configuration, builder.Environment);

idBuilder.AddDeveloperSigningCredential();

builder.Services.AddDataAccess(builder.Configuration);
builder.Services.AddIdentityCore(builder.Configuration);

builder.Services.ConfigureSameSiteCookies();

builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

builder.Services.AddMessaging(builder.Configuration);
builder.Services.AddMassTransitHostedService();
builder.Services.AddOpenTelemetry("Magic-Identity");

builder.Services.AddSingleton<IDemoUserService>(s => new DemoUserService(
    builder.Environment.IsDemo(),
    builder.Configuration));

WebApplication app = builder.Build();
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

if (builder.Environment.IsDevelopment() || builder.Environment.IsDemo())
{
    await app.Services.GetRequiredService<DataSeeder>()
        .SeedIntialDataAsync(default);
}

app.UseDefaultForwardedHeaders();
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

app.Run();
