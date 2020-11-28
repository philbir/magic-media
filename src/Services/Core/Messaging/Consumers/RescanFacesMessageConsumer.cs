using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicMedia.Operations;
using MassTransit;

namespace MagicMedia.Messaging.Consumers
{
    public class RescanFacesMessageConsumer : IConsumer<RescanFacesMessage>
    {
        private readonly IRescanFacesHandler _rescanFacesHandler;

        public RescanFacesMessageConsumer(IRescanFacesHandler rescanFacesHandler)
        {
            _rescanFacesHandler = rescanFacesHandler;
        }

        public async Task Consume(ConsumeContext<RescanFacesMessage> context)
        {
            await _rescanFacesHandler.ExecuteAsync(context.Message, context.CancellationToken);
        }
    }
}
