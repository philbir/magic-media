using Squadron;

namespace MagicMedia.Api.Host.Tests.Containers
{
    public class MagicApiAppOptions : ComposeResourceOptions
    {
        public override void Configure(ComposeResourceBuilder builder)
        {
            builder.AddContainer<IdentityDbOptions>("mongo");
            builder.AddContainer<IdentityHostOptions>("identity")
                    .AddLink("mongo",
                             new EnvironmentVariableMapping(
                                "Identity:Database:ConnectionString",
                                "#CONNECTIONSTRING_INTERNAL#"));
        }
    }

    public class IdentityHostOptions : GenericContainerOptions
    {
        public override void Configure(ContainerResourceBuilder builder)
        {
            base.Configure(builder);
            builder
                .Name("identity")
                .WaitTimeout(30)
                .PreferLocalImage()
                .InternalPort(80)
                //.ExternalPort(5880)
                .AddNetwork("magic-api-net")
                .Image("magic-media-identity:dev");
        }
    }

    public class IdentityDbOptions : MongoDefaultOptions
    {
        public override void Configure(ContainerResourceBuilder builder)
        {
            base.Configure(builder);
            builder.AddNetwork("magic-api-net");
        }
    }
}
