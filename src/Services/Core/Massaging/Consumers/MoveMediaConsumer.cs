using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicMedia.Operations;
using MassTransit;

namespace MagicMedia.Massaging.Consumers
{
    public class MoveMediaConsumer : IConsumer<MoveMediaRequest>
    {
        public Task Consume(ConsumeContext<MoveMediaRequest> context)
        {
            throw new NotImplementedException();
        }
    }
}
