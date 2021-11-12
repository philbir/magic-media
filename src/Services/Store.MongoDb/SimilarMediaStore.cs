using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MagicMedia.Store.MongoDb
{
    public class SimilarMediaStore : ISimilarMediaStore
    {
        private readonly MediaStoreContext _mediaStoreContext;

        public SimilarMediaStore(MediaStoreContext mediaStoreContext)
        {
            _mediaStoreContext = mediaStoreContext;
        }

        public async Task AddAsync(IEnumerable<SimilarMediaInfo> similarInfos, CancellationToken cancellationToken)
        {
            IEnumerable<ReplaceOneModel<SimilarMediaInfo>>? operations = similarInfos.Select(x =>
            {
                return new ReplaceOneModel<SimilarMediaInfo>(
                    Builders<SimilarMediaInfo>.Filter.Eq(f => f.Id, x.Id),x)
                    {
                        IsUpsert = true
                    };
            });

            await _mediaStoreContext.SimilarInfo.BulkWriteAsync(
                operations,
                options: null,
                cancellationToken);
        }

        public async Task<IEnumerable<SimilarMediaGroup>> GetSimilarGroupsAsync(
            SearchSimilarMediaRequest request,
            CancellationToken cancellationToken)
        {
            if (request.HashType < MediaHashType.ImageAverageHash)
            {
                return await GetSimilarGroupsFromMediaHashedAsync(request, cancellationToken);
            }
            else
            {
                return await GetSimilarGroupsFromSimilarMediaAsync(request, cancellationToken);
            }
        }

        private async Task<IEnumerable<SimilarMediaGroup>> GetSimilarGroupsFromMediaHashedAsync(SearchSimilarMediaRequest request, CancellationToken cancellationToken)
        {
            var parameters = new List<AggregationParameter>()
            {
                new AggregationParameter("HashType", (int) request.HashType),
                new AggregationParameter("Skip",request.PageNr * request.PageSize),
                new AggregationParameter("Limit",request.PageSize),
            };

            IEnumerable<BsonDocument> docs = await _mediaStoreContext.ExecuteAggregation(
                CollectionNames.Media,
                "Duplicate_ByHashes",
                parameters,
                cancellationToken);

            IEnumerable<SimilarMediaGroup> groups = docs.Select(x => new SimilarMediaGroup
            {
                Identifier = x["_id"] != BsonNull.Value ? x["_id"].AsString : null,
                Count = x["Count"].AsInt32,
                MediaIds = x["MediaIds"].AsBsonArray.Select(i => i.AsGuid)
            }).ToList();

            foreach (SimilarMediaGroup group in groups)
            {
                group.Id = group.MediaIds.First();
            }

            return groups;
        }

        private async Task<IEnumerable<SimilarMediaGroup>> GetSimilarGroupsFromSimilarMediaAsync(SearchSimilarMediaRequest request, CancellationToken cancellationToken)
        {
            var parameters = new List<AggregationParameter>()
            {
                new AggregationParameter("HashType", (int) request.HashType),
                new AggregationParameter("Similarity",request.Similarity),
                new AggregationParameter("Skip",request.PageNr * request.PageSize),
                new AggregationParameter("Limit",request.PageSize),
            };

            IEnumerable<BsonDocument> docs = await _mediaStoreContext.ExecuteAggregation(
                CollectionNames.SimilarMedia,
                "SimilarMedia",
                parameters,
                cancellationToken);

            return docs.Select(x => new SimilarMediaGroup
            {
                Id = x["_id"].AsGuid,
                Count = x["Count"].AsInt32,
                MediaIds = x["MediaIds"].AsBsonArray.Select(i => i.AsGuid)
            });
        }
    }


}
