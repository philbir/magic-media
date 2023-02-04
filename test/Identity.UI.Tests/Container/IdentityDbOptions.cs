using Squadron;

namespace Identity.UI.Tests.Container
{
    public class IdentityDbOptions : MongoDefaultOptions
    {
        public override void Configure(ContainerResourceBuilder builder)
        {
            base.Configure(builder);
            builder.AddNetwork("identity-net");
        }
    }
}
