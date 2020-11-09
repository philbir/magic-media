using System;
using MagicMedia.Operations;

namespace MagicMedia.Messaging
{
    public class NewMediaOperationTaskMessage
    {
        public Guid OperationId { get; set; }

        public MediaOperationTask Task { get; set; }
    }
}
