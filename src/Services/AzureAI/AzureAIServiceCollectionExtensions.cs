using System;
using System.Net.Http;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Extensions.DependencyInjection;

namespace MagicMedia.AzureAI
{
    public static class AzureAIServiceCollectionExtensions
    {
        public static IServiceCollection AddAzureAI(
            this IServiceCollection services,
            AzureAIOptions options)
        {
            services.AddSingleton<AzureComputerVision>();
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

}
