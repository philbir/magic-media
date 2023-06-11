using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;

namespace MagicMedia;

public interface IMediaDownloadService
{
    Task<MediaDownload> CreateDownloadAsync(
        Guid id,
        DownloadMediaOptions options,
        CancellationToken cancellationToken);
    Task<MediaDownload> CreateDownloadAsync(Guid id, string profile, CancellationToken cancellationToken);
}

public interface IMediaExportService
{
    Task<MediaExportResult> ExportAsync(
        Guid id,
        MediaExportOptions options,
        CancellationToken cancellationToken);
}

public interface IDestinationExporter
{
    ExportDestinationType CanHandleType { get; }
    Task<string> ExportAsync(
        TransformedMedia media,
        ExportDestination destination,
        MediaExportOptions options,
        CancellationToken cancellationToken);
}

public class MediaExportOptions
{
    public Guid? ProfileId { get; set; }
    public string? Path { get; set; }
}

public interface IMediaTransformService
{
    Task<TransformedMedia> TransformAsync(
        Guid id,
        MediaTransform transform,
        CancellationToken cancellationToken);
}

public class MediaExportResult
{
    public string Path { get; set; }
}

public class MediaExportProfile
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public IEnumerable<ExportDestination> Destinations { get; set; }

    public MediaTransform? Transform { get; set; }

    public bool IsDefault { get; set; }
}

public class MediaTransform
{
    public Resize? Resize { get; set; }

    public bool RemoveMetadata { get; set; }

    public string? Format { get; set; }

    public int Quality { get; set; }
}

public class Resize
{
    public MediaSize? Size { get; set; }

    public string Mode { get; set; }
}

public class MediaSize
{
    public int Width { get; set; }
    public int Height { get; set; }
}

public class ExportDestination
{
    public ExportDestinationType Type { get; set; }

    public string Name { get; set; }

    public IList<ExportDestinationOption> Options { get; set; } = new List<ExportDestinationOption>();
}

public class ExportDestinationOption
{
    public string Name { get; set; }

    public string Value { get; set; }
}

public enum ExportDestinationType
{
    FileSystem,
    SamsungTvArt
}

public class TransformedMedia
{
    public Media Media { get; set; }

    public byte[] Data { get; set; }

    public MediaSize Size { get; set; }

    public string Format { get; set; }
}
