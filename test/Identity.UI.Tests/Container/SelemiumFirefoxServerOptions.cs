using Squadron;

namespace Identity.UI.Tests.Container
{
    public class SelemiumFirefoxServerOptions : GenericContainerOptions
    {
        public override void Configure(ContainerResourceBuilder builder)
        {
            base.Configure(builder);
            builder
                .Name("selenium-firefox")
                .WaitTimeout(30)
                .InternalPort(4444)
                .ExternalPort(4444)
                .Image("selenium/standalone-firefox:latest")
                .AddVolume("/dev/shm:/dev/shm")
                .AddNetwork("identity-net");
        }
    }
}
