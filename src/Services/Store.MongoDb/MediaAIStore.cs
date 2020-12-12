using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Search;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;


namespace MagicMedia.Store.MongoDb
{
    public class MediaAIStore : IMediaAIStore
    {
        private readonly MediaStoreContext _mediaStoreContext;

        public MediaAIStore(MediaStoreContext mediaStoreContext)
        {
            _mediaStoreContext = mediaStoreContext;
        }

        public async Task<MediaAI> GetByMediaIdAsync(
            Guid mediaId,
            CancellationToken cancellationToken)
        {
            return await _mediaStoreContext.MediaAI.AsQueryable()
                .Where(x => x.MediaId == mediaId)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<SearchFacetItem>> GetGroupedAITagsAsync(
            CancellationToken cancellationToken)
        {
            IEnumerable<BsonDocument> docs = await _mediaStoreContext.ExecuteAggregation(
                CollectionNames.MediaAI,
                "MediaAI_GroupByTag",
                cancellationToken);

            var result = new List<SearchFacetItem>();

            foreach (BsonDocument doc in docs)
            {
                var item = new SearchFacetItem();
                item.Count = doc["count"].AsInt32;

                if (doc["_id"].IsString)
                {
                    item.Value = doc["_id"].AsString;
                    item.Text = TitleCase(item.Value.Replace("_", " "));
                    result.Add(item);
                }
            }

            return result;
        }

        public async Task<IEnumerable<SearchFacetItem>> GetGroupedAIObjectsAsync(
            CancellationToken cancellationToken)
        {
            IEnumerable<BsonDocument> docs = await _mediaStoreContext.ExecuteAggregation(
                CollectionNames.MediaAI,
                "MediaAI_GroupByObject",
                cancellationToken);

            var result = new List<SearchFacetItem>();

            foreach (BsonDocument doc in docs)
            {
                var item = new SearchFacetItem();
                item.Count = doc["count"].AsInt32;

                if (doc["_id"].IsString)
                {
                    item.Value = doc["_id"].AsString;
                    item.Text = TitleCase(item.Value.Replace("_", " "));
                    result.Add(item);
                }
            }

            return result;
        }


        private string TitleCase(string input)
        {
            return input[0].ToString().ToUpper() + input[1..];
        }
    }
}
