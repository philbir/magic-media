namespace MagicMedia.Identity.Messaging;

public class MessagingOptions
{
    public MessagingTransport Transport { get; set; }

    public ServiceBusOptions? ServiceBus { get; set; }
}

public enum MessagingTransport
{
    InMemory,
    RabbitMQ
}

public class ServiceBusOptions
{
    public string? Host { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? QueueName { get; set; }
}
