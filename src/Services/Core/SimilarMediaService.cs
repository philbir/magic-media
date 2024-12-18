using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoenM.ImageHash;
using MagicMedia.Store;
using Microsoft.Extensions.Logging;

namespace MagicMedia;

public class SimilarMediaService(
    IMediaStore mediaStore,
    ISimilarMediaStore similarMediaInfoStore,
    ILogger<SimilarMediaService> logger)
    : ISimilarMediaService
{
    public async Task GetDuplicatesAsync(CancellationToken cancellationToken)
    {
        Dictionary<Guid, IEnumerable<MediaHash>>? medias = await mediaStore.GetAllHashesAsync(cancellationToken);
        var todo = medias.Count;

        foreach (KeyValuePair<Guid, IEnumerable<MediaHash>> media in medias)
        {
            Console.WriteLine($"{todo} - {media.Key}");
            foreach (KeyValuePair<Guid, IEnumerable<MediaHash>> compareTarget in medias)
            {
                List<SimilarMediaInfo> similar = new List<SimilarMediaInfo>();

                if (media.Key != compareTarget.Key)
                {
                    foreach (MediaHash hash in media.Value.Where(x => x.Type >= MediaHashType.ImageAverageHash))
                    {
                        try
                        {
                            double similarity = GetSimilarity(
                                hash.Value,
                                compareTarget.Value,
                                hash.Type);

                            if (similarity > 99)
                            {
                                similar.Add(new SimilarMediaInfo
                                {
                                    Id = CreateKey(media.Key, compareTarget.Key),
                                    CreatedAt = DateTime.UtcNow,
                                    CompareType = hash.Type,
                                    Similarity = similarity,
                                    SourceMediaId = media.Key,
                                    TargetMediaId = compareTarget.Key,
                                });

                            }
                        }
                        catch (Exception ex)
                        {
                            logger.ErrorGetSimilarity(ex);
                        }
                    }
                }

                if (similar.Any())
                {
                    await similarMediaInfoStore.AddAsync(similar, cancellationToken);
                }
            }

            todo--;
        }
    }

    private string CreateKey(Guid a, Guid b)
    {
        var key = BitConverter.GetBytes(
            BitConverter.ToInt32(a.ToByteArray()) +
            BitConverter.ToInt32(b.ToByteArray()));

        return Convert.ToBase64String(key);
    }

    public async Task<IEnumerable<SimilarMediaGroup>> GetSimilarMediaGroupsAsync(
        SearchSimilarMediaRequest request,
        CancellationToken cancellationToken)
    {
        return await similarMediaInfoStore.GetSimilarGroupsAsync(request, cancellationToken);
    }

    private double GetSimilarity(string aHash, IEnumerable<MediaHash> b, MediaHashType type)
    {
        MediaHash? bHash = b.FirstOrDefault(x => x.Type == type);

        if (aHash != null && bHash != null)
        {
            var similarity = CompareHash.Similarity(ulong.Parse(aHash), ulong.Parse(bHash.Value));

            return similarity;
        }

        return 0;
    }
}

public static partial class SimilarMediaServiceLoggerExtensions
{
    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Error in GetSimilarity")]
    public static partial void ErrorGetSimilarity(this ILogger logger, Exception ex);
}
