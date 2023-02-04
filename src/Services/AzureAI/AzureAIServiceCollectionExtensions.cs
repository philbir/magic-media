using System;
using System.Net.Http;
using MagicMedia.AzureAI;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MagicMedia;

public static class AzureAIServiceCollectionExtensions
{
    public static IMagicMediaServerBuilder AddAzureAI(
        this IMagicMediaServerBuilder builder)
    {
        AzureAIOptions options = builder.Configuration
            .GetSection("MagicMedia:AzureAI")
            .Get<AzureAIOptions>();

        builder.Services.AddAzureAI(options);

        return builder;
    }

    private static IServiceCollection AddAzureAI(
        this IServiceCollection services,
        AzureAIOptions options)
    {
        services.AddSingleton<ICloudAIMediaAnalyser, AzureComputerVision>();
        Func<ComputerVisionClient> factory = () => CreateComputerVisionClient(options);
        services.AddSingleton(factory);

        return services;
    }

    private static ComputerVisionClient CreateComputerVisionClient(AzureAIOptions options)
    {
        ComputerVisionClient computerVision = new ComputerVisionClient(
          new ApiKeyServiceClientCredentials(options.SubscriptionKey),
          new DelegatingHandler[] { });

        computerVision.Endpoint = options.Endpoint;

        return computerVision;
    }
}
