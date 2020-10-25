using Squadron;

namespace Identity.UI.Tests.Container
{
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
                .ExternalPort(80)
                .AddNetwork("identity-net")
                .AddEnvironmentVariable($"ECall:Password=***")
                .AddEnvironmentVariable($"ECall:From=***")
                .Image("magic-media-identity:dev");
        }
    }
}
