using MagicMedia;
using MagicMedia.Api;
using MagicMedia.Api.Security;
using MagicMedia.BingMaps;
using MagicMedia.Hubs;
using MagicMedia.Messaging;
using MagicMedia.Security;
using MagicMedia.Store.MongoDb;
using MagicMedia.Stores;
using MagicMedia.Telemetry;
using MassTransit;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.ConfigureOpenTelemetry();

builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>(optional: true)
    .AddEnvironmentVariables();

builder.Services
    .AddMagicMediaServer(builder.Configuration)
    .AddGraphQLServer()
    .AddBingMaps()
    .AddAzureAI()
    .AddMongoDbStore()
    .AddFileSystemStore()
    .AddSamsungTv()
    .AddClientThumbprintServices()
    .AddApiMessaging();

builder.Services.AddMvc();
builder.Services.AddSignalR();

builder.Services.AddMagicAuthorization();
builder.Services.AddAuthentication(builder.Environment, builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IUserContextFactory, ClaimsPrincipalUserContextFactory>();
builder.Services.AddMassTransitHostedService();

WebApplication app = builder.Build();

app.UseDefaultForwardedHeaders();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGraphQL();
    endpoints.MapControllers();
    endpoints.MapHub<MediaHub>("/signalr");
});

app.MapFallbackToFile("index.html");

app.Run();
