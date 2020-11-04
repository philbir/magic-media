using System.IO;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace MagicMedia.Store.MongoDb
{
    public static class AggregationPipelineFactory
    {
        public static PipelineDefinition<BsonDocument, BsonDocument> Create(string name)
        {
            var json = GetResource(name);
            var stages = JArray.Parse(json);

            var pipeline = PipelineDefinition<BsonDocument, BsonDocument>
                .Create(stages.Select(x => BsonDocument.Parse(x.ToString())));

            return pipeline;
        }

        private static string GetResource(string name)
        {
            using Stream stream = typeof(AggregationPipelineFactory).Assembly
                .GetManifestResourceStream(
                 $"MagicMedia.Store.MongoDb.Aggregations.{name}.json");

            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }
    }
}
