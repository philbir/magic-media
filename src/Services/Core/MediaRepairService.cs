using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Configuration;
using MagicMedia.Extensions;
using MagicMedia.Store;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace MagicMedia;

public class MediaRepairService : IMediaRepairService
{
    private readonly IFileSystemSnapshotService _snapshotService;
    private readonly FileSystemStoreOptions _fileSystemStoreOptions;

    public MediaRepairService(
        IFileSystemSnapshotService snapshotService,
        FileSystemStoreOptions fileSystemStoreOptions)
    {
        _snapshotService = snapshotService;
        _fileSystemStoreOptions = fileSystemStoreOptions;
    }

    public async Task ApplyRepairsAsync(Media media, ConsistencyCheck check, CancellationToken cancellationToken)
    {
        switch (check.Name)
        {
            case "File_Original":
                await ApplyOriginalFileMissingRepairAsync(media, check, cancellationToken);
                break;
        }
    }

    private async Task ApplyOriginalFileMissingRepairAsync(Media media, ConsistencyCheck check,
        CancellationToken cancellationToken)
    {
        FileSystemSnapshot snapshot = await _snapshotService.LoadAsync(cancellationToken);

        IEnumerable<MediaFileEntry> files = snapshot.Entries.Where(x => x.Name == media.Filename);

        foreach (MediaFileEntry file in files)
        {
            var repair = new MediaRepair { Type = "UpdateFolder", Title = "Update media folder" };
            var repairMove = new MediaRepair { Type = "MoveFile", Title = "Move file" };

            repair.Parameters.Add(new(
                "Resolved_Path",
                Path.Combine(file.Folder, file.Name)) { AddToAction = true });

            var fileName = Path.Combine(_fileSystemStoreOptions.RootDirectory + file.Folder, file.Name);
            var dataUrl = await GetPreviewDataUrlAsync(fileName, cancellationToken);

            repair.Parameters.Add(new(
                "Found_Image",
                dataUrl));

            repairMove.Parameters.AddRange(repair.Parameters.ToList());

            check.Repairs.Add(repair);
            check.Repairs.Add(repairMove);
        }
    }

    private async Task<string> GetPreviewDataUrlAsync(string fileName, CancellationToken cancellationToken)
    {
        Image? image = await Image.LoadAsync(fileName, cancellationToken);
        if (image.Width > 250)
        {
            image.Mutate(m => m.Resize(new Size(250)));
        }

        using var ms = new MemoryStream();
        await image.SaveAsWebpAsync(ms, cancellationToken);

        var data = await File.ReadAllBytesAsync(fileName, cancellationToken);
        return data.ToDataUrl("webp");
    }
}
