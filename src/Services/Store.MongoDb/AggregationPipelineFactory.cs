using System;
using System.Collections.Generic;
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

        public static IEnumerable<BsonDocument> CreateStages(string name)
        {
            var json = GetResource(name);
            var stages = JArray.Parse(json);

            return stages.Select(x => BsonDocument.Parse(x.ToString()));
        }

        public static BsonDocument? CreateMatchInStage(
            IEnumerable<Guid>? ids,
            string fieldName = "_id")
        {
            if (ids != null)
            {
                return new BsonDocument
                {
                    {
                        "$match",
                        new BsonDocument
                            {
                                {fieldName, new BsonDocument
                                {
                                    {
                                        "$in", new BsonArray(ids)
                                    }
                                }}
                            }
                    }
                };
            }

            return null;
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
