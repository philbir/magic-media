using System;

namespace MagicMedia.Store;

public class SimilarMediaInfo
{
    public string? Id { get; set; }

    public Guid SourceMediaId { get; set; }

    public Guid TargetMediaId { get; set; }

    public MediaHashType CompareType { get; set; }

    public double Similarity { get; set; }

    public DateTime CreatedAt { get; set; }
}
