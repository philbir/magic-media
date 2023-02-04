using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Store.MongoDb.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;
using MongoDB.Driver.GridFS;
using MongoDB.Extensions.Context;

namespace MagicMedia.Store.MongoDb;
public class MediaStoreContext : MongoDbContext
{
    IMongoCollection<Media> _medias;
    IMongoCollection<MediaFace> _faces;
    IMongoCollection<Camera> _cameras;
    IMongoCollection<MediaAI> _mediaAI;
    IMongoCollection<Person> _persons;
    IMongoCollection<User> _users;
    IMongoCollection<Group> _groups;
    IMongoCollection<GeoAddressCache> _geoAddressCache;
    IMongoCollection<Album> _albums;
    IMongoCollection<AuditEvent> _auditEvents;
    IMongoCollection<ClientThumbprint> _clientThumbprints;
    IMongoCollection<SimilarMediaInfo> _similarInfo;

    public MediaStoreContext(MongoOptions mongoOptions)
        : base(mongoOptions)
    {
    }

    protected override void OnConfiguring(IMongoDatabaseBuilder builder)
    {
        builder.ConfigureConnection(settings =>
        {
            settings.ClusterConfigurator = cb => cb.Subscribe(new DiagnosticsActivityEventSubscriber(
                new InstrumentationOptions
                {
                    CaptureCommandText = true
                }));
        });

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
            .ConfigureCollection(new ClientThumbprintCollectionConfiguration())
            .ConfigureCollection(new SimilarMediaInfoCollectionConfiguration())
            .ConfigureCollection(new MediaCollectionConfiguration());
    }

    public IMongoCollection<Media> Medias
    {
        get
        {
            if (_medias is null)
            {
                _medias = CreateCollection<Media>();
            }

            return _medias;
        }
    }

    public IMongoCollection<MediaAI> MediaAI
    {
        get
        {
            if (_mediaAI is null)
            {
                _mediaAI = CreateCollection<MediaAI>();
            }

            return _mediaAI;
        }
    }

    public IMongoCollection<MediaFace> Faces
    {
        get
        {
            if (_faces is null)
            {
                _faces = CreateCollection<MediaFace>();
            }

            return _faces;
        }
    }

    public IMongoCollection<Camera> Cameras
    {
        get
        {
            if (_cameras is null)
            {
                _cameras = CreateCollection<Camera>();
            }

            return _cameras;
        }
    }

    public IMongoCollection<Person> Persons
    {
        get
        {
            if (_persons is null)
            {
                _persons = CreateCollection<Person>();
            }

            return _persons;
        }
    }

    public IMongoCollection<User> Users
    {
        get
        {
            if (_users is null)
            {
                _users = CreateCollection<User>();
            }

            return _users;
        }
    }

    public IMongoCollection<Group> Groups
    {
        get
        {
            if (_groups is null)
            {
                _groups = CreateCollection<Group>();
            }

            return _groups;
        }
    }

    public IMongoCollection<GeoAddressCache> GeoAddressCache
    {
        get
        {
            if (_geoAddressCache is null)
            {
                _geoAddressCache = CreateCollection<GeoAddressCache>();
            }

            return _geoAddressCache;
        }
    }

    public IMongoCollection<Album> Albums
    {
        get
        {
            if (_albums is null)
            {
                _albums = CreateCollection<Album>();
            }

            return _albums;
        }
    }

    public IMongoCollection<AuditEvent> AuditEvents
    {
        get
        {
            if (_auditEvents is null)
            {
                _auditEvents = CreateCollection<AuditEvent>();
            }

            return _auditEvents;
        }
    }

    public IMongoCollection<ClientThumbprint> ClientThumbprints
    {
        get
        {
            if (_clientThumbprints is null)
            {
                _clientThumbprints = CreateCollection<ClientThumbprint>();
            }

            return _clientThumbprints;
        }
    }

    public IMongoCollection<SimilarMediaInfo> SimilarInfo
    {
        get
        {
            if (_similarInfo is null)
            {
                _similarInfo = CreateCollection<SimilarMediaInfo>();
            }

            return _similarInfo;
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
        return await ExecuteAggregation(
            collectionName,
            name,
            Array.Empty<AggregationParameter>(),
            cancellationToken);
    }

    internal async Task<IEnumerable<BsonDocument>> ExecuteAggregation(
        string collectionName,
        string name,
        IEnumerable<AggregationParameter>? parameters,
        CancellationToken cancellationToken)
    {
        PipelineDefinition<BsonDocument, BsonDocument> pipeline =
            AggregationPipelineFactory.Create(name, parameters);

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
        BsonDocument? prependStage = null,
        CancellationToken cancellationToken = default)
    {
        List<BsonDocument> stages = AggregationPipelineFactory.CreateStages(name).ToList();

        if (prependStage != null)
        {
            stages.Insert(0, prependStage);
        }

        PipelineDefinition<BsonDocument, BsonDocument> pipeline = PipelineDefinition<BsonDocument, BsonDocument>
            .Create(stages);

        IMongoCollection<BsonDocument> collection = Database
            .GetCollection<BsonDocument>(collectionName);

        IAsyncCursor<BsonDocument> cursor = await collection.AggregateAsync(
            pipeline,
            options: null,
            cancellationToken);

        List<BsonDocument> documents = await cursor.ToListAsync(cancellationToken);

        return documents;
    }
}
