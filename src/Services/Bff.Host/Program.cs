using System.Diagnostics;
using Duende.Bff.Yarp;
using MagicMedia.AspNetCore;
using MagicMedia.Bff;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBff()
    .AddServerSideSessions()
    .AddRemoteApis();

builder.Services.AddReverseProxy()
    .AddBffExtensions()
    .LoadFromConfig(builder.Configuration.GetSection("MagicMedia:Bff:Yarp"));

BffOptions bffOptions = builder.Configuration.GetSection("MagicMedia:Bff")
    .Get<BffOptions>();

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

app.Run();
