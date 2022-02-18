using System;
using MagicMedia.Store;

namespace MagicMedia.Messaging;

public class FaceUpdatedMessage
{
    public Guid Id { get; set; }

    public string? Action { get; set; }

    public Guid? PersonId { get; set; }

    public FaceUpdatedMessage()
    {
    }

    public FaceUpdatedMessage(Guid id, string action)
    {
        Id = id;
        Action = action;
    }

    public FaceUpdatedMessage(Guid id, string action, Guid personId)
        : this(id, action)
    {
        PersonId = personId;
    }
}

public record NewAuditEventMessage(AuditEvent Event);
