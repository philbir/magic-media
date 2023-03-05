using MagicMedia;
using MagicMedia.Api;
using MagicMedia.Api.Security;
using MagicMedia.AspNetCore;
using MagicMedia.BingMaps;
using MagicMedia.Hubs;
using MagicMedia.Messaging;
using MagicMedia.Security;
using MagicMedia.Store.MongoDb;
using MagicMedia.Stores;
using MagicMedia.Telemetry;
using MassTransit;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

builder.Logging.ConfigureSerilog(builder.Configuration);

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
    .AddClientThumbprintServices()
    .AddApiMessaging();

builder.Services.AddMvc();
builder.Services.AddSignalR();

builder.Services.AddMagicAuthorization();
builder.Services.ConfigureSameSiteCookies();
builder.Services.AddAuthentication(builder.Environment, builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IUserContextFactory, ClaimsPrincipalUserContextFactory>();
builder.Services.AddMassTransitHostedService();
builder.Services.AddOpenTelemetry(builder.Configuration);

WebApplication app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGraphQL();
    endpoints.MapControllers();
    endpoints.MapHub<MediaHub>("/signalr");
});

app.Run();
