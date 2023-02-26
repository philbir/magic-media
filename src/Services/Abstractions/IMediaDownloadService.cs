using System;
using System.Threading;
using System.Threading.Tasks;

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
        Guid? profileId,
        CancellationToken cancellationToken);
}

public class MediaExportResult
{
    public string Path { get; set; }
}

public class MediaExportProfile
{
    public ExportLocation Location { get; set; }

    public ImageSize Size { get; set; }
}

public class ImageSize
{
    public int Width { get; set; }
    public int Height { get; set; }
}

public class ExportLocation
{
    public string Path { get; set; }
}
