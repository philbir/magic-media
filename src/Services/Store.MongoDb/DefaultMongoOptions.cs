using MongoDB.Driver;

namespace MagicMedia.Store.MongoDb
{
    public class DefaultMongoOptions
    {
        public static UpdateOptions Update => new UpdateOptions();

        public static DeleteOptions Delete => new DeleteOptions();

        public static InsertOneOptions InsertOne => new InsertOneOptions();

        public static InsertManyOptions InsertMany => new InsertManyOptions();

        public static ReplaceOptions Replace => new ReplaceOptions();
    }
}
