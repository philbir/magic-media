using System;
using MagicMedia.SampleDataSeader;
using Microsoft.Extensions.DependencyInjection;

namespace SampleDataSeeder
{
    public static class SampleDataSeaderServiceCollectionExtensions
    {
        public static IServiceCollection AddDataSeaders(this IServiceCollection services)
        {
            services.AddHttpClient("Unsplash", c =>
            {
                c.BaseAddress = new Uri("https://source.unsplash.com/random/");
            });

            services.AddHttpClient("MagicMedia", c =>
            {
                c.BaseAddress = new Uri("http://192.168.0.77:7780/api/");
            });

            services.AddSingleton<ApiSeeder>();
            services.AddSingleton<Seeder>();

            services.AddSingleton<ISampleDataSource, UnsplashRandomDataSource>();

            return services;
        }
    }
}
