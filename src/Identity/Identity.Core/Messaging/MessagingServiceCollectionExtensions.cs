using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MagicMedia.Identity.Messaging;

public static class MessagingServiceCollectionExtensions
{
    public static IServiceCollection AddMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        MessagingOptions options = configuration.GetMessagingOptions();

        services.AddMassTransit(s =>
        {
            s.AddConsumers();
            s.ConfigureBus(options.ServiceBus?.QueueName, options);
        });

        return services;
    }


    private static MessagingOptions GetMessagingOptions(this IConfiguration configuration)
    {
        MessagingOptions options = configuration.GetSection("Identity:Messaging")
            .Get<MessagingOptions>();

        return options;
    }

    private static void ConfigureBus(
        this IServiceCollectionBusConfigurator busConfigurator,
        string? queueName,
        MessagingOptions options)
    {
        if (options.Transport == MessagingTransport.InMemory)
        {
            busConfigurator.AddBus(provider => Bus.Factory.CreateUsingInMemory(cfg =>
            {
                cfg.ReceiveEndpoint(e =>
                {
                    e.ConfigureConsumers(provider);
                });
            }));
        }
        else
        {
            busConfigurator.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(options.ServiceBus.Host, c =>
                {
                    c.Username(options.ServiceBus.Username);
                    c.Password(options.ServiceBus.Password);
                });

                cfg.ReceiveEndpoint(queueName, e =>
                {
                    e.ConfigureConsumers(provider);
                });
            }));
        };
    }

    private static void AddConsumers(this IServiceCollectionBusConfigurator busConfigurator)
    {
        busConfigurator.AddConsumer<InviteUserRequestedConsumer>();

    }
}
