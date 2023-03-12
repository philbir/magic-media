using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Configuration;
using MagicMedia.Extensions;
using MagicMedia.Store;

namespace MagicMedia;

public class MediaConsistencyService : IMediaConsistencyService
{
    private readonly IMediaService _mediaService;
    private readonly IFileSystemSnapshotService _fileSystemSnapshotService;
    private readonly IMediaRepairService _mediaRepairService;
    private readonly FileSystemStoreOptions _fileSystemStoreOptions;

    public MediaConsistencyService(
        IMediaService mediaService,
        IFileSystemSnapshotService fileSystemSnapshotService,
        IMediaRepairService mediaRepairService,
        FileSystemStoreOptions fileSystemStoreOptions)
    {
        _mediaService = mediaService;
        _fileSystemSnapshotService = fileSystemSnapshotService;
        _mediaRepairService = mediaRepairService;
        _fileSystemStoreOptions = fileSystemStoreOptions;
    }

    public async Task<ConsistencyReport> GetReportAsync(Media media, CancellationToken cancellationToken)
    {
        var report = new ConsistencyReport();
        report.Checks.AddRange(CheckMediaFiles(media));

        //Check for repairs
        foreach (ConsistencyCheck check in report.Checks.Where(x => !x.Success))
        {
            await _mediaRepairService.ApplyRepairsAsync(media, check, cancellationToken);
        }

        return report;
    }

    private IReadOnlyList<ConsistencyCheck> CheckMediaFiles(
        Media media)
    {
        var checks = new List<ConsistencyCheck>();

        IEnumerable<MediaFileInfo> files = _mediaService.GetMediaFiles(media);

        checks.Add(CreateFileCheck(files, MediaFileType.Original));
        checks.Add(CreateFileCheck(files, MediaFileType.WebPreview));

        return checks;
    }

    private ConsistencyCheck CreateFileCheck(IEnumerable<MediaFileInfo> files, MediaFileType type)
    {
        MediaFileInfo? file = files.FirstOrDefault(x => x.Type == type);

        var check = new ConsistencyCheck
        {
            Name = "File_" + type
        };

        if (file is { })
        {
            check.Success = file.Exists;
            check.Data = new()
            {
                new("Path", Path.Combine(file.Location, file.Filename))
            };
        }

        return check;
    }
}
