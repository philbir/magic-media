using System;
using System.Collections.Generic;

namespace MagicMedia.Messaging;

public class MediaOperationCompletedMessage
{
    public MediaOperationType Type { get; set; }

    public string? OperationId { get; set; }

    public Guid MediaId { get; set; }

    public Dictionary<string, string>? Data { get; set; }

    public bool IsSuccess { get; set; }

    public string? Message { get; set; }
}


public class MediaOperationRequestCompletedMessage
{
    public MediaOperationType Type { get; set; }

    public string? OperationId { get; set; }

    public int ErrorCount { get; set; }

    public int SuccessCount { get; set; }
}

public enum MediaOperationType
{
    Move,
    Recycle,
    UpdateMetadata,
    RescanFaces,
    Delete
}
