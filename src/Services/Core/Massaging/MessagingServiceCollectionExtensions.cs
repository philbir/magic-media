using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MagicMedia.Massaging.Consumers;
using MagicMedia.Messaging;
using MassTransit;
using MassTransit.Azure.ServiceBus.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MagicMedia.Massaging
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

                if (options.Transport == MessagingTransport.InMemory)
                {
                    s.AddBus(provider => Bus.Factory.CreateUsingInMemory(cfg => {
                       cfg.ReceiveEndpoint(e =>
                       {
                           e.ConfigureConsumers();
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
                            e.ConfigureConsumers();
                        });
                    }));
                };
            });


            return services;
        }

        public static void ConfigureConsumers(this IReceiveEndpointConfigurator e)
        {
            e.Consumer<FaceUpdatedConsumer>();
        }
    }
}
