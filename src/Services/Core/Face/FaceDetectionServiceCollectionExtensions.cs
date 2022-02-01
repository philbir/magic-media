using System;
using System.Net.Http;
using MagicMedia.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace MagicMedia.Face;

public static class FaceDetectionServiceCollectionExtensions
{
    public static IServiceCollection AddFaceDetection(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        FaceOptions? faceOptions = configuration.GetSection("MagicMedia:Face")
            .Get<FaceOptions>();

        AsyncRetryPolicy<HttpResponseMessage>? retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(retryAttempt));

        services.AddHttpClient("Face", c =>
        {
            c.BaseAddress = faceOptions.Url;
        }).AddPolicyHandler(retryPolicy);

        services.AddSingleton<IFaceDetectionService, FaceDetectionService>();
        services.AddSingleton<IFaceModelBuilderService, FaceModelBuilderService>();

        return services;
    }
}
