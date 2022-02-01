using System;
using System.Net.Http;
using MagicMedia.Identity.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MagicMedia.Identity.Services.Sms;

public static class SmsServiceCollectionExtensions
{
    public static IServiceCollection AddECallSms(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ECallOptions options = configuration.GetSection("ECall").Get<ECallOptions>();

        services.AddHttpClient("ECall", c =>
        {
            c.BaseAddress = new Uri(options.Url);
        });

        services.AddSingleton<ISmsService>(c =>
            new ECallSmsService(c.GetService<IHttpClientFactory>(), options));

        return services;
    }
}
