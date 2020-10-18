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
                .ConfigureCollection(new MediaCollectionConfiguration());
        }

        public IMongoCollection<Media> Medias
        {
            get
            {
                return CreateCollection<Media>();
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

        public IGridFSBucket CreateGridFsBucket()
        {
            return new GridFSBucket(Database, new GridFSBucketOptions());
        }
    }
}
