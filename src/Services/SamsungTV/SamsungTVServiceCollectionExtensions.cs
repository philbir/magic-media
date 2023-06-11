using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MagicMedia.SamsungTv;

namespace MagicMedia;

public static class SamsungTvServiceCollectionExtensions
{
    public static IMagicMediaServerBuilder AddSamsungTv(
        this IMagicMediaServerBuilder builder)
    {
        builder.Services.AddSamsungTv(builder.Configuration);

        return builder;
    }

    private static IServiceCollection AddSamsungTv(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        SamsungTvOptions options = configuration.GetSection("MagicMedia:SamsungTV")
            .Get<SamsungTvOptions>();

        services.AddSingleton<IDestinationExporter, SamsungTvExporter>();
        services.AddSingleton<ISamsungTvClientFactory, SamsungTvClientFactory>();
        services.AddSingleton(options);

        foreach (SamsungTvDevice device in options.Devices)
        {
            services.AddHttpClient(device.Name)
                .ConfigureHttpClient(c => c.BaseAddress = device.Address);
        }

        return services;
    }
}

public class SamsungTvOptions
{
    public IEnumerable<SamsungTvDevice> Devices { get; set; }
}
