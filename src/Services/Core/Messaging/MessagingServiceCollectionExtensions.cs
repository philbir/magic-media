using MagicMedia.Messaging.Consumers;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.Configuration;

namespace MagicMedia.Messaging
{
    public static class MessagingServiceCollectionExtensions
    {
        public static IMagicMediaServerBuilder AddWorkerMessaging(
            this IMagicMediaServerBuilder builder)
        {
            MessagingOptions options = builder.GetOptions();

            builder.Services.AddMassTransit(s =>
            {
                s.AddWorkerConsumers();
                s.ConfigureBus(options.ServiceBus?.WorkerQueueName, options);
            });

            return builder;
        }

        public static IMagicMediaServerBuilder AddApiMessaging(
            this IMagicMediaServerBuilder builder)
        {
            MessagingOptions options = builder.GetOptions();

            builder.Services.AddMassTransit(s =>
            {
                s.AddApiConsumers();
                s.ConfigureBus(options.ServiceBus?.ApiQueueName, options);
            });

            return builder;
        }

        private static MessagingOptions GetOptions(this IMagicMediaServerBuilder builder)
        {
            MessagingOptions options = builder.Configuration.GetSection("MagicMedia:Messaging")
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

        private static void AddWorkerConsumers(this IServiceCollectionBusConfigurator busConfigurator)
        {
            busConfigurator.AddConsumer<FaceUpdatedConsumer>();
            busConfigurator.AddConsumer<MoveMediaConsumer>();
            busConfigurator.AddConsumer<FavoriteMediaToggledConsumer>();
            busConfigurator.AddConsumer<PersonUpdatedConsumer>();
            busConfigurator.AddConsumer<ItemsAddedToAlbumConsumer>();
            busConfigurator.AddConsumer<RecycleMediaConsumer>();
            busConfigurator.AddConsumer<UpdateMediaMetadataConsumer>();
            busConfigurator.AddConsumer<NewMediaAddedConsumer>();
            busConfigurator.AddConsumer<RescanFacesMessageConsumer>();
        }

        private static void AddApiConsumers(this IServiceCollectionBusConfigurator busConfigurator)
        {
            busConfigurator.AddConsumer<MediaOperationCompletedConsumer>();
            busConfigurator.AddConsumer<MediaOperationRequestCompletedConsumer>();
        }
    }
}
