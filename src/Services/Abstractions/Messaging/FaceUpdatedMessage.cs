using System;

namespace MagicMedia.Messaging
{
    public record FaceUpdatedMessage(Guid Id, string Action)
    {
        public Guid PersonId { get; init; }
    }
}
