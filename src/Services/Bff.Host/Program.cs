using System.Diagnostics;
using Duende.Bff.Yarp;
using MagicMedia.AspNetCore;
using MagicMedia.Bff;
using Microsoft.AspNetCore.DataProtection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

BffOptions bffOptions = builder.Configuration.GetSection("MagicMedia:Bff")
    .Get<BffOptions>();

builder.Services.AddBff()
    .AddServerSideSessions()
    .AddRemoteApis();

IReverseProxyBuilder proxyBuilder = builder.Services.AddReverseProxy()
    .AddBffExtensions();

if (bffOptions.YarpConfigSectionName is { })
{
    proxyBuilder.LoadFromConfig(builder.Configuration.GetSection(bffOptions.YarpConfigSectionName));
}
else
{
    proxyBuilder.LoadFromBffOptions(bffOptions);
}

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(bffOptions.DataProtectionKeysDirectory));

builder.Services.AddAuthorization();

builder.Services.AddAuthentication(builder.Environment, builder.Configuration);

var app = builder.Build();

app.UseDefaultForwardedHeaders();
app.UseCookiePolicy();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseBff();
app.UseAuthorization();

app.MapBffManagementEndpoints();
app.MapReverseProxy(proxyApp =>
{
    proxyApp.UseAntiforgeryCheck();
});

app.MapDevelopmentHandler();
app.MapFallbackToFile("index.html");

app.Run();
