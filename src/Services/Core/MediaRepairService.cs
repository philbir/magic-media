using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Configuration;
using MagicMedia.Extensions;
using MagicMedia.Security;
using MagicMedia.Store;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace MagicMedia;

public class MediaRepairService : IMediaRepairService
{
    private readonly IFileSystemSnapshotService _snapshotService;
    private readonly IMediaService _mediaService;
    private readonly IMediaStore _mediaStore;
    private readonly FileSystemStoreOptions _fileSystemStoreOptions;

    public MediaRepairService(
        IFileSystemSnapshotService snapshotService,
        IMediaService mediaService,
        IMediaStore mediaStore,
        FileSystemStoreOptions fileSystemStoreOptions)
    {
        _snapshotService = snapshotService;
        _mediaService = mediaService;
        _mediaStore = mediaStore;
        _fileSystemStoreOptions = fileSystemStoreOptions;
    }

    public async Task GetPossibleRepairsAsync(Media media, ConsistencyCheck check, CancellationToken cancellationToken)
    {
        switch (check.Name)
        {
            case "File_Original":
                await GetOriginalFileMissingRepairAsync(media, check, cancellationToken);
                break;
        }
    }

    private async Task GetOriginalFileMissingRepairAsync(
        Media media,
        ConsistencyCheck check,
        CancellationToken cancellationToken)
    {
        FileSystemSnapshot snapshot = await _snapshotService.LoadAsync(cancellationToken);

        IEnumerable<MediaFileEntry> files = snapshot.Entries.Where(x => x.Name.EndsWith(media.Filename));


        foreach (MediaFileEntry file in files)
        {
            var repair = new MediaRepair { Type = "UpdateFolder", Title = "Update media folder" };
            var repairMove = new MediaRepair { Type = "MoveFile", Title = "Move file" };

            repair.Parameters.Add(new(
                "Resolved_Path",
                file.Folder) { AddToAction = true });

            repair.Parameters.Add(new(
                "Resolved_Filename",
                file.Name) { AddToAction = true });

            /*
            var fileName = Path.Combine(_fileSystemStoreOptions.RootDirectory + file.Folder, file.Name);

            var dataUrl = await GetPreviewDataUrlAsync(fileName, cancellationToken);

            repair.Parameters.Add(new(
                "Found_Image",
                dataUrl));*/

            repairMove.Parameters.AddRange(repair.Parameters.ToList());

            check.Repairs.Add(repair);
            //check.Repairs.Add(repairMove);
        }
    }

    public async Task<Media> ExecuteRepairAsync(RepairMediaRequest request, CancellationToken cancellationToken)
    {
        Media media = await _mediaService.GetByIdAsync(request.MediaId, cancellationToken);

        switch (request.Type)
        {
            case "UpdateFolder":
                await ExecuteUpdateFolderRepairAsync(media, request, cancellationToken);
                break;
        }

        await RemoveCheckTagsAsync(media, cancellationToken);

        return media;
    }

    private async Task ExecuteUpdateFolderRepairAsync(Media media, RepairMediaRequest request,
        CancellationToken cancellationToken)
    {
        MediaRepairParameter newFolder = request.Parameters.Single(x => x.Name == "Resolved_Path");
        media.Folder = newFolder.Value.TrimStart(new[] { '/' });

        var resolvedName = request.Parameters.Single(x => x.Name == "Resolved_Filename").Value;

        if (resolvedName != media.Filename)
        {
            var oldPath = Path.Combine(_fileSystemStoreOptions.RootDirectory + newFolder.Value, resolvedName);
            var newPath = Path.Combine(_fileSystemStoreOptions.RootDirectory + newFolder.Value, media.Filename);

            File.Move(oldPath, newPath);
        }

        await _mediaStore.UpdateAsync(media, cancellationToken);
    }

    private async Task RemoveCheckTagsAsync(Media media, CancellationToken cancellationToken)
    {
        IReadOnlyList<TagDefintion> tagDefs = await _mediaStore.TagDefinitions.GetAllAsync(cancellationToken);
        IEnumerable<Guid> ids = tagDefs.Where(x => x.Name.StartsWith("CC-")).Select(x => x.Id);

        await _mediaStore.RemoveTagsByDefinitionIdAsync(media.Id, ids, cancellationToken);
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

        /*await File.WriteAllBytesAsync(
            Path.Combine(_fileSystemStoreOptions.RootDirectory + "/_RECOVERY", Guid.NewGuid() + ".webp" ),
            data,
            cancellationToken);*/

        return data.ToDataUrl("webp");
    }
}
