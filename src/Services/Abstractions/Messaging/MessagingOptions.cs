using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicMedia.Messaging
{
    public class MessagingOptions
    {
        public MessagingTransport Transport { get; set; }

        public ServiceBusOptions ServiceBus { get; set; }

    }

    public enum MessagingTransport
    {
        InMemory,
        AzureServiceBus,
        RabbitMQ
    }

    public class ServiceBusOptions
    {
        public string ConnectionString { get; set; }

        public string ReceiveQueueName { get; set; }
    }
}
