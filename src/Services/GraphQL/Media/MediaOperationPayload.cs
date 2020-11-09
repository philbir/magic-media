using System;
using System.Collections.Generic;

namespace MagicMedia.GraphQL
{
    public class MediaOperationPayload : Payload
    {
        public Guid OperationId { get; }

        public MediaOperationPayload(Guid operationId)
        {
            OperationId = operationId;
        }

        public MediaOperationPayload(IReadOnlyList<UserError>? errors = null)
            : base(errors)
        {
        }
    }
}
