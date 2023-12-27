using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Discovery;
using MagicMedia.Store;
using Microsoft.Extensions.Logging;

namespace MagicMedia;

public class DuplicateMediaGuard(IMediaStore mediaStore, ILogger<DuplicateMediaGuard> logger) : IDuplicateMediaGuard
{
    private HashSet<MediaHash> _hashes = new();

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        using Activity? activity = App.ActivitySource.StartActivity("Initialize duplicate guard");

        Dictionary<Guid, IEnumerable<MediaHash>>? medias = await mediaStore.GetAllHashesAsync(cancellationToken);

        _hashes = medias.SelectMany(x => x.Value)
            .Where(x =>
                x.Type == MediaHashType.FileHashSha256 ||
                x.Type == MediaHashType.Identifiers)
            .ToHashSet();

        activity?.AddTag("hashes.count", _hashes.Count);
    }

    public void AddMedia(IEnumerable<MediaHash> hashes)
    {
        foreach (MediaHash hash in hashes)
        {
            _hashes.Add(hash);
        }
    }

    public bool IsDuplicate(IEnumerable<MediaHash> hashes)
    {
        MediaHash fileHash = hashes.FirstOrDefault(x => x.Type == MediaHashType.FileHashSha256);
        MediaHash identifierHash = hashes.FirstOrDefault(x => x.Type == MediaHashType.Identifiers);

        if (fileHash is { } && _hashes.Contains(fileHash, new MediaHashComparer()))
        {
            return true;
        }

        if (identifierHash is { } && _hashes.Contains(identifierHash, new MediaHashComparer()))
        {
            logger.MediaIdentifierAllreadyExsists(identifierHash.Value);

            return true;
        }

        return false;
    }
}

