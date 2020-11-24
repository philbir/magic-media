using System;
using MagicMedia.Messaging.Consumers;
using MagicMedia.Messaging;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MagicMedia.Messaging
{
    public static class MessagingServiceCollectionExtensions
    {
        public static IServiceCollection AddMessaging(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            MessagingOptions options = configuration.GetSection("MagicMedia:Messaging")
                .Get<MessagingOptions>();

            services.AddMassTransit(s =>
            {
                s.AddConsumer<FaceUpdatedConsumer>();
                s.AddConsumer<MoveMediaConsumer>();
                s.AddConsumer<MediaOperationCompletedConsumer>();
                s.AddConsumer<MediaOperationRequestCompletedConsumer>();
                s.AddConsumer<FavoriteMediaToggledConsumer>();
                s.AddConsumer<PersonUpdatedConsumer>();
                s.AddConsumer<ItemsAddedToAlbumConsumer>();
                s.AddConsumer<RecycleMediaConsumer>();
                s.AddConsumer<UpdateMediaMetadataConsumer>();

                if (options.Transport == MessagingTransport.InMemory)
                {
                    s.AddBus(provider => Bus.Factory.CreateUsingInMemory(cfg => {
                       cfg.ReceiveEndpoint(e =>
                       {
                           e.ConfigureConsumers(provider);
                       });
                    }));
                }
                else
                {
                    s.AddBus(provider => Bus.Factory.CreateUsingAzureServiceBus(cfg =>
                    {
                        cfg.Host(options.ServiceBus.ConnectionString);
                        cfg.ReceiveEndpoint(options.ServiceBus.ReceiveQueueName, e =>
                        {
                            e.ConfigureConsumers(provider);
                        });
                    }));
                };
            });


            return services;
        }

        public static void ConfigureConsumers(
            this IReceiveEndpointConfigurator e,
            IServiceCollection services,
            IServiceProvider serviceProvider)
        {
            services.AddSingleton<FaceUpdatedConsumer>();
            

            e.Consumer<FaceUpdatedConsumer>(serviceProvider);
        }
    }
}
