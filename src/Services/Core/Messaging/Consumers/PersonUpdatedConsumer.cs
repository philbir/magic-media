using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MassTransit;

namespace MagicMedia.Messaging.Consumers
{
    public class PersonUpdatedConsumer : IConsumer<PersonUpdatedMessage>
    {
        private readonly IAgeOperationsService _ageOperationsService;

        public PersonUpdatedConsumer(IAgeOperationsService ageOperationsService)
        {
            _ageOperationsService = ageOperationsService;
        }

        public async Task Consume(ConsumeContext<PersonUpdatedMessage> context)
        {
            await _ageOperationsService.UpdateAgesByPersonAsync(
                context.Message.Id,
                context.CancellationToken);
        }
    }
}
