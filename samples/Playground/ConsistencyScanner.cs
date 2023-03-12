using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Processing;
using MagicMedia.Store;
using MagicMedia.Store.MongoDb;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MagicMedia.Playground;

public class ConsistencyScanner
{
    private readonly MediaStoreContext _storeContext;
    private readonly IMediaService _mediaService;

    public ConsistencyScanner(
        MediaStoreContext storeContext,
        IMediaService mediaService)
    {
        _storeContext = storeContext;
        _mediaService = mediaService;
    }

    public async Task CreateFileList()
    {
        var root = "/Users/p7e/nas/home/Photos/Moments";

        var files = new List<FileEntry>();
        var validExtensions = new[] { ".jpg", ".jpeg", ".mp4", ".mov" };

        foreach (string entry in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
        {
            var extension = Path.GetExtension(entry);

            if ( !string.IsNullOrWhiteSpace(extension) && validExtensions.Contains(extension))
            {
                var path = entry.Replace(root, "");
                files.Add(new FileEntry(Path.GetFileName(entry), Path.GetDirectoryName(path)));
            }

        }

        var json = JsonSerializer.Serialize(files);
        await File.WriteAllTextAsync(Path.Combine(root, "all_files.json"), json);
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var missingMediaTagId = Guid.Parse("81c97802-dc4e-46cc-a910-25cbcef62963");

        List<Store.Media> medias = await _storeContext.Medias.AsQueryable()
            .Where(x =>
                x.MediaType == MediaType.Image &&
                x.State == MediaState.Active)
            .OrderByDescending(x => x.DateTaken)
            .ToListAsync(cancellationToken);

        int total = medias.Count();
        int completed = 0;

        foreach (Media media in medias)
        {
            try
            {
                Console.WriteLine($"{completed} of {total} | Scanning faces: {media.Id}.");
                if (media.Tags.Any(x => x.DefinitionId == missingMediaTagId))
                {
                    completed++;
                    continue;
                }

                var fileName = _mediaService.GetFilename(media, MediaFileType.Original);

                if (!File.Exists(fileName))
                {
                    await _mediaService.SetMediaTagAsync(media.Id, new MediaTag { DefinitionId = missingMediaTagId },
                        cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            completed++;
        }
    }
}

record FileEntry(string Name, string Folder);
