using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using CoenM.ImageHash.HashAlgorithms;
using MagicMedia.Store;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace MagicMedia.Processing;


public class CreateMediaHashesTask : IMediaProcessorTask
{
    public string Name => MediaProcessorTaskNames.CreateHashes;

    public Task ExecuteAsync(MediaProcessorContext context, CancellationToken cancellationToken)
    {
        var hashes = new List<MediaHash>();

        hashes.Add(new MediaHash
        {
            Type = MediaHashType.FileHashSha256,
            Value = ComputeFileHash(context)
        });

        hashes.Add(new MediaHash
        {
            Type = MediaHashType.Identifiers,
            Value = BuildUniqueIdentifier(context)
        });

        if (context.MediaType == MediaType.Image)
        {
            var avgHasher = new AverageHash();
            var percHasher = new PerceptualHash();
            var diffHasher = new DifferenceHash();

            Image<Rgba32> image = context.Image!.CloneAs<Rgba32>();

            hashes.Add(new MediaHash
            {
                Type = MediaHashType.ImageAverageHash,
                Value = avgHasher.Hash(image).ToString()
            });

            hashes.Add(new MediaHash
            {
                Type = MediaHashType.ImagePerceptualHash,
                Value = percHasher.Hash(image).ToString()
            });

            hashes.Add(new MediaHash
            {
                Type = MediaHashType.ImageDifferenceHash,
                Value = diffHasher.Hash(image).ToString()
            });
        }

        context.Hashes = hashes;

        return Task.CompletedTask;
    }

    private string ComputeFileHash(MediaProcessorContext context)
    {
        if (context.OriginalData != null)
        {
            return ComputeFileHash(context.OriginalData);
        }

        return ComputeFileHash(context.File.Id);
    }

    public string ComputeFileHash(byte[] data)
    {
        var sha = new SHA256Managed();
        byte[] checksum = sha.ComputeHash(data);
        return BitConverter.ToString(checksum).Replace("-", string.Empty);
    }

    public string ComputeFileHash(string filename)
    {
        var sha = new SHA256Managed();
        using var stream = new FileStream(filename, FileMode.Open);

        byte[] checksum = sha.ComputeHash(stream);
        return BitConverter.ToString(checksum).Replace("-", string.Empty);
    }

    public string BuildUniqueIdentifier(MediaProcessorContext context)
    {
        var frags = new List<string>();
        frags.Add(context.Metadata!.Camera?.Make ?? "NA");
        frags.Add(context.Metadata.Camera?.Model ?? "NA");
        frags.Add(Path.GetFileName(context.File.Id));
        frags.Add(context.Metadata.DateTaken.HasValue ? context.Metadata.DateTaken.Value.Ticks.ToString() : "NA");

        return string.Join("|", frags.Select(x => x.Trim()));
    }
}
