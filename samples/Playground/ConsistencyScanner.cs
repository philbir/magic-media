using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using CoenM.ImageHash;
using CoenM.ImageHash.HashAlgorithms;
using MagicMedia.Store;
using MagicMedia.Store.MongoDb;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace MagicMedia.Playground;

public class ConsistencyScanner
{
    private readonly MediaStoreContext _storeContext;
    private readonly IMediaService _mediaService;
    private readonly IFileSystemSnapshotService _fileSystemSnapshotService;
    private readonly PlaygroundOptions _options;

    public ConsistencyScanner(
        MediaStoreContext storeContext,
        IMediaService mediaService,
        IFileSystemSnapshotService fileSystemSnapshotService,
        PlaygroundOptions options)
    {
        _storeContext = storeContext;
        _mediaService = mediaService;
        _fileSystemSnapshotService = fileSystemSnapshotService;
        _options = options;
    }

    public async Task RepairMissingFile(CancellationToken cancellationToken)
    {
        var missingMediaTagId = Guid.Parse("81c97802-dc4e-46cc-a910-25cbcef62963");
        var repairTagId = Guid.Parse("3482fbbc-b0e3-46ba-b810-bf6aa06b4c1b");

        FileSystemSnapshot snapshot = await _fileSystemSnapshotService.LoadAsync(cancellationToken);

        List<Store.Media> medias = await _storeContext.Medias.AsQueryable()
            .Where(x => x.Tags.Any(t => t.DefinitionId == missingMediaTagId))
            .OrderByDescending(x => x.DateTaken)
            .Take(100)
            .ToListAsync(cancellationToken);

        int total = medias.Count();
        int completed = 0;

        foreach (Media media in medias)
        {
            try
            {
                Console.WriteLine($"{completed} of {total} | Finding file: {media.Id}.");

                if (media.Folder.StartsWith("New"))
                {
                    IEnumerable<MediaFileEntry> existing = snapshot.Entries.Where(x => x.Name == media.Filename);

                    if (existing.Any())
                    {
                        MediaFileEntry hashMatch = await MatchByHash(media, existing);

                        if (hashMatch is {})
                        {
                            Console.WriteLine($"Has Hash match with: {hashMatch.Folder}");
                            continue;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (existing.Count() == 1)
                    {
                        //Update folder?
                        Console.WriteLine($"Found in: {existing.First().Folder}: Now: {media.Folder}");

                        Console.WriteLine("Update folder?");

                        ConsoleKeyInfo key = Console.ReadKey();

                        if (key.KeyChar == 'y')
                        {
                            var newFolder = existing.First().Folder.TrimStart(new[] { '/' });

                            await _storeContext.Medias.UpdateOneAsync(
                                x => x.Id == media.Id,
                                Builders<Media>.Update.Set(x => x.Folder, newFolder),
                                new UpdateOptions(),
                                cancellationToken);

                            await _mediaService.SetMediaTagAsync(media.Id,
                                new MediaTag { DefinitionId = repairTagId, Data = $"Move from: {media.Folder}" },
                                cancellationToken);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }

    private async Task<MediaFileEntry> MatchByHash(Media media, IEnumerable<MediaFileEntry> files)
    {
        var percHasher = new PerceptualHash();

        foreach (MediaFileEntry file in files)
        {
            var filename = Path.Combine(_options.RootDirectory + file.Folder, file.Name);

            Image image = await Image.LoadAsync(filename);
            var clone = image.CloneAs<Rgba32>();

            var hash = percHasher.Hash(clone);

            var p = media.Hashes.FirstOrDefault(x => x.Type == MediaHashType.ImagePerceptualHash);

            if (p is { })
            {
                var value = Convert.ToUInt64(p.Value);

                var similar = CompareHash.Similarity(value, hash);

                if (similar == 100)
                    return file;
            }

        }

        return null;
    }

    public string ComputeFileHash(string filename)
    {
        var sha = new SHA256Managed();
        using var stream = new FileStream(filename, FileMode.Open);

        byte[] checksum = sha.ComputeHash(stream);
        return BitConverter.ToString(checksum).Replace("-", string.Empty);
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

