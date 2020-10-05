using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace MagicMedia.Face
{
    public static class FaceDetectionServiceCollectionExtensions
    {
        public static IServiceCollection AddFaceDetection(this IServiceCollection services)
        {
            services.AddHttpClient("Face", c =>
            {
                c.BaseAddress = new Uri("http://localhost:5001");
            });

            services.AddSingleton<IFaceDetectionService, FaceDetectionService>();
            services.AddSingleton<IFaceModelBuilderService, FaceModelBuilderService>();

            return services;
        }
    }
}
