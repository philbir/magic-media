using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicMedia.Messaging;
using MagicMedia.Operations;
using MassTransit;

namespace MagicMedia.Massaging.Consumers
{
    public class MoveMediaConsumer : IConsumer<MoveMediaMessage>
    {
        private readonly IMoveMediaHandler _moveMediaHandler;

        public MoveMediaConsumer(IMoveMediaHandler moveMediaHandler)
        {
            _moveMediaHandler = moveMediaHandler;
        }

        public async Task Consume(ConsumeContext<MoveMediaMessage> context)
        {
            await _moveMediaHandler.ExecuteAsync(
                context.Message,
                context.CancellationToken);
        }
    }
}
