using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia;

public interface IMediaConsistencyService
{
    Task<ConsistencyReport> GetReportAsync(Media media, CancellationToken cancellationToken);
}

public class ConsistencyReport
{
    public List<ConsistencyCheck> Checks { get; set; } = new List<ConsistencyCheck>();
}

public class ConsistencyCheck
{
    public string Name { get; set; }

    public bool Success { get; set; }

    public List<ConsistencyCheckData>? Data { get; set; } = new();
    public List<MediaRepair> Repairs { get; set; } = new();
}

public record ConsistencyCheckData(string Name, string Value);

public class MediaRepair
{
    public string Type { get; set; }

    public string Title { get; set; }

    public List<MediaRepairParameter> Parameters { get; set; } = new();
}

public record MediaRepairParameter(string Name, string Value)
{
    public bool AddToAction { get; init; }
}

public class RepairMediaRequest
{
    public Guid MediaId { get; set; }

    public string Type { get; set; }

    public List<MediaRepairParameter> Parameters { get; set; }
}
