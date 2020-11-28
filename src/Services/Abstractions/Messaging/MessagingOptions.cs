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
        public string Host { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string WorkerQueueName { get; set; }

        public string ApiQueueName { get; set; }
    }
}
