using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicMedia.Messaging;
using MassTransit;

namespace MagicMedia.Massaging.Consumers
{
    public class FaceUpdatedConsumer : IConsumer<FaceUpdatedMessage>
    {
        public Task Consume(ConsumeContext<FaceUpdatedMessage> context)
        {
            return Task.CompletedTask;
        }
    }
}
