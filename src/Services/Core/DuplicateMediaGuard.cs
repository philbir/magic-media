using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store;
using Microsoft.Extensions.Logging;

namespace MagicMedia;

public class DuplicateMediaGuard(
    IMediaStore mediaStore,
    ILogger<DuplicateMediaGuard> logger)
    : IDuplicateMediaGuard
{
    private readonly ILogger<DuplicateMediaGuard> _logger = logger;
    private HashSet<MediaHash> _hashes = new HashSet<MediaHash>();

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        using Activity? activity = Tracing.Source.StartActivity("Initialize duplicate guard");

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
            _logger.IdentifierAllreadyExists(identifierHash.Value);
            return true;
        }

        return false;
    }
}
public static partial class DuplicateMediaGuardLoggerExtensions
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Identifier allready exists: {Identifier}")]
    public static partial void IdentifierAllreadyExists(this ILogger logger, string identifier);
}


