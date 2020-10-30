using System;
using System.Collections.Generic;
using System.Text;
using MagicMedia.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MagicMedia.Face
{
    public static class FaceDetectionServiceCollectionExtensions
    {
        public static IServiceCollection AddFaceDetection(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            FaceOptions? faceOptions = configuration.GetSection("MagicMedia:Face")
                .Get<FaceOptions>();

            services.AddHttpClient("Face", c =>
            {
                c.BaseAddress = faceOptions.Url;
            });

            services.AddSingleton<IFaceDetectionService, FaceDetectionService>();
            services.AddSingleton<IFaceModelBuilderService, FaceModelBuilderService>();

            return services;
        }
    }
}
