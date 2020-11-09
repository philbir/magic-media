using System;
using MagicMedia.Operations;

namespace MagicMedia.Messaging
{
    public class MediaOperationStepExecutedMessage
    {
        public Guid OperationId { get; set; }

        public MediaOperationStep Step { get; set; }

        public MediaOperationStepResult Result { get; set; }
    }
}
