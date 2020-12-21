using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store.MongoDb.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using MongoDB.Extensions.Context;

namespace MagicMedia.Store.MongoDb
{
    public class MediaStoreContext : MongoDbContext
    {
        public MediaStoreContext(MongoOptions mongoOptions)
            : base(mongoOptions)
        {
        }

        protected override void OnConfiguring(IMongoDatabaseBuilder builder)
        {
            builder
                .RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String))
                .ConfigureConnection(con => con.ReadConcern = ReadConcern.Majority)
                .ConfigureConnection(con => con.WriteConcern = WriteConcern.WMajority)
                .ConfigureConnection(con => con.ReadPreference = ReadPreference.Primary)
                .ConfigureCollection(new FaceCollectionConfiguration())
                .ConfigureCollection(new CameraCollectionConfiguration())
                .ConfigureCollection(new PersonCollectionConfiguration())
                .ConfigureCollection(new UserCollectionConfiguration())
                .ConfigureCollection(new GroupCollectionConfiguration())
                .ConfigureCollection(new AlbumCollectionConfiguration())
                .ConfigureCollection(new MediaAICollectionConfiguration())
                .ConfigureCollection(new AuditEventCollectionConfiguration())
                .ConfigureCollection(new GeoAddressCacheCollectionConfiguration())
                .ConfigureCollection(new MediaCollectionConfiguration());
        }

        public IMongoCollection<Media> Medias
        {
            get
            {
                return CreateCollection<Media>();
            }
        }

        public IMongoCollection<MediaAI> MediaAI
        {
            get
            {
                return CreateCollection<MediaAI>();
            }
        }

        public IMongoCollection<MediaFace> Faces
        {
            get
            {
                return CreateCollection<MediaFace>();
            }
        }

        public IMongoCollection<Camera> Cameras
        {
            get
            {
                return CreateCollection<Camera>();
            }
        }

        public IMongoCollection<Person> Persons
        {
            get
            {
                return CreateCollection<Person>();
            }
        }

        public IMongoCollection<User> Users
        {
            get
            {
                return CreateCollection<User>();
            }
        }

        public IMongoCollection<Group> Groups
        {
            get
            {
                return CreateCollection<Group>();
            }
        }

        public IMongoCollection<GeoAddressCache> GeoAddressCache
        {
            get
            {
                return CreateCollection<GeoAddressCache>();
            }
        }

        public IMongoCollection<Album> Albums
        {
            get
            {
                return CreateCollection<Album>();
            }
        }

        public IMongoCollection<AuditEvent> AuditEvents
        {
            get
            {
                return CreateCollection<AuditEvent>();
            }
        }

        public IGridFSBucket CreateGridFsBucket()
        {
            return new GridFSBucket(Database, new GridFSBucketOptions());
        }

        internal async Task<IEnumerable<BsonDocument>> ExecuteAggregation(
            string collectionName,
            string name,
            CancellationToken cancellationToken)
        {
            PipelineDefinition<BsonDocument, BsonDocument> pipeline =
                AggregationPipelineFactory.Create(name);


            IMongoCollection<BsonDocument> collection = Database
                .GetCollection<BsonDocument>(collectionName);

            IAsyncCursor<BsonDocument> cursor = await collection.AggregateAsync(
                pipeline,
                options: null,
                cancellationToken);

            List<BsonDocument> documents = await cursor.ToListAsync(cancellationToken);

            return documents;
        }

        internal async Task<IEnumerable<BsonDocument>> ExecuteAggregation(
            string collectionName,
            string name,
            BsonDocument? prependStage=null,
            CancellationToken cancellationToken=default)
        {
            List<BsonDocument> stages = AggregationPipelineFactory.CreateStages(name).ToList();

            if (prependStage != null)
            {
                stages.Insert(0, prependStage);
            }

            PipelineDefinition<BsonDocument, BsonDocument> pipeline = PipelineDefinition<BsonDocument, BsonDocument>
                .Create(stages);

            IMongoCollection <BsonDocument> collection = Database
                .GetCollection<BsonDocument>(collectionName);

            IAsyncCursor<BsonDocument> cursor = await collection.AggregateAsync(
                pipeline,
                options: null,
                cancellationToken);

            List<BsonDocument> documents = await cursor.ToListAsync(cancellationToken);

            return documents;
        }
    }
}
