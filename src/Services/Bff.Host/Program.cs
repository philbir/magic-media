using Duende.Bff.Yarp;
using MagicMedia.Bff;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBff()
    .AddServerSideSessions()
    .AddRemoteApis();

BffOptions bffOptions = builder.Configuration.GetSection("MagicMedia:Bff")
    .Get<BffOptions>();

builder.Services.AddAuthorization();

builder.Services.AddAuthentication(builder.Environment, builder.Configuration);

builder.Services.AddUserAccessTokenHttpClient("api_client", configureClient: client =>
{
    client.BaseAddress = new Uri(bffOptions.ApiUrl);
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseBff();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapBffManagementEndpoints();

    IEndpointConventionBuilder apiEndpoint = endpoints.MapRemoteBffApiEndpoint("/api", $"{bffOptions.ApiUrl}api/")
        .RequireAccessToken(Duende.Bff.TokenType.User);

    IEndpointConventionBuilder gqlEndpint = endpoints.MapRemoteBffApiEndpoint("/graphql", $"{bffOptions.ApiUrl}graphql/")
        .RequireAccessToken(Duende.Bff.TokenType.User)
        .SkipAntiforgery();

    IEndpointConventionBuilder signalREndpoint = endpoints.MapRemoteBffApiEndpoint("/signalr", $"{bffOptions.ApiUrl}signalr/")
        .SkipAntiforgery();

    if (bffOptions.DisableAntiforgery)
    {
        apiEndpoint.SkipAntiforgery();
        gqlEndpint.SkipAntiforgery();
        signalREndpoint.SkipAntiforgery();
    }
});

app.Run();
