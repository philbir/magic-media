using Squadron;

namespace MagicMedia.Store.MongoDb.Tests
{
    public class Mongo362Options : MongoDefaultOptions
    {
        public override void Configure(ContainerResourceBuilder builder)
        {
            base.Configure(builder);
            builder.Tag("3.6.20-xenial");
        }
    }
}
