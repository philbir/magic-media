using System;
using System.Collections.Generic;

namespace MagicMedia.Operations
{
    public class MoveMediaRequest
    {
        public IEnumerable<Guid> Ids { get; set; }

        public string NewLocation { get; set; }
        public string? OperationId { get; set; }
    }
}
