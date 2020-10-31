using System;
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

            services.AddSingleton<Seeder>();

            services.AddSingleton<ISampleDataSource, UnsplashRandomDataSource>();

            return services;
        }
    }
}
