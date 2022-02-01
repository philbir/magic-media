using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;
using Serilog;

namespace MagicMedia;

public class DuplicateMediaGuard : IDuplicateMediaGuard
{
    private readonly IMediaStore _mediaStore;
    private HashSet<MediaHash> _hashes = new HashSet<MediaHash>();

    public DuplicateMediaGuard(IMediaStore mediaStore)
    {
        _mediaStore = mediaStore;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        Dictionary<Guid, IEnumerable<MediaHash>>? medias = await _mediaStore.GetAllHashesAsync(cancellationToken);

        _hashes = medias.SelectMany(x => x.Value)
            .Where(x =>
                x.Type == MediaHashType.FileHashSha256 ||
                x.Type == MediaHashType.Identifiers)
            .ToHashSet();
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
            //Log.Information("Identifier also exists: {Identifier}", identifierHash.Value);

            return true;
        }

        return false;
    }
}

